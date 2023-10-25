using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.Rave.Medidata.Models.Configuration
{
    public class AppSettings
    {
        public string CurrentArchiveFolder { get; set; }

        public string TraceId { get; set; }

        public string ArchiveBucket { get; set; }
    }
}