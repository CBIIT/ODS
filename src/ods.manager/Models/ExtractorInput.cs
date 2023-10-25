using Theradex.ODS.Manager.Enums;

namespace Theradex.ODS.Manager.Models
{
    public class ExtractorInput
    {
        public string TableName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ExtractorTypeEnum ExtractorType { get; set; }
    }
}
