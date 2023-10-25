using Theradex.ODS.Extractor.Enums;
using Theradex.ODS.Extractor.Helpers.Extensions;
using Theradex.ODS.Extractor.Interfaces;
using Theradex.ODS.Extractor.Models;
using Theradex.ODS.Extractor.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amazon.S3;
using Theradex.ODS.Models;

namespace Theradex.ODS.Extractor.Processors
{
    public class BaseProcessor
    {
        protected readonly IMedidataRWSService _medidateRWSService;
        protected readonly ILogger<BaseProcessor> _logger;
        protected readonly AppSettings _appSettings;
        protected readonly IAWSCoreHelper _awsCoreHelper;
        protected readonly IAmazonS3 _s3Client;
        protected readonly IBatchRunControlRepository<BatchRunControl> _odsRepository;

        public BaseProcessor(IMedidataRWSService medidateRWSService,
            ILogger<BaseProcessor> logger,
            IOptions<AppSettings> appOptions,
            IAWSCoreHelper awsCoreHelper,
            IBatchRunControlRepository<BatchRunControl> odsRepository,
            IAmazonS3 s3Client)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _medidateRWSService = medidateRWSService;
            _awsCoreHelper = awsCoreHelper;
            _s3Client = s3Client;
            _odsRepository = odsRepository;
        }
    }
}