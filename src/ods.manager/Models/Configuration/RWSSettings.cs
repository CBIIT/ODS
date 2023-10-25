using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.ODS.Manager.Models.Configuration
{
    public class RWSSettings
    {
        public string RWSServer { get; set; }

        public string RWSUserName { get; set; }

        public string RWSPassword { get; set; }

        public int TimeoutInSecs { get; set; }

    }
}
