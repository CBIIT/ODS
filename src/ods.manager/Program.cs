using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using Npgsql;
using System.Configuration;
using Theradex.ODS.Manager;
using Theradex.ODS.Manager.Configuration;
using Theradex.ODS.Manager.Enums;
using Theradex.ODS.Manager.Helpers;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Manager.Models.Configuration;
using Theradex.ODS.Manager.Processors;
using Theradex.ODS.Manager.Repositories;
using Theradex.ODS.Manager.Repository.Dynamodb;
using Theradex.ODS.Manager.Services;
using Theradex.ODS.Models;

class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await RunAsync(args, host.Services);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                AddAWSConfiguration(context, services);
                AddCustomServices(services);
                ConfigureLogging(services);
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                AddEnvironmentSpecificConfiguration(context, config);
            });

    private static void AddAWSConfiguration(HostBuilderContext context, IServiceCollection services)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var isLocal = environmentName.Equals("local", StringComparison.OrdinalIgnoreCase);
        var awsCredentials = isLocal ? new BasicAWSCredentials("testkey", "testsecret") : null;
        var awsConfig = new AmazonDynamoDBConfig();

        if (isLocal)
        {
            awsConfig = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.USEast1,
                ServiceURL = "http://localhost:4566",
                UseHttp = true,
                AuthenticationRegion = "us-east-1",
            };

            services.AddSingleton<IAmazonDynamoDB>(provider => new AmazonDynamoDBClient(awsCredentials, awsConfig));
            services.AddSingleton<IAmazonS3>(provider => new AmazonS3Client(awsCredentials, new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1,
                ServiceURL = "http://localhost:4566",
                ForcePathStyle = true,
                UseHttp = true,
                AuthenticationRegion = "us-east-1",
            }));

            Console.WriteLine("Added LocalStack AWS Services");
        }
        else
        {
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonS3>();
        }
        var configurationRoot = context.Configuration;

        services.Configure<AppSettings>(configurationRoot.GetSection(nameof(AppSettings)));

        services.Configure<RWSSettings>(configurationRoot.GetSection(nameof(RWSSettings)));

        services.Configure<EmailSettings>(configurationRoot.GetSection(nameof(EmailSettings)));

        services.Configure<ODSSettings>(configurationRoot.GetSection(nameof(ODSSettings)));
    }

    private static void AddCustomServices(IServiceCollection services)
    {
        services.AddSingleton<IConfigManager, ConfigManager>();
        services.AddTransient<IAWSCoreHelper, AWSCoreHelper>();
        services.AddScoped<IDynamoDBContext, DynamoDBContext>();

        services.AddTransient<IMedidataRWSService, MedidataRWSService>();

        services.AddSingleton<IBatchRunControlRepository<BatchRunControl>, BatchRunControlRepository>();
        services.AddSingleton<IManagerTableInfoRepository<BatchRunControl>, ManagerTableInfoRepository>();
        services.AddSingleton<IProductReviewRepository, ProductReviewRepository>();


        services.AddScoped<ODSManager_Processor>();
        services.AddScoped<ODSManager_Incremental_Processor>();

        services.AddTransient<Func<ManagerTypeEnum, IProcessor>>(serviceProvider =>
            (ext) =>
            {
                if (ext == ManagerTypeEnum.ODSManager)
                    return serviceProvider.GetService<ODSManager_Processor>();
                else if (ext == ManagerTypeEnum.ODSManager_Incremental)
                    return serviceProvider.GetService<ODSManager_Incremental_Processor>();
                else
                    return null;
            });

    }

    private static void ConfigureLogging(IServiceCollection services)
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddNLog("nlog.config");
        });
        var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddNLog("nlog.config"));

        NpgsqlLoggingConfiguration.InitializeLogging(loggerFactory, parameterLoggingEnabled: true);

        LogManager.ReconfigExistingLoggers();
    }

    private static void AddEnvironmentSpecificConfiguration(HostBuilderContext context, IConfigurationBuilder config)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var isLocal = environmentName.Equals("local", StringComparison.OrdinalIgnoreCase);

        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
        config.AddEnvironmentVariables();

        if (isLocal)
        {
            var env = Environment.GetEnvironmentVariable("ODSEnvironment");
            config.AddSystemsManager($"/{env}/app/odsmanager/", new AWSOptions
            {
                DefaultClientConfig = {
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
            config.AddSystemsManager(configSource =>
            {
                var env = Environment.GetEnvironmentVariable("ODSEnvironment");
                configSource.Path = $"/{env}/app/odsmanager/";
                configSource.Optional = false;
            });
        }
    }

    private static async Task RunAsync(string[] args, IServiceProvider services)
    {
        var service = ActivatorUtilities.CreateInstance<App>(services);

        await service.RunAsync(args);
    }
}
