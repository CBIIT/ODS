using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Theradex.ODS.Manager.Configuration;
using Theradex.ODS.Manager.Helpers;
using Theradex.ODS.Manager.Enums;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Manager.Models.Configuration;
using Theradex.ODS.Manager.Processors;
using Theradex.ODS.Manager.Services;
using Theradex.ODS.Models;
using Theradex.ODS.Manager.Repositories;
using LocalStack.Client.Extensions;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using System;
using Microsoft.AspNetCore.Hosting;
using Amazon;
using Microsoft.Extensions.Options;

namespace Theradex.ODS.Manager
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
            Environment.SetEnvironmentVariable("AWS_SERVICE_URL", string.Empty);

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IApp, App>();
                    
                    services.AddLocalStack(context.Configuration);
                    services.AddDefaultAWSOptions(context.Configuration.GetAWSOptions());

                    if (environmentName.ToLower() == "development")
                    {
                        services.AddSingleton<IAmazonDynamoDB>(sp =>
                        {
                            var clientConfig = new AmazonDynamoDBConfig
                            {
                                ServiceURL = "http://localhost:4566",
                                UseHttp = true
                            };
                            return new AmazonDynamoDBClient(new BasicAWSCredentials("testkey", "testsecret"), clientConfig);
                        });
                        Console.WriteLine("Added LocalStack DynamoDb");

                        var amazonS3 = new AmazonS3Client(new BasicAWSCredentials("testkey", "testsecret"), new AmazonS3Config
                        {
                            RegionEndpoint = RegionEndpoint.USEast1,
                            ServiceURL = "http://localhost:4566",
                            ForcePathStyle = true,
                            UseHttp = true,
                            AuthenticationRegion = "us-east-1",
                        });

                        services.AddSingleton(typeof(IAmazonS3), provider => amazonS3);
                        Console.WriteLine("Added LocalStack S3");
                    }
                    else
                    {
                        services.AddAWSService<IAmazonDynamoDB>();
                        services.AddAWSService<IAmazonS3>();
                    }

                    services.AddSingleton<IConfigManager, ConfigManager>();

                    services.AddTransient<IAWSCoreHelper, AWSCoreHelper>();

                    services.AddTransient<IMedidataRWSService, MedidataRWSService>();

                    services.AddTransient<IBatchRunControlRepository<BatchRunControl>, BatchRunControlRepository>();
                    services.AddTransient<IManagerTableInfoRepository<BatchRunControl>, ManagerTableInfoRepository>();

                    services.AddScoped<ODSManager_Processor>();

                    services.AddTransient<Func<ExtractorTypeEnum, IProcessor>>(serviceProvider =>
                        (ext) =>
                        {
                            if (ext == ExtractorTypeEnum.ODSManager)
                                return serviceProvider.GetService<ODSManager_Processor>();
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
                    //configuration.AddSystemsManager(configSource =>
                    //{
                    //    var env = Environment.GetEnvironmentVariable("ODSManagerEnvironment");
                    //    configSource.Path = $"/{env}/app/odsmanager/";
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