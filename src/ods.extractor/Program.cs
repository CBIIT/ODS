using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using Npgsql;
using Theradex.ODS.Extractor.Configuration;
using Theradex.ODS.Extractor.Enums;
using Theradex.ODS.Extractor.Helpers;
using Theradex.ODS.Extractor.Interfaces;
using Theradex.ODS.Extractor.Models.Configuration;
using Theradex.ODS.Extractor.Processors;
using Theradex.ODS.Extractor.Services;
using Theradex.ODS.Models;

namespace Theradex.ODS.Extractor
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

            // entry to run app
            await RunAsync(args, host.Services);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IApp, App>();

                    services.AddDefaultAWSOptions(context.Configuration.GetAWSOptions());

                    services.AddAWSService<IAmazonS3>();

                    services.AddTransient<IAWSCoreHelper, AWSCoreHelper>();

                    services.AddTransient<IMedidataRWSService, MedidataRWSService>();

                    services.AddSingleton<IBatchRunControlRepository<BatchRunControl>, BatchRunControlRepository>();

                    services.AddSingleton<IConfigManager, ConfigManager>();

                    services.AddScoped<ODSExtractor_Processor>();

                    services.AddTransient<Func<ExtractorTypeEnum, IProcessor>>(serviceProvider =>
                        (ext) =>
                        {
                            if (ext == ExtractorTypeEnum.ODSExtractor)
                                return serviceProvider.GetService<ODSExtractor_Processor>();
                            else
                                return null;
                        });

                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.AddNLog("nlog.config");
                    });
                    var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddNLog("nlog.config"));
                    NpgsqlLoggingConfiguration.InitializeLogging(loggerFactory, parameterLoggingEnabled: true);

                    LogManager.ReconfigExistingLoggers();

                    services.AddOptions();

                    var configurationRoot = context.Configuration;

                    services.Configure<AppSettings>(configurationRoot.GetSection(nameof(AppSettings)));

                    services.Configure<RWSSettings>(configurationRoot.GetSection(nameof(RWSSettings)));

                    services.Configure<EmailSettings>(configurationRoot.GetSection(nameof(EmailSettings)));

                    services.Configure<ODSSettings>(configurationRoot.GetSection(nameof(ODSSettings)));


                })
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    //configuration.AddSystemsManager(configSource =>
                    //{
                    //    var env = Environment.GetEnvironmentVariable("ODSExtractorEnvironment");
                    //    configSource.Path = $"/{env}/app/odsextractor/";
                    //    configSource.Optional = false;
                    //});

                    IHostEnvironment env = hostingContext.HostingEnvironment;
                    IConfigurationRoot configurationRoot = configuration.Build();

                    configuration.SetBasePath(Directory.GetCurrentDirectory());
                    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configuration.AddEnvironmentVariables();
                });

            return host;
        }

        private static async Task RunAsync(string[] args, IServiceProvider services)
        {
            var service = ActivatorUtilities.CreateInstance<App>(services);

            await service.RunAsync(args);
        }
    }
}