using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.ODS.Manager.Models.Configuration
{
    public class AppSettings
    {
        public string LocalArchivePath { get; set; }

        public string TraceId { get; set; }

        public string Env { get; set; }

        public string ArchiveBucket { get; set; }
    }
}