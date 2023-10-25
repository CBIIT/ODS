using Amazon.Extensions.NETCore.Setup;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Theradex.Rave.Medidata.Interfaces;
using Theradex.Rave.Medidata.Models.Configuration;
using Theradex.Rave.Medidata.Processors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theradex.Rave.Medidata.Repositories
{
    public class ODSRepository : IODSRepository
    {
        private NpgsqlConnection connection;
        private readonly ODSSettings _odsSetting;
        private readonly ILogger<ODSRepository> _logger;

        public ODSRepository(ILogger<ODSRepository> logger, IOptions<ODSSettings> odsSettings) 
        {
            _logger = logger;

            _odsSetting = odsSettings.Value;

            var CONNECTION_STRING = $"Host={_odsSetting.Host};" +
                   $"Username={_odsSetting.Username};" +
                   $"Password={_odsSetting.Password};" +
                   $"Database=postgres";

            connection = new NpgsqlConnection(CONNECTION_STRING);

            connection.Open();
        }

        public async Task Update()
        {
            var commandText = $@"UPDATE ods.batch_run_control
                                SET updated = @updated
                                WHERE id = @id";

            var queryArgs = new
            {
                id = 690,
                updated = DateTime.Now               
            };

            try
            {
                await connection.ExecuteAsync(commandText, queryArgs);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error in the update.", ex);
            }            
        }
    }
}
