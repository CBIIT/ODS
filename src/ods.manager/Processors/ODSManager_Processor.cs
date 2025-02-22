using Amazon.S3;
using Extension.Methods;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.CircuitBreaker;
using System.Net;
using Theradex.ODS.Manager.Helpers.Extensions;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Manager.Models;
using Theradex.ODS.Manager.Models.Configuration;
using Theradex.ODS.Models;

namespace Theradex.ODS.Manager.Processors
{
    public class ODSManager_Processor : BaseProcessor, IProcessor
    {
        static List<string> ALLODSTABLES = new List<string>
                            {
                                "DATAPOINTROLESTATUS",
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
                                "UPLOADDATAPOINTS",
                                "UNITDICTIONARIES",
                                "UNITDICTIONARYENTRIES"
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

        public async Task<bool> ProcessAsync(ManagerInput exInput)
        {
            try
            {
                //await GetTableIntervalsDetailInfoViaThxExtracts(exInput.TableName);
                //await GetTableIntervalsDetailInfoViaThxExtracts("DATAPOINTS");
                //await GetODSTableIntervalIncrementalDate(exInput.TableName);
                await GetTableIntervalsDetailInfoViaThxExtracts(exInput);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task GetTableIntervalsDetailInfoViaThxExtracts(ManagerInput exInput)
        {
            string tableName = "NONE";
            if(exInput.TableName !=string.Empty) tableName = exInput.TableName;
            
            
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            var odsmanager_table_matadatas = await _odsManagerRepository.GetAllAsync();


            string baseUrl = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";

            if (tableName != "NONE")
                odsmanager_table_matadatas = odsmanager_table_matadatas.Where(i => i.TableName.ToUpper() == tableName).ToList();


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

                    odsmanager_table_matadata.RaveDataUrl = exInput.RaveDataUrl != string.Empty ? exInput.RaveDataUrl : "/RaveWebServices/datasets/ThxExtracts2.json";

                    if (odsmanager_table_matadata.Payloads.Payload != null && odsmanager_table_matadata.Payloads.Payload.Count == 1)
                    {
                        var isMinDateValid = DateTime.TryParse(odsmanager_table_matadata.Payloads.Payload.First().MinDate, out minDate);
                        if (!isMinDateValid)
                        {
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Min date not valid.");

                            minDate = new DateTime(2000, 01, 01);
                            odsmanager_table_matadata.RaveDataUrl = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Defaulting to 01/01/2000");
                        }

                    }

                    var last = await _odsRepository.GetByMaxApiEndDateAsync(odsmanager_table_matadata.TableName);

                    if (last != null)
                    {
                        var isMinDateValid = DateTime.TryParse(last.ApiEndDate, out minDate);
                        if (!isMinDateValid)
                        {
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Min date not valid.");

                            minDate = new DateTime(2000, 01, 01);
                            odsmanager_table_matadata.RaveDataUrl = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Defaulting to 01/01/2000");
                        }
                    }

                    //minDate = DateTime.Now.Date;
                    ODSData oDSData = new ODSData();
                    oDSData.TableName = odsmanager_table_matadata.TableName;
                    oDSData.StartDate = minDate;
                    oDSData.URL = baseUrl;

                    //var batchRunControl = await GeneratODSTableIntervals(odsmanager_table_matadata, oDSData);
                    var batchRunControl = await ExtractODSTableIntervals(odsmanager_table_matadata, oDSData);

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{odsmanager_table_matadata.Id}]  [Table:{odsmanager_table_matadata.TableName}] Ended");
                }
            }
        }
        private async Task<List<BatchRunControl>> ExtractODSTableIntervals(BatchRunControl batchRunControl, ODSData odsData)
        {
            var baseURL = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";  //"?StartDate=2023-01-01&EndDate=2023-12-31&TableName=DATAPOINTS&Interval_In_Seconds=86400"
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

            DateTime currentEndDate = DateTime.Now;

            DateTime startDate = odsData.StartDate;
            DateTime endDate = odsData.StartDate.AddMonths(6);
            endDate = new DateTime(endDate.Year, endDate.Month, 1);

            // Subtract one day to get the last day of the current month
            endDate = endDate.AddMilliseconds(-1);

            List<IntervalDataItem> tableIntervals = new List<IntervalDataItem>();

            while (startDate.Date <= currentEndDate.Date)
            {
                string formattedStartDate = startDate.ToString("yyyy-MM-dd");
                string formattedEndDate = endDate.ToString("yyyy-MM-dd");

                string responseDataFileName = Path.Combine(odsData.FilePath ?? "", $"{odsData.TableName}_{formattedStartDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_TO_{formattedEndDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}");
                string responseDataFileNameWithExtensionRAW = responseDataFileName + "_RAW.json";
                string responseDataFileNameWithExtension = responseDataFileName + ".json";
                string url = $"{baseURL}?StartDate={formattedStartDate}&EndDate={formattedEndDate}&TableName={odsData.TableName}&Interval_In_Seconds=86400";

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
                endDate = startDate.AddMonths(6);
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
                        batchRunControlRow.ApiStartDate = item.Start.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                        batchRunControlRow.ApiEndDate = item.Start.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                        batchRunControlRow.JobStartTime = null;
                        batchRunControlRow.JobEndTime = null;
                        batchRunControlRow.Slot = item.Bucket;
                        batchRunControlRow.RaveUsername = CONFIG_RWSUSERNAME;
                        batchRunControlRow.RavePassword = CONFIG_RWSPASSWORD;
                        batchRunControlRow.NoOfRecords = item.Records;
                        batchRunControlRow.IsRunCompleteFlag = "false";
                        batchRunControlRow.Payload = null;
                        batchRunControlRow.RaveDataUrl = "/RaveWebServices/datasets/ThxExtracts2.json";
                        batchRunControlRow.UrlToPullData = "/RaveWebServices/datasets/ThxExtracts2.json";

                        if (item.Start.Date == DateTime.Now.Date) continue;

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

                await _odsRepository.AddMultipleAsync(records);

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

        #region IncrementalProcessing

        private async Task<List<BatchRunControl>> ResumeAndGenerateODSTableIntervals(BatchRunControl batchRunControl, ODSData odsData)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Manager - Extract ODS Table Intervals");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{batchRunControl.Id}]  [Table:{batchRunControl.TableName}] Started");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            DateTime startDate = odsData.StartDate;

            DateTime endDate = startDate.AddDays(1);
            DateTime currentEndDate = DateTime.Now;

            // Subtract one day to get the last day of the current month
            endDate = endDate.AddMilliseconds(-1);

            while (startDate.Date < currentEndDate.Date)
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
                    batchRunControlRow.ApiStartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                    batchRunControlRow.ApiEndDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
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
                endDate = startDate.AddDays(1);
                endDate = endDate.AddMilliseconds(-1);

            }

            if (batchRunControls.Count > 0)
            {
                await _odsRepository.AddMultipleAsync(batchRunControls);
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; - Saving to database success.");
            }
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{batchRunControl.Id}]  [Table:{batchRunControl.TableName}] Ended");

            return batchRunControls;

        }

        private async Task GetODSTableIntervalIncrementalDate(string tableName = "NONE")
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            var odsmanager_table_matadatas = await _odsManagerRepository.GetAllAsync();


            string baseUrl = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";

            if (tableName != "NONE")
                odsmanager_table_matadatas = odsmanager_table_matadatas.Where(i => i.TableName.ToUpper() == tableName).ToList();

            if (odsmanager_table_matadatas.Any())
            {
                foreach (var odsmanager_table_matadata in odsmanager_table_matadatas)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{odsmanager_table_matadata.Id}]  [Table:{odsmanager_table_matadata.TableName}] Started");

                    //if (odsmanager_table_matadata.TableName == "LOCALIZEDDATASTRINGS" || odsmanager_table_matadata.TableName == "FORMRESTRICTIONS" || odsmanager_table_matadata.TableName == "FIELDRESTRICTIONS")
                    //{
                    //    _logger.LogWarning($"TraceId:{_appSettings.TraceId}; [Id :{odsmanager_table_matadata.Id}]  [Table:{odsmanager_table_matadata.TableName}] We have issue in extracting so skiping this.");
                    //    continue;
                    //}

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

                            minDate = new DateTime(2000, 01, 01);
                            odsmanager_table_matadata.RaveDataUrl = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Defaulting to 01/01/2000");
                        }

                    }

                    var last = await _odsRepository.GetByMaxApiEndDateAsync(odsmanager_table_matadata.TableName);

                    if (last != null)
                    {
                        var isMinDateValid = DateTime.TryParse(last.ApiEndDate, out minDate);
                        if (!isMinDateValid)
                        {
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Min date not valid.");

                            minDate = new DateTime(2000, 01, 01);
                            odsmanager_table_matadata.RaveDataUrl = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";
                            _logger.LogError($"TraceId:{_appSettings.TraceId}; [Id: {odsmanager_table_matadata.Id}] [Table:{odsmanager_table_matadata.TableName}] - Defaulting to 01/01/2000");
                        }
                    }

                    ODSData oDSData = new ODSData();
                    oDSData.TableName = odsmanager_table_matadata.TableName;
                    oDSData.StartDate = minDate;
                    oDSData.URL = baseUrl;

                    if (oDSData.StartDate.Date >= DateTime.Now.Date) continue;

                    var batchRunControl = await ResumeAndGenerateODSTableIntervals(odsmanager_table_matadata, oDSData);
                    //var batchRunControl = await ExtractODSTableIntervals(odsmanager_table_matadata, oDSData);

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; [Id :{odsmanager_table_matadata.Id}]  [Table:{odsmanager_table_matadata.TableName}] Ended");
                }
            }
        }

        #endregion

        #region Load from File System

        private async Task<bool> LoadAllDataFromFileSystem()
        {
            string rootDirectory = @"C:\ODS\Manager\Data\dev\Manager\Data\"; // Replace with the root directory path
            foreach (string table in ALLODSTABLES)
            {
                if (Directory.Exists(rootDirectory + table))
                {
                    await LoadSingle(rootDirectory, table);
                }

            }
            return true;
        }

        private async Task<bool> LoadSingle(string basePath, string tableName)
        {
            string searchPattern = "*.json"; // Specify the file name pattern

            List<string> jsonFiles = new List<string>();
            // Search for JSON files in the root directory and subdirectories
            foreach (string filePath in Directory.GetFiles(basePath + tableName, searchPattern, SearchOption.AllDirectories))
            {
                jsonFiles.Add(filePath);
            }

            var tablejsons = jsonFiles;
            List<IntervalDataItem> tableIntervals = new List<IntervalDataItem>();
            foreach (string file in tablejsons)
            {
                string json = File.ReadAllText(file);
                json = json.Replace("\"Data\":, \"JsonDataChecksum\":", "\"Data\":[], \"JsonDataChecksum\":");
                json = json.Replace(" \"Data\":\"[{", " \"Data\":[{");
                json = json.Replace("]\"}      ]}", "]}      ]}");

                if (json == "  {\"Payload\":[ { \"Data\":\"[]}      ]}  ")
                    continue;

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

                        batchRunControlRow.TableName = tableName.ToUpper().Trim();
                        batchRunControlRow.ApiStartDate = data.Start.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                        batchRunControlRow.ApiEndDate = data.Start.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                        batchRunControlRow.JobStartTime = null;
                        batchRunControlRow.JobEndTime = null;
                        batchRunControlRow.Slot = data.Bucket;
                        batchRunControlRow.RaveUsername = CONFIG_RWSUSERNAME;
                        batchRunControlRow.RavePassword = CONFIG_RWSPASSWORD;
                        batchRunControlRow.NoOfRecords = data.Records;
                        batchRunControlRow.IsRunCompleteFlag = "false";
                        batchRunControlRow.Payload = null;
                        batchRunControlRow.RaveDataUrl = "/RaveWebServices/datasets/ThxExtracts2.json";
                        batchRunControlRow.UrlToPullData = "/RaveWebServices/datasets/ThxExtracts2.json";
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
        #endregion

        private async Task<bool> SaveData(string response, string tableName, string fileName)
        {
            if (_appSettings.ArchiveBucket.NotNullAndNotEmpty())
            {
                var key = $"{_appSettings.Env}/Manager/Data/{tableName}/{fileName}";

                var isSuccess = await _awsCoreHelper.UploadDataAsync(_appSettings.ArchiveBucket, key, response);

                if (!isSuccess)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; SaveData failed; Path: {key}");
                }
                else
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; SaveData success; Path: {key}");
                }
            }

            if (_appSettings.LocalArchivePath.NotNullAndNotEmpty())
            {
                var basePath = Path.Combine(_appSettings.LocalArchivePath, "odsmanager", tableName);

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                var path = Path.Combine(basePath, fileName);

                await File.WriteAllTextAsync(path, response);
            }

            if (!_appSettings.ArchiveBucket.NotNullAndNotEmpty() && !_appSettings.LocalArchivePath.NotNullAndNotEmpty())
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; No ArchiveBucket or LocalArchivePath configured;");

                return false;
            }

            return true;
        }

    }
}