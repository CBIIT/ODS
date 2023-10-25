using Amazon.S3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Theradex.Rave.Medidata.Helpers;
using Theradex.Rave.Medidata.Helpers.Extensions;
using Theradex.Rave.Medidata.Interfaces;
using Theradex.Rave.Medidata.Models;
using Theradex.Rave.Medidata.Models.Configuration;
using Theradex.Rave.Medidata.Repositories;

namespace Theradex.Rave.Medidata.Processors
{
    public class ExtractRaveODSData_Processor : BaseProcessor, IProcessor
    {
        const int MaxPageData = 50000;

        public ExtractRaveODSData_Processor(
            IMedidataRWSService medidateRWSService,
            ILogger<ExtractRaveODSData_Processor> logger,
            IOptions<AppSettings> appOptions,
            IAWSCoreHelper awsCoreHelper,
            IODSRepository odsRepository,
            IAmazonS3 s3Client) : base(medidateRWSService, logger, appOptions, awsCoreHelper, odsRepository, s3Client)
        {
        }

        public async Task<bool> ProcessAsync(ExtractorInput exInput)
        {
            try
            {
                var totalPages = CalculatePages(exInput.Count, MaxPageData);

                for (int pageNo = 1; pageNo <= totalPages; pageNo++)
                {
                    var isSuccess = await _medidateRWSService.GetData(exInput.StartDate, exInput.EndDate, exInput.TableName, pageNo, MaxPageData);

                    if(isSuccess)
                    {
                        await _odsRepository.Update();
                    }
                }
                               
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private int CalculatePages(int totalCount, int pageSize)
        {
            int fullyFilledPages = totalCount / pageSize;
            int partiallyFilledPages = (totalCount % pageSize == 0) ? 0 : 1;

            return fullyFilledPages + partiallyFilledPages;
        }
    }
}