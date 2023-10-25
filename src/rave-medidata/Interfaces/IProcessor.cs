using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theradex.Rave.Medidata.Models;
using Theradex.Rave.Medidata.Models.Configuration;

namespace Theradex.Rave.Medidata.Interfaces
{
    public interface IProcessor
    {
        Task<bool> ProcessAsync(ExtractorInput exInput);
    }
}
