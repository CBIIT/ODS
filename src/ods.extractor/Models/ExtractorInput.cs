using Theradex.ODS.Extractor.Enums;

namespace Theradex.ODS.Extractor.Models
{
    public class ExtractorInput
    {
        public string TableName { get; set; } = string.Empty;
        //public DateTime StartDate { get; set; } = new DateTime(2005, 1, 1);
        //public DateTime EndDate { get; set; } = new DateTime(2005, 12, 31);
        public int NoOfRecords { get; set; } //= 50000;
        public ExtractorTypeEnum ExtractorType { get; set; } //= ExtractorTypeEnum.ODSExtractor; // Replace DefaultValue with your desired default value
    }
}
