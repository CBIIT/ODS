using Amazon.S3;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Theradex.ODS.Extractor.Helpers;
using Theradex.ODS.Extractor.Helpers.Extensions;
using Theradex.ODS.Extractor.Interfaces;
using Theradex.ODS.Extractor.Models;
using Theradex.ODS.Extractor.Models.Configuration;
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

namespace Theradex.ODS.Extractor.Processors
{
    public class ODSExtractor_Processor : BaseProcessor, IProcessor
    {
        const int MaxPageData = 50000;

        public ODSExtractor_Processor(
            IMedidataRWSService medidateRWSService,
            ILogger<ODSExtractor_Processor> logger,
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
                // Get the current date and time
                DateTime folderCurrentDateTime = DateTime.Now;

                // Define the base path where you want to create the folder
                string basePath = @"C:\ODS\Extractor\Data\";

                // Format the current date and time as a string (e.g., "yyyyMMdd_HHmmss")
                string folderName = folderCurrentDateTime.ToString("yyyyMMdd_HHmmss");
                // Combine the base path and folder name to create the full path
                //string fullPath = Path.Combine(basePath, folderName);
                string fullPath = Path.Combine(basePath);
                string baseUrl = "/RaveWebServices/datasets/ThxExtracts2.json";


                ODSData odsData = new ODSData();
                odsData.TableName = exInput.TableName;
                odsData.URL = baseUrl;
                odsData.FilePath = Path.Combine(fullPath, odsData.TableName);
                odsData.FilePath = Path.Combine(fullPath, odsData.TableName);
                odsData.RecordCount = exInput.Count;

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Extraction");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; {odsData.TableName}");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; {odsData.URL}");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                bool bexecute = true;

                while (bexecute)
                {

                    var HasActiveJobs = await _odsRepository.HasActiveJobsAsync(odsData.TableName.ToUpper());

                    if (HasActiveJobs.Count > 0)
                    {
                        BatchRunControl brc = HasActiveJobs.First();
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; [Id:{brc.Id}]" + $"[Slot:{brc.Slot}] " + $"[NoOfRecords:{brc.NoOfRecords}] " + $"[IsRunCompleteFlag:{brc.IsRunCompleteFlag}] ");
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; There is allready active job running.");
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                        bexecute = false;
                        break;
                    }

                    var brcNext = await _odsRepository.GetNextBatchAsync(odsData.TableName.ToUpper());
                    //brcNext = await _odsRepository.GetByTableNameAndIdAsync(odsData.TableName.ToUpper(), 7977);

                    if (brcNext == null)
                    {
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; Current batch not found!!!");
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                        bexecute = false;
                        break;
                    }

                    var isSuccess = await ExtractSingleBatch(brcNext, odsData);
                    //var isSuccess = await Extract(brcNext, odsData);

                    bexecute = true;
                }

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Error - {ex}");
                return false;
            }
        }
        private async Task<BatchRunControl> ExtractSingleBatch(BatchRunControl batchRunControl, ODSData odsData)
        {
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Extraction");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Current TableName : " + batchRunControl.TableName);
            _logger.LogInformation($"TraceId:{_appSettings.TraceId};        Current Id : " + batchRunControl.Id.ToString());
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Current StartDate : " + batchRunControl.ApiEndDate);
            _logger.LogInformation($"TraceId:{_appSettings.TraceId};   Current Enddate : " + batchRunControl.ApiEndDate);
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

            if (!Directory.Exists(odsData.FilePath))
            {
                Directory.CreateDirectory(odsData.FilePath);
            }
            var brcNext = await _odsRepository.GetByTableNameAndIdAsync(odsData.TableName.ToUpper(), batchRunControl.Id);
            //var brcNext = await _odsRepository.GetByTableNameAndIdAsync(odsData.TableName.ToUpper(), 7978);

            if (brcNext == null)
            {
                _logger.LogWarning($"TraceId:{_appSettings.TraceId}; Next batch not found!!!");
                _logger.LogWarning($"TraceId:{_appSettings.TraceId}; Defaulting to Current system date!!!");
                _logger.LogWarning($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                batchRunControl.ApiEndDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
            }
            else
            {
                _logger.LogWarning($"TraceId:{_appSettings.TraceId}; Next batch found!!!");
                _logger.LogWarning($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                DateTime tdtEndDate = DateTime.Parse(brcNext.ApiStartDate).AddDays(-1);
                batchRunControl.ApiEndDate = tdtEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Next TableName : " + brcNext.TableName);
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Next Id : " + brcNext.Id.ToString());
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Next StartDate : " + brcNext.ApiEndDate);
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Next Enddate : " + brcNext.ApiEndDate);
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            }

            //await _odsRepository.StartedAsync(batchRunControl);

            DateTime dtStartDate = DateTime.Parse(batchRunControl.ApiStartDate);
            DateTime dtEndDate = DateTime.Parse(batchRunControl.ApiEndDate);

            TimeSpan dateDifference = dtEndDate - dtStartDate;

            // Check if the date difference is more than 365 days
            if (dateDifference.TotalDays > 365)
            {
                // Log an info message indicating the date difference is being limited
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Date difference is more than 365 days. Limiting to 365 days.");

                // Limit the date difference to 365 days
                dtEndDate = dtStartDate.AddDays(365);
            }


            string formattedStartDate = dtStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
            string formattedEndDate = dtEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");


            string responseDataFileName = Path.Combine(odsData.FilePath, $"{odsData.TableName}_{batchRunControl.Id}_{formattedStartDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_TO_{formattedEndDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_#PAGENUMBER#");

            int pageSize = odsData.RecordCount; // Set your desired page size
            var totalPages = 10000;
            int pageNumber = 1; // Initialize pageNumber

            Payloads payloads = new Payloads();
            payloads.Payload = new List<PayloadItem>();

            while (pageNumber <= totalPages)
            {
                string responseDataFileNameWithExtensionRAW = responseDataFileName.Replace("#PAGENUMBER#", pageNumber.ToString()) + "_RAW.json";
                string responseDataFileNameWithExtension = responseDataFileName.Replace("#PAGENUMBER#", pageNumber.ToString()) + ".json";
                string error_responseDataFileNameWithExtensionRAW = responseDataFileName.Replace("#PAGENUMBER#", pageNumber.ToString()) + "ERROR_RAW.json";
                string error_responseDataFileNameWithExtension = responseDataFileName.Replace("#PAGENUMBER#", pageNumber.ToString()) + "ERROR.json";

                try
                {
                    // Inside this block, Polly will handle retries and exponential back-off
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; STARTED - Calling Medidata Rave to fetch data for [Table:{odsData.TableName}] " +
                                            $"[StartDate:{batchRunControl.ApiStartDate}]" + $"[EndDate:{batchRunControl.ApiEndDate}] " + $"[URL:{odsData.URL}] " +
                                            $"[PageSize:{pageSize}] [PageNumber:{pageNumber}]");

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
                        batchRunControl.HttpStatusCode = response.StatusCode.ToString();
                        batchRunControl.RaveDataUrl = url;
                        batchRunControl.Success = response.StatusCode.ToString();
                        batchRunControl.ExtractedFileName = responseDataFileNameWithExtension;

                        json = json.Replace("\"Data\":, \"JsonDataChecksum\":", "\"Data\":[], \"JsonDataChecksum\":");

                        // Deserialize JSON into a C# object
                        Payloads payload = JsonConvert.DeserializeObject<Payloads>(json);

                        batchRunControl.Payload = json;
                        batchRunControl.Payloads = payload;

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

                                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; STARTED - Calling Save to Postgres database  [Table:{odsData.TableName}] " +
                                                        $"[StartDate:{odsData.StartDate}]" + $"[EndDate:{odsData.EndDate}] " + $"[URL:{odsData.URL}] " + $"[Query Counts :{payloadReceived.QueryCountofRows}] ");

                                await _odsRepository.UpdateAsync(batchRunControl);

                                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; COMPLETED - Calling Save to Postgres database  [Table:{odsData.TableName}] " +
                                                        $"[StartDate:{odsData.StartDate}]" + $"[EndDate:{odsData.EndDate}] " + $"[URL:{odsData.URL}] ");
                            }
                        }
                    });

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; STARTED - Calling Save to Postgres database  [Table:{odsData.TableName}] " +
                                               $"[StartDate:{odsData.StartDate}]" + $"[EndDate:{odsData.EndDate}] " + $"[URL:{odsData.URL}] ");

                    if (batchRunControl.Payloads != null)
                        payloads.Payload.AddRange(batchRunControl.Payloads.Payload);
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
                    await _odsRepository.CompletedErrorAsync(batchRunControl, ex.StackTrace);
                }
                catch (BrokenCircuitException ex)
                {
                    // The circuit breaker is open, handle accordingly
                    _logger.LogError($"TraceId:{_appSettings.TraceId}; Circuit breaker is open. {ex}");
                }

                pageNumber++; // Increment pageNumber for the next iteration
            }

            batchRunControl.Payload = JsonConvert.SerializeObject(payloads);

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            await _odsRepository.CompletedAsync(batchRunControl);
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; COMPLETED - Calling Save to Postgres database  [Table:{odsData.TableName}] " +
                                   $"[StartDate:{odsData.StartDate}]" + $"[EndDate:{odsData.EndDate}] " + $"[URL:{odsData.URL}] ");


            return batchRunControl;


        }

    }
}