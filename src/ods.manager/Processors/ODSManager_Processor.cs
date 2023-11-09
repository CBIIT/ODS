using Amazon.S3;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Theradex.ODS.Manager.Helpers;
using Theradex.ODS.Manager.Helpers.Extensions;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Manager.Models;
using Theradex.ODS.Manager.Models.Configuration;
using Theradex.ODS.Models;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Extension.Methods;
using System.Drawing.Printing;
using RestSharp;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json.Linq;
using Amazon.DynamoDBv2.Model;
using NLog.LayoutRenderers;
using Org.BouncyCastle.Asn1.Pkcs;
using static ServiceStack.Diagnostics.Events;

namespace Theradex.ODS.Manager.Processors
{
    public class ODSManager_Processor : BaseProcessor, IProcessor
    {
        static List<string> ALLODSTABLES = new List<string>
                            {
                                //"DATAPOINTROLESTATUS",
                                "DATAPOINTS",
                                "DATAPOINTREVIEWSTATUS",
                                "REPORTINGRECORDSEXT2",
                                "DATADICTIONARYENTRIES",
                                "RECORDS",
                                "FIELDRESTRICTIONS",
                                "REPORTINGLABDATAPOINTS",
                                "REPORTINGRECORDS",
                                "FIELDS",
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
                                "SUBJECTS",
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

        const int MaxPageData = 50000;
        private const string PAYLOAD = "Payload";

        public ODSManager_Processor(
            IMedidataRWSService medidateRWSService,
            ILogger<ODSManager_Processor> logger,
            IOptions<AppSettings> appOptions,
            IAWSCoreHelper awsCoreHelper,
            IBatchRunControlRepository<BatchRunControl> odsRepository,
            IManagerTableInfoRepository<BatchRunControl> odsManagerRepository,
            IAmazonS3 s3Client) : base(medidateRWSService, logger, appOptions, awsCoreHelper, odsRepository, odsManagerRepository, s3Client)
        {
        }

        public async Task<bool> ProcessAsync(ExtractorInput exInput)
        {
            try
            {
                //var batchMetadata = await GetExtractionDates();
                await GetODSTableIntervalDate();
                return true;
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



        private async Task<Dictionary<string, BatchRunControl>> GetExtractionDates()
        {

            string baseUrl = "/RaveWebServices/datasets/ThxExtracts2.json";
            string baseFilePath = @"ODS\Manager\Data\";

            Dictionary<string, BatchRunControl> mapBatchRunControls = new Dictionary<string, BatchRunControl>();

            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            if (!Directory.Exists(baseFilePath))
            {
                Directory.CreateDirectory(baseFilePath);
            }

            foreach (var tableName in ALLODSTABLES)
            {
                var odsData = new ODSData();
                odsData.TableName = tableName;
                odsData.StartDate = DateTime.Now.AddDays(-7);
                odsData.EndDate = odsData.StartDate.AddMilliseconds(2);
                odsData.FilePath = baseFilePath;
                odsData.URL = baseUrl;
                var batchRunControl = await ExtractODSTableMetadata(odsData);
                mapBatchRunControls[tableName] = batchRunControl;
                batchRunControls.Add(batchRunControl);
            }

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; + Saving to database");

            await _odsManagerRepository.AddMultipleAsync(batchRunControls);

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database");

            return mapBatchRunControls;
        }

        private async Task<BatchRunControl> ExtractODSTableMetadata(ODSData odsData)
        {
            BatchRunControl batchRunControl = new BatchRunControl();

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Manager");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Current TableName : " + odsData.TableName);
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            // Create a policy for retry with exponential back-off
            var retryPolicy = Policy
                .Handle<HttpRequestException>() // Specify the exception to handle
                .WaitAndRetryAsync(
                    10, // Retry 5 times
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // Exponential back-off
                );

            // Create a policy for the circuit breaker with a threshold of 3 failures
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromMinutes(1) // Break for 1 minute on failure
                );

            string formattedStartDate = odsData.StartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
            string formattedEndDate = odsData.EndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

            //string formattedEndDate = DateTime.Now.AddDays(-6).ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

            string responseDataFileName = Path.Combine(odsData.FilePath, $"{odsData.TableName}_{formattedStartDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_TO_{formattedEndDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}");


            int pageSize = 1; // Set your desired page size
            var totalPages = 1;
            int pageNumber = 1; // Initialize pageNumber

            Payloads payloads = new Payloads();
            payloads.Payload = new List<PayloadItem>();


            string responseDataFileNameWithExtensionRAW = responseDataFileName + "_RAW.json";
            string responseDataFileNameWithExtension = responseDataFileName + ".json";
            string error_responseDataFileNameWithExtensionRAW = responseDataFileName + "ERROR_RAW.json";
            string error_responseDataFileNameWithExtension = responseDataFileName + "ERROR.json";

            try
            {

                string url = $"{odsData.URL}?PageSize={pageSize}&PageNumber={pageNumber}&StartDate={formattedStartDate}&EndDate={formattedEndDate}&TableName={odsData.TableName}";

                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Call the Medidata Rave service
                    var response = await _medidateRWSService.GetAndWriteToDiskWithResponse(odsData.TableName, url, responseDataFileNameWithExtensionRAW);

                    // Check for a failed response
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogError($"HTTP request failed. [Error Exception : {response?.ErrorException}] [Content: {response?.Content}] ");
                        throw new HttpRequestException($"HTTP request failed. [Error Exception : {response?.ErrorException}] [Content: {response?.Content}] ");
                        //return ; // Exit the loop on failure
                    }

                    var json = response.Content;

                    // Serialize the modified object back to JSON
                    batchRunControl.TableName = odsData.TableName;
                    batchRunControl.ApiStartDate = formattedStartDate;
                    batchRunControl.ApiStartDate = formattedEndDate;
                    batchRunControl.HttpStatusCode = response.StatusCode.ToString();
                    batchRunControl.RaveDataUrl = url;
                    batchRunControl.Success = response.StatusCode.ToString();
                    batchRunControl.ExtractedFileName = responseDataFileNameWithExtension;

                    json = json.Replace("\"Data\":, \"JsonDataChecksum\":", "\"Data\":[], \"JsonDataChecksum\":");

                    // Deserialize JSON into a C# object
                    Payloads? payload;
                    try
                    {
                        payload = JsonConvert.DeserializeObject<Payloads>(json);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"TraceId:{_appSettings.TraceId}; Error parsing response received. [Table:{odsData.TableName}] " +
                                                   $"[StartDate:{odsData.StartDate}]" + $"[EndDate:{odsData.EndDate}] " + $"[URL:{odsData.URL}] ");
                        throw;
                    }


                    batchRunControl.Payload = json;
                    batchRunControl.Payloads = payload != null ? payload : new Payloads();

                    if (payload != null)
                    {
                        // Remove the "Data" property from the object
                        foreach (var item in payload.Payload)
                        {
                            item.Data = null;
                        }

                        var payloadReceived = payload.Payload.FirstOrDefault();

                        if (payloadReceived != null)
                        {
                            batchRunControl.NoOfRecords = payloadReceived.TableRowCount;
                            batchRunControl.NoOfRecordsRetrieved = payloadReceived.QueryCountofRows;
                            batchRunControl.TotalPages = payloadReceived.TotalPages;
                            batchRunControl.PageSize = payloadReceived.PageSize;
                            batchRunControl.PageNumber = payloadReceived.PageNumber;

                            totalPages = payloadReceived.TotalPages;

                            await File.WriteAllTextAsync(responseDataFileNameWithExtension, json);

                        }
                    }
                });

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; STARTED - Calling Save to Postgres database  [Table:{odsData.TableName}] " +
                                           $"[StartDate:{odsData.StartDate}]" + $"[EndDate:{odsData.EndDate}] " + $"[URL:{odsData.URL}] ");

            }
            catch (HttpRequestException ex)
            {
                // Handle the exception
                File.WriteAllText(error_responseDataFileNameWithExtensionRAW, ex.Message);
                File.WriteAllText(error_responseDataFileNameWithExtension, ex.Message);
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Critical Error Occurred. {ex}");
                batchRunControl.ExtractedFileName = error_responseDataFileNameWithExtension;
                batchRunControl.ErrorMessage = ex.StackTrace;
                batchRunControl.Success = "FAILED";
            }
            catch (BrokenCircuitException ex)
            {
                // The circuit breaker is open, handle accordingly
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Circuit breaker is open. {ex}");
            }

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            return batchRunControl;

        }


        private async Task GetODSTableIntervalDate()
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            var odsmanager_table_matadatas = await _odsManagerRepository.GetAllAsync();

            string baseUrl = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";
            string baseFilePath = @"ODS\Manager\Data\";


            if (odsmanager_table_matadatas.Any())
            {
                foreach (var odsmanager_table_matadata in odsmanager_table_matadatas)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{odsmanager_table_matadata.Id}]  [Table:{odsmanager_table_matadata.TableName}] Started");


                    DateTime minDate = DateTime.Now;

                    // Deserialize JSON into a C# object
                    Payloads? payload;
                    try
                    {
                        payload = JsonConvert.DeserializeObject<Payloads>(odsmanager_table_matadata.Payload);
                        if (payload != null)
                        {
                            odsmanager_table_matadata.Payloads = payload;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"TraceId:{_appSettings.TraceId}; Error parsing response received. [Table:{odsmanager_table_matadata.TableName}] ");
                        throw;
                    }

                    odsmanager_table_matadata.RaveDataUrl = "/RaveWebServices/datasets/ThxExtracts2.json";

                    if (odsmanager_table_matadata.Payloads.Payload != null && odsmanager_table_matadata.Payloads.Payload.Count == 1)
                    {
                        var isMinDateValid = DateTime.TryParse(odsmanager_table_matadata.Payloads.Payload.First().MinDate, out minDate);
                        if (!isMinDateValid)
                        {
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Min date not valid.");

                            minDate = new DateTime(2000,01,01);
                            odsmanager_table_matadata.RaveDataUrl = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Defaulting to 01/01/2000");
                        }

                    }

                    ODSData oDSData = new ODSData();
                    oDSData.TableName = odsmanager_table_matadata.TableName;
                    oDSData.StartDate = minDate;
                    oDSData.URL = baseUrl;
                    oDSData.FilePath = baseFilePath;

                    var batchRunControl = await GeneratODSTableIntervals(odsmanager_table_matadata, oDSData);
                    //var batchRunControl = await ExtractODSTableIntervals(odsmanager_table_matadata, oDSData);

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{odsmanager_table_matadata.Id}]  [Table:{odsmanager_table_matadata.TableName}] Ended");
                }
            }
        }
        private async Task<List<BatchRunControl>> GeneratODSTableIntervals(BatchRunControl batchRunControl, ODSData odsData)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Manager - Extract ODS Table Intervals");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{batchRunControl.Id}]  [Table:{batchRunControl.TableName}] Started");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            DateTime startDate = odsData.StartDate;
            startDate = new DateTime(startDate.Year, startDate.Month, 1);

            DateTime endDate = startDate.AddDays(7);
            DateTime currentEndDate = DateTime.Now;

            // Subtract one day to get the last day of the current month
            endDate = endDate.AddMilliseconds(-1);

            while (startDate <= currentEndDate)
            {
                string formattedStartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                string formattedEndDate = endDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

                try
                {
                    var CONFIG_RWSUSERNAME = "SPEC_TRACK_RWS";
                    var CONFIG_RWSPASSWORD = "Password@01";


                    string bucket = $"{formattedStartDate}_{formattedEndDate}";

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Table:{batchRunControl.TableName}] Started:{bucket}");
                    var batchRunControlRow = new BatchRunControl();

                    batchRunControlRow.TableName = batchRunControl.TableName;
                    batchRunControlRow.ApiStartDate = formattedStartDate;
                    batchRunControlRow.ApiEndDate = formattedEndDate;
                    batchRunControlRow.JobStartTime = null;
                    batchRunControlRow.JobEndTime = null;
                    batchRunControlRow.Slot = bucket;
                    batchRunControlRow.RaveUsername = CONFIG_RWSUSERNAME;
                    batchRunControlRow.RavePassword = CONFIG_RWSPASSWORD;
                    batchRunControlRow.RaveDataUrl = batchRunControl.RaveDataUrl;
                    batchRunControlRow.NoOfRecords = batchRunControl.NoOfRecords;
                    batchRunControlRow.IsRunCompleteFlag = "false";
                    batchRunControlRow.Payload = null;

                    batchRunControls.Add(batchRunControlRow);

                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Error processing ${ex.ToString()}.");
                    throw;
                }

                startDate = endDate.AddMilliseconds(1);
                endDate = startDate.AddDays(7);
                endDate = endDate.AddMilliseconds(-1);

            }

            if (batchRunControls.Count > 0)
                await _odsRepository.AddMultipleAsync(batchRunControls);

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database");

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database success.");

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{batchRunControl.Id}]  [Table:{batchRunControl.TableName}] Ended");

            return batchRunControls;

        }

        private async Task<List<BatchRunControl>> ExtractODSTableIntervals(BatchRunControl batchRunControl, ODSData odsData)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Manager - Extract ODS Table Intervals");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{batchRunControl.Id}]  [Table:{batchRunControl.TableName}] Started");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            // Create a policy for retry with exponential back-off
            var retryPolicy = Policy
                .Handle<HttpRequestException>() // Specify the exception to handle
                .WaitAndRetryAsync(
                    10, // Retry 5 times
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // Exponential back-off
                );

            // Create a policy for the circuit breaker with a threshold of 3 failures
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromMinutes(1) // Break for 1 minute on failure
                );

            DateTime startDate = odsData.StartDate;
            DateTime currentEndDate = DateTime.Now;

            DateTime endDate = odsData.StartDate.AddDays(365);
            endDate = new DateTime(endDate.Year, endDate.Month, 1);

            // Subtract one day to get the last day of the current month
            endDate = endDate.AddMilliseconds(-1);

            List<IntervalDataItem> tableIntervals = new List<IntervalDataItem>();

            while (startDate <= currentEndDate)
            {
                string formattedStartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                string formattedEndDate = endDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

                string responseDataFileName = Path.Combine(odsData.FilePath, $"{odsData.TableName}_{formattedStartDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_TO_{formattedEndDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}");
                string responseDataFileNameWithExtensionRAW = responseDataFileName + "_RAW.json";
                string responseDataFileNameWithExtension = responseDataFileName + ".json";
                string url = $"{odsData.URL}?StartDate={formattedStartDate}&EndDate={formattedEndDate}&TableName={odsData.TableName}";

                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Fetching data for StartDate: {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName}");

                        // Call the Medidata Rave service
                        var response = await _medidateRWSService.GetAndWriteToDiskWithResponse(odsData.TableName, url, responseDataFileNameWithExtensionRAW);

                        // Check for a failed response
                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Fetching data for StartDate: {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName} -  HTTP request failed. [Error Exception : {response?.ErrorException}] [Content: {response?.Content}]");
                            throw new HttpRequestException($"TraceId:{_appSettings.TraceId}; HTTP request failed. [Error Exception : {response?.ErrorException}] [Content: {response?.Content}] ");
                        }

                        var json = response.Content;

                        var isSaveSuccess = await SaveData(json, odsData.TableName, Path.GetFileName(responseDataFileNameWithExtensionRAW));

                        // Check for a failed response
                        if (!isSaveSuccess)
                        {
                            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Fetching data for StartDate: {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName} -  Unable to save to S3 - {responseDataFileNameWithExtensionRAW}");
                            throw new HttpRequestException($"TraceId:{_appSettings.TraceId}; HTTP request failed. [Error Exception : {response?.ErrorException}] [Content: {response?.Content}] ");
                        }

                        json = json.Replace("\"Data\":\"[{\"", "\"Data\":[{\"");
                        json = json.Replace("}]\"}", "}]}");

                        isSaveSuccess = await SaveData(json, odsData.TableName, Path.GetFileName(responseDataFileNameWithExtension));

                        // Check for a failed response
                        if (!isSaveSuccess)
                        {
                            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Fetching data for StartDate: {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName} -  Unable to save to S3 - {responseDataFileNameWithExtension}");
                            throw new HttpRequestException($"TraceId:{_appSettings.TraceId}; HTTP request failed. [Error Exception : {response?.ErrorException}] [Content: {response?.Content}] ");
                        }


                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; StartDate: {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName} - Parsing started for Intervals ");
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
                                _logger.LogInformation($"TraceId:{_appSettings.TraceId};  StartDate: {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName} - One or more required properties are missing.");
                                // Handle the case where the required properties are missing
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; StartDate: {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName} - An error occurred: {ex.Message}");
                            // Handle the exception here
                        }

                        // Check if 'data' is null (indicating an issue with JSON parsing)
                        if (data == null)
                        {
                            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; StartDate:  {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName}  - Failed to parse JSON data.");
                            data = new JArray();
                            // Handle the case where JSON parsing failed
                        }

                        // Check if the "Data" property is an empty array
                        if (data.Count == 0)
                        {
                            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; StartDate:  {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName} -  DATA IS AN EMPTY ARRAY");
                            return;
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
                            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; StartDate: {formattedStartDate}, EndDate: {formattedEndDate}, TableName: {odsData.TableName} -  Unable to Serilize");
                        }
                    });
                }
                catch (HttpRequestException ex)
                {
                    // Handle the exception
                    _logger.LogError($"TraceId:{_appSettings.TraceId}; Critical Error Occurred. {ex}");
                    batchRunControl.ErrorMessage = ex.StackTrace;
                    batchRunControl.Success = "FAILED";
                }
                catch (BrokenCircuitException ex)
                {
                    // The circuit breaker is open, handle accordingly
                    _logger.LogError($"TraceId:{_appSettings.TraceId}; Circuit breaker is open. {ex}");
                }


                startDate = endDate.AddMilliseconds(1);
                endDate = startDate.AddDays(365);
                endDate = new DateTime(endDate.Year, endDate.Month, 1);

                // Subtract one day to get the last day of the current month
                endDate = endDate.AddMilliseconds(-1);


            }

            try
            {
                var CONFIG_RWSUSERNAME = "SPEC_TRACK_RWS";
                var CONFIG_RWSPASSWORD = "Password@01";

                List<BatchRunControl> records = new List<BatchRunControl>();

                tableIntervals = tableIntervals.OrderBy(i => i.Start).ToList();

                foreach (var item in tableIntervals)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Started:{item.Bucket}  Total Count:{tableIntervals.Count} ");
                    try
                    {
                        var batchRunControlRow = new BatchRunControl();

                        batchRunControlRow.TableName = odsData.TableName;
                        batchRunControlRow.ApiStartDate = item.BucketStart.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                        batchRunControlRow.ApiEndDate = item.BucketEnd.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                        batchRunControlRow.JobStartTime = null;
                        batchRunControlRow.JobEndTime = null;
                        batchRunControlRow.Slot = item.Bucket;
                        batchRunControlRow.RaveUsername = CONFIG_RWSUSERNAME;
                        batchRunControlRow.RavePassword = CONFIG_RWSPASSWORD;
                        batchRunControlRow.NoOfRecords = item.Records;
                        batchRunControlRow.IsRunCompleteFlag = "false";
                        batchRunControlRow.Payload = null;
                        records.Add(batchRunControlRow);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ERROR {ex.StackTrace}");
                        throw;
                    }
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Finished :{item.Bucket}  Total Count:{tableIntervals.Count} ");
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

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            return batchRunControls;

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{batchRunControl.Id}]  [Table:{batchRunControl.TableName}] Ended");

        }

        private async Task<bool> SaveData(string response, string tableName, string fileName, string bucketName = "ods-table-data")
        {
            var path = string.Format($"odsmanager/interval/{tableName}/{fileName}");

            if (string.IsNullOrEmpty(_appSettings.ArchiveBucket))
            {
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; SaveData failed as S3 Bucket was not configured; Path: {path}");
                return false;
            }

            var isSuccess = await _awsCoreHelper.UploadDataAsync(_appSettings.ArchiveBucket, path, response);

            if (!isSuccess)
            {
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; SaveData failed; Path: {path}");

                return false;
            }
            else
            {
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; SaveData success; Path: {path}");

                return true;
            }
        }

    }
}