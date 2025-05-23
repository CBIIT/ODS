﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;
using Theradex.ODS.Extractor.Helpers.Extensions;
using Theradex.ODS.Extractor.Interfaces;
using Theradex.ODS.Extractor.Models.Configuration;
using System;
using Theradex.ODS.Models;
using Org.BouncyCastle.Crypto.Prng;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Extension.Methods;

namespace Theradex.ODS.Extractor.Services
{
    public class MedidataRWSService : IMedidataRWSService
    {
        private readonly ILogger<MedidataRWSService> _logger;
        private readonly RWSSettings _rwsSettings;
        private readonly AppSettings _appSettings;
        protected readonly IAWSCoreHelper _awsCoreHelper;

        public MedidataRWSService(ILogger<MedidataRWSService> logger, IOptions<RWSSettings> rwsOptions, IOptions<AppSettings> appOptions, IAWSCoreHelper awsCoreHelper)
        {
            _logger = logger;
            _rwsSettings = rwsOptions.Value;
            _appSettings = appOptions.Value;
            _awsCoreHelper = awsCoreHelper;
        }

        private async Task<bool> SaveData(string response, string tableName, string fileName)
        {
            if(_appSettings.ArchiveBucket.NotNullAndNotEmpty())
            {
                var key = $"{_appSettings.Env}/Extractor/Medidata/{tableName}/{fileName}";

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
                var basePath = Path.Combine(_appSettings.LocalArchivePath, _appSettings.Env, "Extractor", "Data", tableName);

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


        public async Task<bool> GetData(DateTime startDate, DateTime endDate, string tableName, int pageNumber, int pageSize)
        {
            var resource = $"/RaveWebServices/datasets/ThxExtracts2.json?PageSize={pageSize}&PageNumber={pageNumber}&StartDate={startDate:yyyy-MM-ddTHH:mm:ss}&EndDate={endDate:yyyy-MM-ddTHH:mm:ss}&TableName={tableName}";

            try
            {
                var client = new RestClient(new RestClientOptions { Authenticator = new HttpBasicAuthenticator(_rwsSettings.RWSUserName, _rwsSettings.RWSPassword) });


                var request = new RestRequest(_rwsSettings.RWSServer + resource)
                {
                    Timeout = _rwsSettings.TimeoutInSecs.ConvertSecsToMs()
                };

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Getting data from Rave; {resource};");

                var response = await client.GetAsync(request);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Successfully got data from Rave; {resource};");

                    return true;
                }
                else
                {
                    _logger.LogError($"TraceId:{_appSettings.TraceId}; Failed getting data from Rave; {resource};");

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Exception getting data from Rave; {resource}; Exception: {ex};");

                return false;
            }
        }

        public async Task<RestResponse> GetAndWriteToDiskWithResponse(string tableName, string uri, string responseDataFileNameWithExtensionRAW)
        {
            var resource = uri;

            try
            {
                var client = new RestClient(new RestClientOptions { Authenticator = new HttpBasicAuthenticator(_rwsSettings.RWSUserName, _rwsSettings.RWSPassword) , MaxTimeout= _rwsSettings.TimeoutInSecs.ConvertSecsToMs()});

                var request = new RestRequest(_rwsSettings.RWSServer + resource)
                {
                    Timeout = _rwsSettings.TimeoutInSecs.ConvertSecsToMs(),
                };

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Rave Timeout (milliseconds): {request.Timeout}; {resource};");

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Getting data from Rave; {resource};");

                var response = await client.GetAsync(request);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Successfully got data from Rave; {resource};");

                    var isSaveSuccess = await SaveData(response.Content, tableName, responseDataFileNameWithExtensionRAW);

                    // Write the response to a file
                    //await File.WriteAllTextAsync(responseDataFileNameWithExtensionRAW, response != null ? response.Content : string.Empty);

                    return response;
                }
                else
                {
                    _logger.LogError($"TraceId:{_appSettings.TraceId}; Failed getting data from Rave; {resource};");

                    // Write the response to a file
                    //await File.WriteAllTextAsync(responseDataFileNameWithExtensionRAW.Replace("json", "_ERROR.json"), response != null ? response.Content : string.Empty);

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Exception getting data from Rave; {resource}; Exception: {ex};");
            }
            return null;
        }
    }
}