using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using EfficientDynamoDb.Configs.Retries;
using LocalStack.Client.Extensions;
using Microsoft.AspNetCore.Builder;
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
using Theradex.ODS.Extractor.Repository.Dynamodb;
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
                 services.AddLocalStack(context.Configuration);
                 services.AddDefaultAWSOptions(context.Configuration.GetAWSOptions());

                 if (environmentName.ToLower() == "local")
                 {
                     var amazonDynamodb = new AmazonDynamoDBClient(new BasicAWSCredentials("testkey", "testsecret"), new AmazonDynamoDBConfig
                     {
                         RegionEndpoint = RegionEndpoint.USEast1,
                         ServiceURL = "http://localhost:4566",
                         UseHttp = true,
                         AuthenticationRegion = "us-east-1",
                     });

                     services.AddSingleton(typeof(IAmazonDynamoDB), provider => amazonDynamodb);

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

                 services.AddScoped<IDynamoDBContext, DynamoDBContext>();


                 services.AddTransient<IAWSCoreHelper, AWSCoreHelper>();

                 services.AddTransient<IMedidataRWSService, MedidataRWSService>();

                 services.AddSingleton<IBatchRunControlRepository<BatchRunControl>, BatchRunControlRepository>();
                 services.AddSingleton<IProductReviewRepository, ProductReviewRepository>();

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
                 var env_ = Environment.GetEnvironmentVariable("ODSExtractorEnvironment");

                 if (environmentName.ToLower() == "local")
                 {
                     configuration.AddSystemsManager($"/{env_}/app/odsextractor/", new AWSOptions
                     {
                         DefaultClientConfig =
                            {
                                ServiceURL = "http://localhost:4566",
                                UseHttp = true,
                                AuthenticationRegion = "us-east-1",
                            },
                         Credentials = new BasicAWSCredentials("testkey", "testsecret")
                     });

                     Console.WriteLine("Added LocalStack SSM");
                 }
                 else
                 {
                     configuration.AddSystemsManager(configSource =>
                     {
                         var env = Environment.GetEnvironmentVariable("ODSExtractorEnvironment");
                         configSource.Path = $"/{env}/app/odsextractor/";
                         configSource.Optional = false;
                     });
                 }

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