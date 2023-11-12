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
        private readonly Func<ManagerTypeEnum, IProcessor> _processServiceResolver;
        private readonly IConfigManager _configManager;
        private const int INPUT_COUNT = 50000;

        public App(ILogger<App> logger,
            IOptions<AppSettings> appOptions,
            IOptions<RWSSettings> rwsOptions,
            IOptions<EmailSettings> emailOptions,
            IAWSCoreHelper awsCoreHelper,
            IConfigManager configManager,
            Func<ManagerTypeEnum, IProcessor> processServiceResolver)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _rwsSettings = rwsOptions.Value;
            _emailSettings = emailOptions.Value;
            _awsCoreHelper = awsCoreHelper;
            _configManager = configManager;
            _processServiceResolver = processServiceResolver;
        }

        private ManagerInput? ValidateAndParseArguments(string[] args)
        {
            if (args.Length == 0)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; No Arguments passed. Aborting.");

                return null;
            }

            if (args.Length < 3)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; 3 Arguments needed to Process. Arguments Passed: {string.Join(",", args)}. Aborting.");

                return null;
            }

            var ManagerType = args[0];
            var tableName = args[1];
            var env = args[2];

            _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Execution Parameters: ManagerType {ManagerType}; tableName {tableName}; env {env};");

            if (ManagerType.IsNullOrEmpty() || tableName.IsNullOrEmpty() || env.IsNullOrEmpty())
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; One or more execution Parameters are empty; Aborting.");

                return null;
            }

            var isValid = Enum.TryParse(ManagerType, true, out ManagerTypeEnum ManagerTypeToRun);

            if (!isValid)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; ManagerType {ManagerType} not valid; Aborting.");

                return null;
            }

            _appSettings.Env = env;

            return new ManagerInput { TableName = tableName, ManagerType = ManagerTypeToRun };
        }

        public async Task RunAsync(string[] args)
        {
            try
            {
                _appSettings.TraceId = CustomerExtensions.GetUniqueIdentifier();

                GlobalDiagnosticsContext.Set("TraceId", _appSettings.TraceId);

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Starting...");

                var ManagerInput = ValidateAndParseArguments(args);

                if (ManagerInput == null) return;

                var processor = _processServiceResolver(ManagerInput.ManagerType);

                if (processor == null)
                {
                    _logger.LogError($"TraceId:{_appSettings.TraceId}; No Processer configured for  ManagerType {ManagerInput.ManagerType};");

                    return;
                }

                var startTime = DateTime.Now;

                var isSuccess = await processor.ProcessAsync(ManagerInput);

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Completed Extraction; TimeTaken: {DateTime.Now.Subtract(startTime).TotalMinutes} mins");

            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Exception in RunAsync: {ex}");
            }
        }
    }
}