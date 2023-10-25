using Theradex.Rave.Medidata.Enums;
using Theradex.Rave.Medidata.Helpers.Extensions;
using Theradex.Rave.Medidata.Interfaces;
using Theradex.Rave.Medidata.Models;
using Theradex.Rave.Medidata.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amazon.S3;

namespace Theradex.Rave.Medidata.Processors
{
    public class BaseProcessor
    {
        protected readonly IMedidataRWSService _medidateRWSService;
        protected readonly ILogger<BaseProcessor> _logger;
        protected readonly AppSettings _appSettings;
        protected readonly IAWSCoreHelper _awsCoreHelper;
        protected readonly IAmazonS3 _s3Client;
        protected readonly IODSRepository _odsRepository;

        public BaseProcessor(IMedidataRWSService medidateRWSService,
            ILogger<BaseProcessor> logger,
            IOptions<AppSettings> appOptions,          
            IAWSCoreHelper awsCoreHelper,
            IODSRepository odsRepository,
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