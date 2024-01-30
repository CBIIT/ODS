 using Microsoft.Extensions.Logging;
using Theradex.ODS.Extractor.Enums;
using Theradex.ODS.Extractor.Interfaces;
using Theradex.ODS.Extractor.Models.Configuration;
using Theradex.ODS.Extractor.Helpers.Extensions;
using Microsoft.Extensions.Options;
using NLog;
using Theradex.ODS.Extractor.Configuration;
using Theradex.ODS.Extractor.Models;

namespace Theradex.ODS.Extractor
{
    public class App : IApp
    {
        private readonly ILogger<App> _logger;
        private readonly AppSettings _appSettings;
        private readonly RWSSettings _rwsSettings;
        private readonly EmailSettings _emailSettings;
        private readonly IAWSCoreHelper _awsCoreHelper;
        private readonly Func<ExtractorTypeEnum, IProcessor> _processServiceResolver;
        private readonly IConfigManager _configManager;
        private const int INPUT_COUNT = 50000;
        private string[] requiredArguments = { "managerType", "tableName", "env", "raveDataUrl" };

        public App(ILogger<App> logger,
            IOptions<AppSettings> appOptions,
            IOptions<RWSSettings> rwsOptions,
            IOptions<EmailSettings> emailOptions,
            IAWSCoreHelper awsCoreHelper,
            IConfigManager configManager,
            Func<ExtractorTypeEnum, IProcessor> processServiceResolver)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _rwsSettings = rwsOptions.Value;
            _emailSettings = emailOptions.Value;
            _awsCoreHelper = awsCoreHelper;
            _configManager = configManager;
            _processServiceResolver = processServiceResolver;
        }

        private ExtractorInput? ValidateAndParseArguments(string[] args)
        {
            
            //--managerType=ODSManager --tableName=FOLDERS --env=dev --raveDataUrl="/RaveWebServices/datasets/ThxExtracts2.json"
            var commandLineArgs = new CommandLineArgs(args);

            if (!commandLineArgs.HasRequiredArguments(requiredArguments))
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId};Missing one or more required arguments.");
                _logger.LogError($"TraceId:{_appSettings.TraceId};Required arguments are:");
                foreach (var arg in requiredArguments)
                {
                    _logger.LogError($"TraceId:{_appSettings.TraceId};--{arg}");
                }
                return null;
            }

            var ManagerType = string.IsNullOrEmpty(commandLineArgs["managerType"]) == false ? commandLineArgs["managerType"] : "ODSManager";
            var tableName = string.IsNullOrEmpty(commandLineArgs["tableName"]) == false ? commandLineArgs["tableName"] : "NONE";
            var env = string.IsNullOrEmpty(commandLineArgs["env"]) == false ? commandLineArgs["env"] : "ODSManager";
            var raveDataUrl = string.IsNullOrEmpty(commandLineArgs["raveDataUrl"]) == false ? commandLineArgs["raveDataUrl"] : "/RaveWebServices/datasets/ThxExtracts2.json";

            int noOfRecords = 1;
            Int32.TryParse(commandLineArgs["noOfRecords"], out noOfRecords);

            if (noOfRecords <= 0) noOfRecords = 1;


            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Execution Parameters: extractorType {ManagerType}; tableName {tableName}; env {env};");

            if (ManagerType.IsNullOrEmpty() || tableName.IsNullOrEmpty() || env.IsNullOrEmpty())
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; One or more execution Parameters are empty; Aborting.");

                return null;
            }

            var isValid = Enum.TryParse(ManagerType, true, out ExtractorTypeEnum extractorTypeToRun);

            if (!isValid)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; extractorType {ManagerType} not valid; Aborting.");

                return null;
            }

            _appSettings.Env = env;

            return new ExtractorInput { TableName = tableName, NoOfRecords = noOfRecords, ExtractorType = extractorTypeToRun, };
        }

        public async Task RunAsync(string[] args)
        {
            ExtractorInput? extractorInput = null;
            try
            {
                _appSettings.TraceId = CustomerExtensions.GetUniqueIdentifier();

                GlobalDiagnosticsContext.Set("TraceId", _appSettings.TraceId);

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting...");

                extractorInput = ValidateAndParseArguments(args);

                if (extractorInput == null) return;

                var processor = _processServiceResolver(extractorInput.ExtractorType);

                if (processor == null)
                {
                    _logger.LogError($"TraceId:{_appSettings.TraceId}; No Processer configured for  extractorType {extractorInput.ExtractorType}; Aborting;");

                    return;
                }

                var isLockAcquired = await AcquireLockAsync(extractorInput);

                if(!isLockAcquired)
                {
                    _logger.LogWarning($"TraceId:{_appSettings.TraceId}; Lock not acquired; TableName: {extractorInput.TableName}; Aborting;");

                    return;
                }

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Lock acquired; TableName: {extractorInput.TableName}; Proceeding;");

                var startTime = DateTime.Now;

                var isSuccess = await processor.ProcessAsync(extractorInput);                

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Completed Extraction; TimeTaken: {DateTime.Now.Subtract(startTime).TotalMinutes} mins");

                await ReleaseLockAsync(extractorInput);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Exception in RunAsync: {ex}");

                await ReleaseLockAsync(extractorInput);
            }            
        }

        public async Task<bool> AcquireLockAsync(ExtractorInput extractorInput)
        {
            var key = $"{_appSettings.Env}/Extractor/Lock/{extractorInput.TableName}_lock";
            try
            {
                var isKeyExists = await _awsCoreHelper.IsKeyExistsAsync(_appSettings.ArchiveBucket, key);

                if (isKeyExists) //Lock file already exists.
                {
                    _logger.LogWarning($"TraceId:{_appSettings.TraceId}; TableName: {extractorInput.TableName}; Lock file {key} already exists;");

                    return false;
                }

                var isSuccess = await _awsCoreHelper.UploadDataAsync(_appSettings.ArchiveBucket, key, "Lock file content");

                if(isSuccess) //Lock file successfully created.
                {
                    _logger.LogWarning($"TraceId:{_appSettings.TraceId}; TableName: {extractorInput.TableName}; Lock file {key} created;");
                }

                return isSuccess; 
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; TableName: {extractorInput.TableName}; Lock file {key}; Exception in AcquireLockAsync; {ex};");

                return false;
            }
        }

        public async Task ReleaseLockAsync(ExtractorInput? extractorInput)
        {
            if (extractorInput == null)
                return;

            var key = $"{_appSettings.Env}/Extractor/Lock/{extractorInput.TableName}_lock";
            try
            {
                var isSuccess = await _awsCoreHelper.DeleteObjectFromBucketAsync(_appSettings.ArchiveBucket, key);

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; TableName: {extractorInput.TableName}; Unlocked successfully; Lock file {key} deleted;");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; TableName: {extractorInput.TableName}; Lock file {key}; Exception in ReleaseLockAsync; {ex};");
            }
        }
    }
    class CommandLineArgs
    {
        private readonly Dictionary<string, string> argsDictionary = new Dictionary<string, string>();
        public string this[string key] => argsDictionary.TryGetValue(key, out var value) ? value : string.Empty;

        public CommandLineArgs(string[] args)
        {
            foreach (var arg in args)
            {
                var split = arg.Split(new char[] { '=' }, 2);
                if (split.Length == 2 && split[0].StartsWith("--"))
                {
                    var key = split[0].Substring(2); // Remove the "--" prefix
                    var value = split[1];
                    argsDictionary[key] = value;
                }
            }
        }
        public bool HasRequiredArguments(params string[] requiredArgs)
        {
            return requiredArgs.All(arg => argsDictionary.ContainsKey(arg));
        }
    }
}