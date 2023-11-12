using Amazon.S3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Manager.Models.Configuration;
using Theradex.ODS.Models;

namespace Theradex.ODS.Manager.Processors
{
    public class BaseProcessor
    {
        protected readonly IMedidataRWSService _medidateRWSService;
        protected readonly ILogger<BaseProcessor> _logger;
        protected readonly AppSettings _appSettings;
        protected readonly IAWSCoreHelper _awsCoreHelper;
        protected readonly IAmazonS3 _s3Client;
        protected readonly IBatchRunControlRepository<BatchRunControl> _odsRepository;
        protected readonly IManagerTableInfoRepository<BatchRunControl> _odsManagerRepository;

        public BaseProcessor(IMedidataRWSService medidateRWSService,
            ILogger<BaseProcessor> logger,
            IOptions<AppSettings> appOptions,          
            IAWSCoreHelper awsCoreHelper,
            IBatchRunControlRepository<BatchRunControl> odsRepository,
            IManagerTableInfoRepository<BatchRunControl> odsManagerRepository,
            IAmazonS3 s3Client)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _medidateRWSService = medidateRWSService;
            _awsCoreHelper = awsCoreHelper;
            _s3Client = s3Client;
            _odsRepository = odsRepository;
            _odsManagerRepository = odsManagerRepository;
        }
    }
}