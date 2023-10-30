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
                string basePath = @"C:\Path\To\ResponseFiles\TablesData\";

                // Format the current date and time as a string (e.g., "yyyyMMdd_HHmmss")
                string folderName = folderCurrentDateTime.ToString("yyyyMMdd_HHmmss");
                // Combine the base path and folder name to create the full path
                string fullPath = Path.Combine(basePath, folderName);
                string baseUrl = "/RaveWebServices/datasets/ThxExtracts2.json";


                ODSData odsData = new ODSData();
                odsData.TableName = exInput.TableName;
                odsData.URL = baseUrl;
                odsData.FilePath = Path.Combine(fullPath, odsData.TableName);

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
                    if (brcNext == null)
                    {
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; Current batch not found!!!");
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                        bexecute = false;
                        break;
                    }

                    var isSuccess = await Extract(brcNext, odsData);

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
        private async Task<bool> Extract(BatchRunControl batchRunControl, ODSData odsData)
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
                    5, // Retry 5 times
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // Exponential back-off
                );

            // Create a policy for the circuit breaker with a threshold of 3 failures
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromMinutes(1) // Break for 1 minute on failure
                );

            var HasActiveJobs = await _odsRepository.HasActiveJobsAsync(batchRunControl.TableName.ToUpper());

            if (HasActiveJobs.Count > 0)
            {
                BatchRunControl brc = HasActiveJobs[0];
                _logger.LogWarning($"TraceId:{_appSettings.TraceId}; [Id:{brc.Id}]" + $"[Slot:{brc.Slot}] " + $"[NoOfRecords:{brc.NoOfRecords}] " + $"[IsRunCompleteFlag:{brc.IsRunCompleteFlag}] ");
                _logger.LogWarning($"TraceId:{_appSettings.TraceId}; There is allready active job running.");
                _logger.LogWarning($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                return false;
            }

            await _odsRepository.StartedAsync(batchRunControl);

            if (!Directory.Exists(odsData.FilePath))
            {
                Directory.CreateDirectory(odsData.FilePath);
            }

            var brcNext = await _odsRepository.GetByTableNameAndIdAsync(odsData.TableName.ToUpper(), batchRunControl.Id);

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

            string responseDataFileName = Path.Combine(odsData.FilePath, $"{odsData.TableName}_{formattedStartDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_TO_{formattedEndDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}");


            string responseDataFileNameWithExtensionRAW = responseDataFileName + "_RAW.json";
            string responseDataFileNameWithExtension = responseDataFileName + ".json";
            string error_responseDataFileNameWithExtensionRAW = responseDataFileName + "ERROR_RAW.json";
            string error_responseDataFileNameWithExtension = responseDataFileName + "ERROR.json";


            try
            {
                // Wrap your code in a policy using ExecuteAsync
                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Inside this block, Polly will handle retries and exponential back-off
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; STARTED - Calling Medidata Rave to fetch All data for [Table:{odsData.TableName}] " +
                                            $"[StartDate:{batchRunControl.ApiStartDate}]" + $"[EndDate:{batchRunControl.ApiEndDate}] " + $"[URL:{odsData.URL}] ");


                    string url = $"{odsData.URL}?PageSize=200000000&StartDate={formattedStartDate}&EndDate={formattedEndDate}&TableName={odsData.TableName}";


                    // Call the Medidata Rave service
                    var response = await _medidateRWSService.GetAndWriteToDiskWithResponse(odsData.TableName, url, responseDataFileNameWithExtensionRAW);

                    // Check for a failed response
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new HttpRequestException($"HTTP request failed. [Error Exception : {response.ErrorException.ToString()}] [Content: {response.Content}] ");
                    }

                    var json = response.Content;

                    batchRunControl.Success = "SUCCESS";
                    batchRunControl.HttpStatusCode = response.StatusCode.ToString();
                    batchRunControl.RaveDataUrl = url;
                    batchRunControl.ExtractedFileName = responseDataFileNameWithExtension;

                    // Deserialize JSON into a C# object
                    Payloads payload = JsonConvert.DeserializeObject<Payloads>(json);

                    // Remove the "Data" property from the object
                    foreach (var item in payload.Payload)
                    {
                        item.Data = null;
                    }

                    // Serialize the modified object back to JSON
                    batchRunControl.Payload = JsonConvert.SerializeObject(payload);
                    batchRunControl.Payloads = payload;

                    // File.WriteAllText(responseDataFileNameWithExtensionRAW, data);
                    await File.WriteAllTextAsync(responseDataFileNameWithExtension, json);

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; STARTED - Calling Save to Postgres database  [Table:{odsData.TableName}] " +
                                            $"[StartDate:{odsData.StartDate}]" + $"[EndDate:{odsData.EndDate}] " + $"[URL:{odsData.URL}] ");

                    await _odsRepository.CompletedAsync(batchRunControl);
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; COMPLETED - Calling Save to Postgres database  [Table:{odsData.TableName}] " +
                                            $"[StartDate:{odsData.StartDate}]" + $"[EndDate:{odsData.EndDate}] " + $"[URL:{odsData.URL}] ");
                });
            }
            catch (HttpRequestException ex)
            {
                // Handle the exception
                File.WriteAllText(error_responseDataFileNameWithExtensionRAW, ex.Message);
                File.WriteAllText(error_responseDataFileNameWithExtension, ex.Message);
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Critical Error Occurred. {ex}");
                batchRunControl.ExtractedFileName = error_responseDataFileNameWithExtension;
                batchRunControl.ErrorMessage= ex.StackTrace;
                await _odsRepository.CompletedErrorAsync(batchRunControl, ex.StackTrace);
            }
            catch (BrokenCircuitException ex)
            {
                // The circuit breaker is open, handle accordingly
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Circuit breaker is open. {ex}");
            }

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            return true;
        }
    }
}