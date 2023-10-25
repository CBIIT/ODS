using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.ODS.Manager.Configuration
{
    public interface IConfigManager
    {
        public string ArchiveBucket { get; }
    }
}
