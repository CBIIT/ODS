using Theradex.ODS.Extractor.Enums;

namespace Theradex.ODS.Extractor.Models
{
    public class ExtractorInput
    {
        public string TableName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Count { get; set; }

        public ExtractorTypeEnum ExtractorType { get; set; }
    }
}
