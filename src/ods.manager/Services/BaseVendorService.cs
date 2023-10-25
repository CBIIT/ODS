using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Manager.Models.Configuration;

namespace Theradex.ODS.Manager.Services
{
    public class BaseVendorService
    {
        protected readonly ILogger<BaseVendorService> _logger;
        protected readonly AppSettings _appSettings;
        protected readonly IAWSCoreHelper _awsCoreHelper;

        public BaseVendorService(ILogger<BaseVendorService> logger, IOptions<AppSettings> appOptions, IAWSCoreHelper awsCoreHelper)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _awsCoreHelper = awsCoreHelper;
        }

        protected void WriteToFile(string fileName, string content)
        {
            StreamWriter writer = new StreamWriter(fileName);
            writer.Write(content);
            writer.Close();
        }
    }
}