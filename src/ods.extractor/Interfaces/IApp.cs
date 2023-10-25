using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.ODS.Extractor.Interfaces
{
    public interface IApp
    {
        Task RunAsync(string[] args);
    }
}
