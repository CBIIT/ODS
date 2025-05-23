﻿using Amazon.S3;
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
using System.Collections.Specialized;

namespace Theradex.ODS.Extractor.Processors
{
    public class ODSExtractor_IncrementalProcessor : BaseProcessor, IProcessor
    {
        public ODSExtractor_IncrementalProcessor(
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
                DateTime folderCurrentDateTime = DateTime.Now;
                ODSData odsData = new ODSData();
                odsData.TableName = exInput.TableName;
                odsData.NoOfRecords= exInput.NoOfRecords;

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Extraction");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; {odsData.TableName}");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; {odsData.URL}");
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                
                var execute = true;
                var count = odsData.NoOfRecords == 0 ? 1 : odsData.NoOfRecords;
                
                while (execute && count <=1)
                {

                    var HasActiveJobs = await _odsRepository.HasActiveJobsAsync(odsData.TableName.ToUpper());

                    if (HasActiveJobs.Count > 0)
                    {
                        BatchRunControl brc = HasActiveJobs.First();
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; [Id:{brc.Id}]" + $"[Slot:{brc.Slot}] " + $"[NoOfRecords:{brc.NoOfRecords}] " + $"[IsRunCompleteFlag:{brc.IsRunCompleteFlag}] ");
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; There is allready active job running.");
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                        execute = false;
                        break;
                    }

                    var brcNext = await _odsRepository.GetNextBatchAsync(odsData.TableName.ToUpper());
                    
                    //var brcNext = await _odsRepository.GetByIdAsync(314493);

                    if (brcNext == null)
                    {
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; Current batch not found!!!");
                        _logger.LogWarning($"TraceId:{_appSettings.TraceId}; -------------------------------------");
                        execute = false;
                        break;
                    }

                    ///RaveWebServices/datasets/ThxExtracts2.json?PageSize=50000&PageNumber=1&StartDate=2018-02-17T00:00:00.0000000&EndDate=2018-02-23T00:00:00.0000000&TableName=CONFIGURATION
                    string[] parts = brcNext.RaveDataUrl.Split('?');
                    string baseUrl = parts[0]; // This will contain "RaveWebServices/datasets/ThxExtracts2.json"

                    odsData.URL = baseUrl;

                    var isSuccess = await ExtractSingleBatch(brcNext, odsData);
                    
                    execute = true;
                    count++;
                }

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Error - {ex}");
                return false;
            }
        }

        private async Task<BatchRunControl> ExtractSingleBatchOLD(BatchRunControl batchRunControl, ODSData odsData)
        {
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Extraction - ExtractSingleBatch ");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Current TableName : " + batchRunControl.TableName);
            _logger.LogInformation($"TraceId:{_appSettings.TraceId};        Current Id : " + batchRunControl.Id.ToString());
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Current StartDate : " + batchRunControl.ApiStartDate);
            _logger.LogInformation($"TraceId:{_appSettings.TraceId};   Current Enddate : " + batchRunControl.ApiEndDate);
            _logger.LogInformation($"TraceId:{_appSettings.TraceId};   RaveDataUrl : " + batchRunControl.RaveDataUrl);
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; -------------------------------------");

            if (StringFunctions.IsNullOrEmpty(batchRunControl.RaveDataUrl))
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; RaveDataUrl is empty; Aborting.");
                return batchRunControl;
            }

            //await _odsRepository.StartedAsync(batchRunControl);

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


            DateTime dtStartDate = DateTime.Parse(batchRunControl.ApiStartDate);
            DateTime dtEndDate = DateTime.Parse(batchRunControl.ApiEndDate);

            TimeSpan dateDifference = dtEndDate - dtStartDate;

            string formattedStartDate = dtStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
            string formattedEndDate = dtEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

            string responseDataFileName = $"{odsData.TableName}_{batchRunControl.Id}_{formattedStartDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_TO_{formattedEndDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_#PAGENUMBER#";

            int pageSize = 50000; //odsData.RecordCount; // Set your desired page size

            var totalPages = 10000;
            int pageNumber = 1; // Initialize pageNumber

            Payloads payloads = new Payloads();
            payloads.Payload = new List<PayloadItem>();

            while (pageNumber <= totalPages)
            {
                string responseDataFileNameWithExtensionRAW = responseDataFileName.Replace("#PAGENUMBER#", pageNumber.ToString()) + "_RAW.json";
                string responseDataFileNameWithExtension = responseDataFileName.Replace("#PAGENUMBER#", pageNumber.ToString()) + ".json";

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

                                await SaveData(json, odsData.TableName, responseDataFileNameWithExtension, false);

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
                    await SaveData(ex.ToString(), batchRunControl.TableName, responseDataFileNameWithExtensionRAW, true);
                    await SaveData(ex.ToString(), batchRunControl.TableName, responseDataFileNameWithExtension, true);

                    _logger.LogError($"TraceId:{_appSettings.TraceId}; Critical Error Occurred. {ex}");
                    batchRunControl.ExtractedFileName = responseDataFileNameWithExtension;
                    batchRunControl.ErrorMessage = ex.ToString();
                    batchRunControl.Success = "FAILED";
                    await _odsRepository.CompletedErrorAsync(batchRunControl, ex.ToString());
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

        private async Task<bool> SaveData(string response, string tableName, string fileName, bool isError = false)
        {
            if (isError)
                fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_ERROR{Path.GetExtension(fileName)}";
            else
                fileName = Path.GetFileName(fileName);

            if (_appSettings.ArchiveBucket.NotNullAndNotEmpty())
            {
                var key = isError ? $"{_appSettings.Env}/Extractor/Errors/{tableName}/{fileName}" : $"{_appSettings.Env}/Extractor/Data/{tableName}/{fileName}";

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
                var basePath = isError ? Path.Combine(_appSettings.LocalArchivePath, _appSettings.Env, "Extractor", "Errors", tableName) 
                                       : Path.Combine(_appSettings.LocalArchivePath, _appSettings.Env, "Extractor", "Data", tableName);

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


        private async Task<BatchRunControl> ExtractSingleBatch(BatchRunControl batchRunControl, ODSData odsData)
        {
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting ODS Extraction - ExtractSingleBatch");

            if (string.IsNullOrEmpty(batchRunControl.RaveDataUrl))
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; RaveDataUrl is empty; Aborting.");
                return batchRunControl;
            }

            try
            {
                var (baseUrl, queryParams, pageSize, startDate, endDate, tableName, pageNumber) = ParseRaveDataUrl(batchRunControl.RaveDataUrl, batchRunControl.TableName);

                odsData.TableName = tableName;
                odsData.URL = baseUrl;
                odsData.StartDate = startDate;
                odsData.EndDate = endDate;
                odsData.RecordCount = pageSize;
                odsData.NoOfRecords = batchRunControl.NoOfRecords;

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Current TableName : {odsData.TableName}, Current Id : {batchRunControl.Id}, Current StartDate : {batchRunControl.ApiStartDate}, Current EndDate : {batchRunControl.ApiEndDate}, RaveDataUrl : {batchRunControl.RaveDataUrl}, PageSize : {pageSize}, PageNumber : {pageNumber}, URL StartDate: {startDate}, URL EndDate: {endDate}");

                var retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
                var circuitBreakerPolicy = Policy.Handle<HttpRequestException>().CircuitBreakerAsync(3, TimeSpan.FromMinutes(1));

                DateTime dtStartDate = DateTime.Parse(batchRunControl.ApiStartDate);
                DateTime dtEndDate = DateTime.Parse(batchRunControl.ApiEndDate);
                string formattedStartDate = dtStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                string formattedEndDate = dtEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                string responseDataFileName = $"{odsData.TableName}_{batchRunControl.Id}_{formattedStartDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_TO_{formattedEndDate.Replace(".", "_").Replace(":", "_").Replace("-", "_")}_#PAGENUMBER#.json";

                Payloads payloads = new Payloads { Payload = new List<PayloadItem>() };
                int totalPages = int.MaxValue; // Initialize to a large value

                while (pageNumber <= totalPages)
                {
                    string currentFileName = responseDataFileName.Replace("#PAGENUMBER#", pageNumber.ToString());
                    string url = $"{odsData.URL}?PageSize={pageSize}&PageNumber={pageNumber}&StartDate={formattedStartDate}&EndDate={formattedEndDate}&TableName={odsData.TableName}";

                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Calling Medidata Rave to fetch data for [Table:{odsData.TableName}], [StartDate:{batchRunControl.ApiStartDate}], [EndDate:{batchRunControl.ApiEndDate}], [URL:{url}]");

                    var response = await retryPolicy.ExecuteAsync(async () =>
                    {
                        var apiResponse = await _medidateRWSService.GetAndWriteToDiskWithResponse(odsData.TableName, url, currentFileName);
                        if (apiResponse == null || apiResponse.StatusCode != HttpStatusCode.OK)
                        {
                            throw new HttpRequestException($"HTTP request failed. [Error Exception : {apiResponse?.ErrorException}] [Content: {apiResponse?.Content}]");
                        }
                        return apiResponse;
                    });

                    string json = response.Content.Replace("\"Data\":, \"JsonDataChecksum\":", "\"Data\":[], \"JsonDataChecksum\":");
                    var payload = JsonConvert.DeserializeObject<Payloads>(json);

                    if (payload?.Payload != null && payload.Payload.Any())
                    {
                        var payloadReceived = payload.Payload.First();
                        batchRunControl.HttpStatusCode = response.StatusCode.ToString();
                        batchRunControl.RaveDataUrl = url;
                        batchRunControl.Success = response.StatusCode.ToString();
                        batchRunControl.ExtractedFileName = currentFileName;
                        batchRunControl.NoOfRecords = payloadReceived.TableRowCount;
                        batchRunControl.NoOfRecordsRetrieved = payloadReceived.QueryCountofRows;
                        batchRunControl.TotalPages = payloadReceived.TotalPages;
                        batchRunControl.PageSize = payloadReceived.PageSize;
                        batchRunControl.PageNumber = payloadReceived.PageNumber;
                        totalPages = payloadReceived.TotalPages;

                        await SaveData(json, odsData.TableName, currentFileName, false);

                        foreach (var item in payload.Payload) item.Data = null; // Remove data before saving

                        payloads.Payload.AddRange(payload.Payload);

                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Data saved to file and added to payload, [Table:{odsData.TableName}], [Query Counts :{payloadReceived.QueryCountofRows}]");

                        batchRunControl.Payload = JsonConvert.SerializeObject(payloads);
                        await _odsRepository.UpdateAsync(batchRunControl);
                        _logger.LogInformation($"TraceId:{_appSettings.TraceId}; BatchRunControl updated in database.");
                    }

                    pageNumber++;
                }

                await _odsRepository.CompletedAsync(batchRunControl);
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Extraction completed for [Table:{odsData.TableName}]");

                return batchRunControl;
            }
            catch (HttpRequestException ex)
            {
                await HandleException(batchRunControl, ex, "HTTP Request Error");
                return batchRunControl;
            }
            catch (BrokenCircuitException ex)
            {
                await HandleException(batchRunControl, ex, "Circuit Breaker Open");
                return batchRunControl;
            }
            catch (Exception ex)
            {
                await HandleException(batchRunControl, ex, "General Exception");
                return batchRunControl;
            }
        }

        private (string baseUrl, NameValueCollection queryParams, int pageSize, DateTime startDate, DateTime endDate, string tableName, int pageNumber) ParseRaveDataUrl(string raveDataUrl, string defaultTableName)
        {
            string[] parts = raveDataUrl.Split('?');
            string baseUrl = parts[0];
            string queryString = parts.Length > 1 ? parts[1] : string.Empty;
            NameValueCollection queryParams = null; // Initialize to null

            if (!string.IsNullOrEmpty(queryString))
            {
                queryParams = System.Web.HttpUtility.ParseQueryString(queryString);
            }

            int pageSize = 50000; // Default pageSize
            int pageNumber = 1; // Default pageNumber
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            if (queryParams != null)
            {
                if (queryParams["PageSize"] != null && int.TryParse(queryParams["PageSize"], out int parsedPageSize))
                {
                    pageSize = parsedPageSize;
                }

                if (queryParams["PageNumber"] != null && int.TryParse(queryParams["PageNumber"], out int parsedPageNumber))
                {
                    pageNumber = parsedPageNumber - 1;
                }

                if (queryParams["StartDate"] != null)
                {
                    DateTime.TryParse(queryParams["StartDate"], out startDate);
                }
                if (queryParams["EndDate"] != null)
                {
                    DateTime.TryParse(queryParams["EndDate"], out endDate);
                }
            }

            string tableName = queryParams != null ? queryParams["TableName"] ?? defaultTableName : defaultTableName;

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("TableName is empty.");
            }

            return (baseUrl, queryParams, pageSize, startDate, endDate, tableName, pageNumber);
        }

        private async Task HandleException(BatchRunControl batchRunControl, Exception ex, string errorMessage)
        {
            _logger.LogError($"TraceId:{_appSettings.TraceId}; {errorMessage}: {ex}");
            batchRunControl.ExtractedFileName = "Error.json";
            batchRunControl.ErrorMessage = ex.ToString();
            batchRunControl.Success = "FAILED";
            await SaveData(ex.ToString(), batchRunControl.TableName, "Error.json", true);
            await _odsRepository.CompletedErrorAsync(batchRunControl, ex.ToString());
        }
    }
}