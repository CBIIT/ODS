using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Theradex.Rave.Medidata.Configuration;
using Theradex.Rave.Medidata.Helpers;
using Theradex.Rave.Medidata.Enums;
using Theradex.Rave.Medidata.Interfaces;
using Theradex.Rave.Medidata.Models.Configuration;
using Theradex.Rave.Medidata.Processors;
using Theradex.Rave.Medidata.Services;
using Theradex.Rave.Medidata.Repositories;

namespace Theradex.Rave.Medidata
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

                    services.AddTransient<IODSRepository, ODSRepository>();

                    services.AddSingleton<IConfigManager, ConfigManager>();                    

                    services.AddScoped<ExtractRaveODSData_Processor>();

                    services.AddTransient<Func<ExtractorTypeEnum, IProcessor>>(serviceProvider =>
                        (ext) =>
                        {
                            if (ext == ExtractorTypeEnum.ExtractRaveODSData)
                                return serviceProvider.GetService<ExtractRaveODSData_Processor>();
                            else
                                return null;
                        });

                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.AddNLog("nlog.config");
                    });

                    services.AddOptions();

                    var configurationRoot = context.Configuration;

                    services.Configure<AppSettings>(configurationRoot.GetSection(nameof(AppSettings)));

                    services.Configure<RWSSettings>(configurationRoot.GetSection(nameof(RWSSettings)));

                    services.Configure<EmailSettings>(configurationRoot.GetSection(nameof(EmailSettings)));

                    services.Configure<ODSSettings>(configurationRoot.GetSection(nameof(ODSSettings)));

                })
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.AddSystemsManager(configSource =>
                    {
                        var env = Environment.GetEnvironmentVariable("ODSExtractorEnvironment");
                        configSource.Path = $"/{env}/app/odsextractor/";
                        configSource.Optional = false;
                    });

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