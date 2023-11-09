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

            if (args.Length < 2)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; 2 Arguments needed to Process. Arguments Passed: {string.Join(",", args)}. Aborting.");

                return null;
            }

            var extractorType = args[0];           
            var tableName = args[1];
            
            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Execution Parameters: extractorType {extractorType}; tableName {tableName};");

            if (extractorType.IsNullOrEmpty() || tableName.IsNullOrEmpty())
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

            return new ExtractorInput { TableName = tableName, ExtractorType = extractorTypeToRun };
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

                var startTime = DateTime.Now;

                var isSuccess = await processor.ProcessAsync(extractorInput);

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Completed Extraction; TimeTaken: {DateTime.Now.Subtract(startTime).TotalMinutes} mins");

            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Exception in RunAsync: {ex}");
            }
        }
    }
}