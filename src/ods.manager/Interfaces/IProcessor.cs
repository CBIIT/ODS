using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theradex.ODS.Manager.Models;
using Theradex.ODS.Manager.Models.Configuration;

namespace Theradex.ODS.Manager.Interfaces
{
    public interface IProcessor
    {
        Task<bool> ProcessAsync(ExtractorInput exInput);
    }
}
