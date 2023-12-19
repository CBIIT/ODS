using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Amazon.DynamoDBv2.DataModel;

namespace Theradex.ODS.Models.DataAccess
{
    [DynamoDBTable("ProductReview")]
    public class ProductReviewItem
    {
        [DynamoDBHashKey]
        public int UserId { get; set; }
        [DynamoDBRangeKey]
        public string ProductName { get; set; }
        public int Rank { get; set; }
        public string Review { get; set; }
        public DateTime ReviewOn { get; set; }
    }
}