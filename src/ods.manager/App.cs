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
        private string[] requiredArguments = { "managerType", "tableName", "env", "raveDataUrl" };


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

            var ManagerType = string.IsNullOrEmpty(commandLineArgs["managerType"]) ==false ? commandLineArgs["managerType"] : "ODSManager" ;
            var tableName = string.IsNullOrEmpty(commandLineArgs["tableName"]) == false ? commandLineArgs["tableName"] : "NONE";
            var env = string.IsNullOrEmpty(commandLineArgs["env"]) == false ? commandLineArgs["env"] : "ODSManager";
            var raveDataUrl = string.IsNullOrEmpty(commandLineArgs["raveDataUrl"]) == false ? commandLineArgs["raveDataUrl"] : "/RaveWebServices/datasets/ThxExtracts2.json";


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

            return new ManagerInput { TableName = tableName, RaveDataUrl = raveDataUrl, ManagerType = ManagerTypeToRun };
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