using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theradex.ODS.Extractor.Models;
using Theradex.ODS.Extractor.Models.Configuration;

namespace Theradex.ODS.Extractor.Interfaces
{
    public interface IProcessor
    {
        Task<bool> ProcessAsync(ExtractorInput exInput);
    }
}
