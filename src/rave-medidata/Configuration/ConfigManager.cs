using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.Rave.Medidata.Configuration
{
    public class ConfigManager: IConfigManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigManager> _logger;

        public ConfigManager(IConfiguration configuration, ILogger<ConfigManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string ArchiveBucket => _configuration["ArchiveBucket"];
    }
}
