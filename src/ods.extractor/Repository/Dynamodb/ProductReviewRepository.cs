using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Theradex.ODS.Extractor.Interfaces;
using Theradex.ODS.Extractor.Models.Configuration;
using Theradex.ODS.Models.DataAccess;

namespace Theradex.ODS.Extractor.Repository.Dynamodb
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<ProductReviewRepository> _logger;
        protected readonly IAWSCoreHelper _awsCoreHelper;
        private readonly DynamoDBContext _context;

        public ProductReviewRepository(ILogger<ProductReviewRepository> logger, IOptions<AppSettings> appOptions, IAWSCoreHelper awsCoreHelper, IAmazonDynamoDB dynamoDbClient)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _awsCoreHelper = awsCoreHelper;

            if (dynamoDbClient == null) throw new ArgumentNullException(nameof(dynamoDbClient));
            _context = new DynamoDBContext(dynamoDbClient);

        }
        public async Task AddAsync(ProductReviewItem reviewItem)
        {
            await _context.SaveAsync(reviewItem);
        }

        public async Task<IEnumerable<ProductReviewItem>> GetAllAsync()
        {
            return await _context.ScanAsync<ProductReviewItem>(new List<ScanCondition>()).GetRemainingAsync();
        }

        public async Task<IEnumerable<ProductReviewItem>> GetUserReviewsAsync(int userId)
        {
            return await _context.QueryAsync<ProductReviewItem>(userId).GetRemainingAsync();
        }

        public async Task<ProductReviewItem> GetReviewAsync(int userId, string productName)
        {
            return await _context.LoadAsync<ProductReviewItem>(userId, productName);
        }
    }
}