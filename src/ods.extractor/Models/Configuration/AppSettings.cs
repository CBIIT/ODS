using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.ODS.Extractor.Models.Configuration
{
    public class AppSettings
    {
        public string CurrentArchiveFolder { get; set; }

        public string TraceId { get; set; }

        public string ArchiveBucket { get; set; }

        public string S3Enabled { get; set; }
    }
}