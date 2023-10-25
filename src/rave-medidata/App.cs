using Microsoft.Extensions.Logging;
using Theradex.Rave.Medidata.Enums;
using Theradex.Rave.Medidata.Interfaces;
using Theradex.Rave.Medidata.Models.Configuration;
using Theradex.Rave.Medidata.Helpers.Extensions;
using Microsoft.Extensions.Options;
using NLog;
using Theradex.Rave.Medidata.Configuration;
using Theradex.Rave.Medidata.Models;

namespace Theradex.Rave.Medidata
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

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Execution Parameters: extractorType {extractorType}; startDate {startDate}; endDate {endDate}; tableName {tableName}; count {count};");

            if (extractorType.IsNullOrEmpty() || startDate.IsNullOrEmpty() || endDate.IsNullOrEmpty() || tableName.IsNullOrEmpty() || count.IsNullOrEmpty())
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

            return new ExtractorInput { Count = count1, StartDate = startDate1, EndDate = endDate1, TableName = tableName, ExtractorType = extractorTypeToRun };
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