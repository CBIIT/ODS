using Amazon.S3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Theradex.ODS.Manager.Helpers;
using Theradex.ODS.Manager.Helpers.Extensions;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Manager.Models;
using Theradex.ODS.Manager.Models.Configuration;
using Theradex.ODS.Models;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Theradex.ODS.Manager.Processors
{
    public class ODSManager_Processor : BaseProcessor, IProcessor
    {
        const int MaxPageData = 50000;
        private const string PAYLOAD = "Payload";

        public ODSManager_Processor(
            IMedidataRWSService medidateRWSService,
            ILogger<ODSManager_Processor> logger,
            IOptions<AppSettings> appOptions,
            IAWSCoreHelper awsCoreHelper,
            IBatchRunControlRepository<BatchRunControl> odsRepository,
            IAmazonS3 s3Client) : base(medidateRWSService, logger, appOptions, awsCoreHelper, odsRepository, s3Client)
        {
        }

        public async Task<bool> ProcessAsync(ExtractorInput exInput)
        {
            try
            {
                string rootDirectory = @"C:\Path\To\ResponseFiles\Intervals\20231023_134033\"; // Replace with the root directory path
                var isSuccess = await LoadSingle(rootDirectory,exInput.TableName);
                return isSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private async Task<bool> LoadAllDataFromFileSystem()
        {
            string rootDirectory = @"C:\Path\To\ResponseFiles\Intervals\20231023_134033\"; // Replace with the root directory path

            List<string> lstTables = new List<string>
                            {
                                "DATAPOINTROLESTATUS",
                                //"DATAPOINTS",
                                "DATAPOINTREVIEWSTATUS",
                                //"REPORTINGRECORDSEXT2",
                                "DATADICTIONARYENTRIES",
                                //"RECORDS",
                                "FIELDRESTRICTIONS",
                                "REPORTINGLABDATAPOINTS",
                                "REPORTINGRECORDS",
                                //"FIELDS",
                                "DATAPAGES",
                                "VARIABLES",
                                "FORMRESTRICTIONS",
                                "INSTANCES",
                                "SUBJECTMATRIX",
                                "FOLDERFORMS",
                                "DATADICTIONARIES",
                                "SUBJECTROLESTATUS",
                                "USERSTUDYSITES",
                                "DERIVATIONSTEPS",
                                "USEROBJECTROLE",
                                "FORMS",
                                "FOLDERS",
                                "REPORTINGLABDPDELETES",
                                "LOCALIZEDDATASTRINGS",
                                "DERIVATIONS",
                                "LOCALIZEDDATASTRINGPKS",
                                "SUBJECTSTATUSHISTORY",
                                //"SUBJECTS",
                                "LOCALIZEDSTRINGS",
                                "USERS",
                                "STUDYSITES",
                                "EXTERNALUSERS",
                                "LABASSIGNMENTS",
                                //"SITES",
                                "STUDIES",
                                "VARIABLECHANGEAUDITS",
                                "LABS",
                                "PROJECTS",
                                "CONFIGURATION",
                                "ROLESUBJECTSTATUSACCESS",
                                "LABUNITDICTIONARYENTRIES",
                                "LABUNITS",
                                "ROLESALLMODULES",
                                "LABUNITDICTIONARIES",
                                "RANGETYPEVARIABLES",
                                "SUBJECTSTATUS",
                                "FIELDOIDDIRECTORY",
                                "LABSTANDARDGROUPENTRIES",
                                "LABSTANDARDGROUPS",
                                "LOCALIZATIONCONTEXTS",
                                "LOCALIZATIONS",
                                "SITEGROUPS",
                                "PROJECTSOURCESYSTEMR",
                                "SUBJECTSTATUSCATEGORYR",
                                "LABUNITCONVERSIONS",
                                "LABUPDATEQUEUE",
                                "UPLOADDATAPOINTS"
                            };

            foreach (string table in lstTables)
            {
                await LoadSingle(rootDirectory, table);
            }
            return true;
        }
        private async Task<bool> LoadSingle(string basePath, string tableName)
        {
            string format = "yyyy-MM-dd HH:mm:ss";
            string searchPattern = "*.json"; // Specify the file name pattern
            List<string> jsonFiles = new List<string>();

            // Search for JSON files in the root directory and subdirectories
            foreach (string filePath in Directory.GetFiles(basePath + tableName, searchPattern, SearchOption.AllDirectories))
            {
                if (!filePath.EndsWith("T23_59_59_9990000.json")) continue;
                jsonFiles.Add(filePath);

            }

            var tablejsons = jsonFiles;

            List<IntervalDataItem> tableIntervals = new List<IntervalDataItem>();

            foreach (string file in tablejsons)
            {
                string json = File.ReadAllText(file);
                // Parse the JSON string into a JObject
                JObject root = JObject.Parse(json);

                // Assuming 'root' is your JObject containing the JSON data
                JArray data = null;

                try
                {
                    // Check if 'Payload' and 'Data' properties exist
                    if (root[PAYLOAD] != null && root[PAYLOAD].Count() > 0 && root[PAYLOAD][0] != null && root[PAYLOAD][0]["Data"] != null)
                    {
                        // Parse the 'Data' property into a JArray
                        data = JArray.Parse(root[PAYLOAD][0]["Data"].ToString());
                    }
                    else
                    {
                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}] - One or more required properties are missing.");
                        // Handle the case where the required properties are missing
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}] - An error occurred: {ex.Message}");
                    // Handle the exception here
                }

                // Check if 'data' is null (indicating an issue with JSON parsing)
                if (data == null)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}]  - Failed to parse JSON data.");

                    data = new JArray();
                    // Handle the case where JSON parsing failed
                }

                // Check if the "Data" property is an empty array
                if (data.Count == 0)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}] -  DATA IS AN EMPTY ARRAY");
                    continue;
                }

                try
                {
                    // Deserialize the JSON string
                    var payload = JsonConvert.DeserializeObject<TableIntervalPayload>(json);
                    if (payload.Payload.Count > 0 && payload.Payload[0]?.Data != null && payload.Payload[0].Data.Count > 0)
                    {
                        tableIntervals.AddRange(payload.Payload[0].Data);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}] -  Unable to Serilize");
                }

            }
            tableIntervals = tableIntervals.OrderBy(i => i.Start).ToList();

            try
            {
                var CONFIG_RWSUSERNAME = "SPEC_TRACK_RWS";
                var CONFIG_RWSPASSWORD = "Password@01";

                List<BatchRunControl> records = new List<BatchRunControl>();

                foreach (var data in tableIntervals)
                {
                    try
                    {
                        var batchRunControlRow = new BatchRunControl();

                        batchRunControlRow.TableName = tableName;
                        batchRunControlRow.ApiStartDate = data.BucketStart;
                        batchRunControlRow.ApiEndDate = data.BucketEnd;
                        batchRunControlRow.JobStartTime = data.Start;
                        batchRunControlRow.JobEndTime = data.End;
                        batchRunControlRow.Slot = data.Bucket;
                        batchRunControlRow.RaveUsername = CONFIG_RWSUSERNAME;
                        batchRunControlRow.RavePassword = CONFIG_RWSPASSWORD;
                        batchRunControlRow.NoOfRecords = data.Records;
                        batchRunControlRow.IsRunCompleteFlag = "false";
                        batchRunControlRow.Payload = null;
                        records.Add(batchRunControlRow);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ERROR {ex.StackTrace}");
                        throw;
                    }

                }

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; + Finding Duplicates [Before-Count - {records.Count} ]");

                // Use LINQ to group by table_name and slot, and select the first record from each group
                List<BatchRunControl> distinctRecords = records
                    .GroupBy(record => new { record.TableName, record.Slot })
                .Select(group => group.First())
                    .ToList();

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Finding Duplicates [After-Count - {distinctRecords.Count} ]");

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; + Saving to database");

                await _odsRepository.AddMultipleAsync(distinctRecords);

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database");

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database success.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Error processing ${ex.ToString()}.");
                throw;
            }

            _logger.LogInformation($"TraceId:{_appSettings.TraceId};DONE");

            return false;
        }
        private async Task<bool> LoadAllDataFromFileSystemOld()
        {
            string format = "yyyy-MM-dd HH:mm:ss";

            string rootDirectory = @"C:\Path\To\ResponseFiles\Intervals\20231023_134033\"; // Replace with the root directory path
            string searchPattern = "*.json"; // Specify the file name pattern


            List<string> lstTables = new List<string>
                            {
                                "DATAPOINTROLESTATUS",
                                "DATAPOINTS",
                                "DATAPOINTREVIEWSTATUS",
                                "REPORTINGRECORDSEXT2",
                                //"DATADICTIONARYENTRIES",
                                "RECORDS",
                                "FIELDRESTRICTIONS",
                                "REPORTINGLABDATAPOINTS",
                                "REPORTINGRECORDS",
                                //"FIELDS",
                                "DATAPAGES",
                                "VARIABLES",
                                "FORMRESTRICTIONS",
                                "INSTANCES",
                                "SUBJECTMATRIX",
                                "FOLDERFORMS",
                                "DATADICTIONARIES",
                                "SUBJECTROLESTATUS",
                                "USERSTUDYSITES",
                                "DERIVATIONSTEPS",
                                "USEROBJECTROLE",
                                "FORMS",
                                "FOLDERS",
                                "REPORTINGLABDPDELETES",
                                "LOCALIZEDDATASTRINGS",
                                "DERIVATIONS",
                                "LOCALIZEDDATASTRINGPKS",
                                "SUBJECTSTATUSHISTORY",
                                //"SUBJECTS",
                                "LOCALIZEDSTRINGS",
                                "USERS",
                                "STUDYSITES",
                                "EXTERNALUSERS",
                                "LABASSIGNMENTS",
                                "SITES",
                                "STUDIES",
                                "VARIABLECHANGEAUDITS",
                                "LABS",
                                "PROJECTS",
                                "CONFIGURATION",
                                "ROLESUBJECTSTATUSACCESS",
                                "LABUNITDICTIONARYENTRIES",
                                "LABUNITS",
                                "ROLESALLMODULES",
                                "LABUNITDICTIONARIES",
                                "RANGETYPEVARIABLES",
                                "SUBJECTSTATUS",
                                "FIELDOIDDIRECTORY",
                                "LABSTANDARDGROUPENTRIES",
                                "LABSTANDARDGROUPS",
                                "LOCALIZATIONCONTEXTS",
                                "LOCALIZATIONS",
                                "SITEGROUPS",
                                "PROJECTSOURCESYSTEMR",
                                "SUBJECTSTATUSCATEGORYR",
                                "LABUNITCONVERSIONS",
                                "LABUPDATEQUEUE",
                                "UPLOADDATAPOINTS"
                            };

            //lstTables = new List<string> { "DATAPOINTS" };

            Dictionary<string, List<IntervalDataItem>> tablesData = new Dictionary<string, List<IntervalDataItem>>();

            foreach (string table in lstTables)
            {
                List<string> jsonFiles = new List<string>();

                // Search for JSON files in the root directory and subdirectories
                foreach (string filePath in Directory.GetFiles(rootDirectory + table, searchPattern, SearchOption.AllDirectories))
                {
                    if (!filePath.EndsWith("T23_59_59_9990000.json")) continue;
                    jsonFiles.Add(filePath);

                }

                var tablejsons = jsonFiles;

                List<IntervalDataItem> tableIntervals = new List<IntervalDataItem>();

                foreach (string file in tablejsons)
                {
                    string json = File.ReadAllText(file);
                    // Parse the JSON string into a JObject
                    JObject root = JObject.Parse(json);

                    // Assuming 'root' is your JObject containing the JSON data
                    JArray data = null;

                    try
                    {
                        // Check if 'Payload' and 'Data' properties exist
                        if (root[PAYLOAD] != null && root[PAYLOAD].Count() > 0 && root[PAYLOAD][0] != null && root[PAYLOAD][0]["Data"] != null)
                        {
                            // Parse the 'Data' property into a JArray
                            data = JArray.Parse(root[PAYLOAD][0]["Data"].ToString());
                        }
                        else
                        {
                            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}] - One or more required properties are missing.");
                            // Handle the case where the required properties are missing
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}] - An error occurred: {ex.Message}");
                        // Handle the exception here
                    }

                    // Check if 'data' is null (indicating an issue with JSON parsing)
                    if (data == null)
                    {
                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}]  - Failed to parse JSON data.");

                        data = new JArray();
                        // Handle the case where JSON parsing failed
                    }

                    // Check if the "Data" property is an empty array
                    if (data.Count == 0)
                    {
                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}] -  DATA IS AN EMPTY ARRAY");
                        continue;
                    }

                    try
                    {
                        // Deserialize the JSON string
                        var payload = JsonConvert.DeserializeObject<TableIntervalPayload>(json);
                        if (payload.Payload.Count > 0 && payload.Payload[0]?.Data != null && payload.Payload[0].Data.Count > 0)
                        {
                            tableIntervals.AddRange(payload.Payload[0].Data);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [FileName:{file}] -  Unable to Serilize");
                    }

                }
                tableIntervals = tableIntervals.OrderBy(i => i.Start).ToList();
                tablesData.Add(table, tableIntervals);
            }

            _logger.LogInformation($"TraceId:{_appSettings.TraceId};DONE");

            try
            {
                var CONFIG_RWSSERVER = "theradex.mdsol.com";
                var CONFIG_RWSUSERNAME = "SPEC_TRACK_RWS";
                var CONFIG_RWSPASSWORD = "Password@01";
                var CONFIG_PRODUCTIONREADY = false;

                foreach (var item in tablesData)
                {
                    List<BatchRunControl> records = new List<BatchRunControl>();
                    foreach (var data in item.Value)
                    {
                        try
                        {
                            var batchRunControlRow = new BatchRunControl();

                            batchRunControlRow.TableName = item.Key;
                            batchRunControlRow.ApiStartDate = data.BucketStart;
                            batchRunControlRow.ApiEndDate = data.BucketEnd;
                            batchRunControlRow.JobStartTime = data.Start;
                            batchRunControlRow.JobEndTime = data.End;
                            batchRunControlRow.Slot = data.Bucket;
                            batchRunControlRow.RaveUsername = CONFIG_RWSUSERNAME;
                            batchRunControlRow.RavePassword = CONFIG_RWSPASSWORD;
                            batchRunControlRow.NoOfRecords = data.Records;
                            batchRunControlRow.IsRunCompleteFlag = "false";
                            batchRunControlRow.Payload = null;
                            records.Add(batchRunControlRow);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"ERROR {ex.StackTrace}");
                            throw;
                        }

                    }

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; + Finding Duplicates [Before-Count - {records.Count} ]");

                    // Use LINQ to group by table_name and slot, and select the first record from each group
                    List<BatchRunControl> distinctRecords = records
                        .GroupBy(record => new { record.TableName, record.Slot })
                    .Select(group => group.First())
                        .ToList();

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Finding Duplicates [After-Count - {distinctRecords.Count} ]");

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; + Saving to database");

                    await _odsRepository.AddMultipleAsync(distinctRecords);

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database");
                }
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database success.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Error processing ${ex.ToString()}.");
                throw;
            }

            return true;
        }
    }
}