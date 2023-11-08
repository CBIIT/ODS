using Microsoft.Extensions.Logging;
using Theradex.ODS.Manager.Enums;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Manager.Models.Configuration;
using Theradex.ODS.Manager.Helpers.Extensions;
using Microsoft.Extensions.Options;
using NLog;
using Theradex.ODS.Manager.Configuration;
using Theradex.ODS.Manager.Models;

namespace Theradex.ODS.Manager
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
            if (args.Length == 0)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; No Arguments passed. Aborting.");

                return null;
            }

            if (args.Length < 5)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; 5 Arguments needed to Process. Arguments Passed: {string.Join(",", args)}. Aborting.");

                return null;
            }

            var extractorType = args[0];
            var startDate = args[1];
            var endDate = args[2];
            var tableName = args[3];
            var count = args[4];

            var s3Enabled = "false";
            var s3BucketName = string.Empty;
            var localEnabled = "true";
            var localPath = @"ODS\Manager\Data\";

            if (args.Length >=9)
            {
                s3Enabled = args[5];
                s3BucketName = args[6];
                localEnabled = args[7];
                localPath = args[8];
            }
            

          


            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Execution Parameters: extractorType {extractorType}; startDate {startDate}; endDate {endDate}; tableName {tableName}; count {count}; s3Enabled {s3Enabled}; s3BucketName {s3BucketName}; localEnabled {localEnabled}; localPath {localPath}; ");

            if (extractorType.IsNullOrEmpty() || startDate.IsNullOrEmpty() || endDate.IsNullOrEmpty() || tableName.IsNullOrEmpty())
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; One or more execution Parameters are empty; Aborting.");

                return null;
            }

            var isValid = Enum.TryParse(extractorType, true, out ExtractorTypeEnum extractorTypeToRun);

            if (!isValid)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; extractorType {extractorType} not valid; Aborting.");

                return null;
            }

            isValid = int.TryParse(count, out int count1);

            if (!isValid)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; count {count} not valid; Aborting.");

                return null;
            }

            isValid = DateTime.TryParse(startDate, out DateTime startDate1);

            if (!isValid)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; startDate {startDate} not valid; Aborting.");

                return null;
            }

            isValid = DateTime.TryParse(endDate, out DateTime endDate1);

            if (!isValid)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; endDate {endDate} not valid; Aborting.");

                return null;
            }

            isValid = bool.TryParse(s3Enabled, out bool s3Enabled1);

            if (!isValid)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; s3Enabled {s3Enabled} not valid or provided; Defaulting to false.");
                s3Enabled1 = false;
            }

            isValid = bool.TryParse(localEnabled, out bool localEnabled1);

            if (!isValid)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; localEnabled {localEnabled} not valid or provided; Defaulting to true.");
                localEnabled1 = false;
            }

            if (s3Enabled1 && string.IsNullOrEmpty(s3BucketName))
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; s3BucketName {s3BucketName} not valid or provided; Aborting.");
                return null;
            }

            string localPath1 = @"ODS\Manager\Data";
            if (localEnabled1 && string.IsNullOrEmpty(localPath))
            {
                _logger.LogError(@$"TraceId:{_appSettings.TraceId}; localEnabled {localEnabled} not valid or provided; Defaulting to ""ODS\Manager\Data""");
                localPath = @"ODS\Manager\Data";
                return null;
            }


            return new ExtractorInput { StartDate = startDate1, EndDate = endDate1, TableName = tableName, ExtractorType = extractorTypeToRun, LocalEnabled = localEnabled1, LocalPath = localPath1, S3Enabled = s3Enabled1, S3BucketName = s3BucketName };
        }

        public async Task RunAsync(string[] args)
        {
            try
            {
                _appSettings.TraceId = CustomerExtensions.GetUniqueIdentifier();

                GlobalDiagnosticsContext.Set("TraceId", _appSettings.TraceId);

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting...");

                var extractorInput = ValidateAndParseArguments(args);

                if (extractorInput == null) return;

                var processor = _processServiceResolver(extractorInput.ExtractorType);

                if (processor == null)
                {
                    _logger.LogError($"TraceId:{_appSettings.TraceId}; No Processer configured for  extractorType {extractorInput.ExtractorType};");

                    return;
                }

                //_appSettings.CurrentArchiveFolder = $"{extractorInput.TableName}";

                var startTime = DateTime.Now;

                var isSuccess = await processor.ProcessAsync(extractorInput);

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Completed Extraction;");

            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Exception in RunAsync: {ex}");
            }
        }
    }
}