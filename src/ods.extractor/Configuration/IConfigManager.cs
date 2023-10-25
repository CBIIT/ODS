using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.ODS.Extractor.Configuration
{
    public interface IConfigManager
    {
        public string ArchiveBucket { get; }
    }
}
