using Theradex.ODS.Manager.Enums;

namespace Theradex.ODS.Manager.Models
{
    public class ExtractorInput
    {
        public string TableName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = new DateTime(2005, 1, 1);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int Count { get; set; } = 50000;
        public ExtractorTypeEnum ExtractorType { get; set; } = ExtractorTypeEnum.ODSManager; // Replace DefaultValue with your desired default value

        public bool S3Enabled { get; set; } = false;
        public string S3BucketName { get; set; }

        public bool LocalEnabled { get; set; } = true;
        public string LocalPath { get; set; } = @"ODS\Manager\Data\";
        public string RWSBaseUrl { get; set; } = "/RaveWebServices/datasets/ThxExtractsGetTableIntervalsDetailInfo.json";

    }
}
