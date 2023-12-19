using Extension.Methods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static ServiceStack.LicenseUtils;
using Theradex.ODS.Manager.Interfaces;
using Theradex.ODS.Models;
using Theradex.ODS.Manager.Helpers.Extensions;
using Theradex.ODS.Manager.Services;
using Theradex.ODS.Manager.Models.Configuration;

namespace Theradex.ODS.Manager.Repositories
{
    public class ManagerTableInfoRepository : IManagerTableInfoRepository<BatchRunControl>
    {
        private readonly ILogger<MedidataRWSService> _logger;
        private readonly AppSettings _appSettings;
        private readonly ODSSettings _odsSettings;
        protected readonly IAWSCoreHelper _awsCoreHelper;
        private readonly string defalutConnectionString = "Host={0};Username={1};Password={2};Database={3};Port={4};Pooling=true;Minimum Pool Size=0;Minimum Pool Size=100;Connection Lifetime=0 ";
        private readonly string CONNECTION_STRING;

        public ManagerTableInfoRepository(ILogger<MedidataRWSService> logger, IOptions<AppSettings> appOptions, IOptions<ODSSettings> odsOptions, IAWSCoreHelper awsCoreHelper)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _odsSettings = odsOptions.Value;
            _awsCoreHelper = awsCoreHelper;

            CONNECTION_STRING = string.Format(defalutConnectionString, _odsSettings.Host, _odsSettings.Username, _odsSettings.Password, _odsSettings.Database, _odsSettings.Port);
            //CONNECTION_STRING = "Host=localhost;Username=postgres;Password=docker;Database=orders;Port=5432;Pooling=true;Minimum Pool Size=0;Minimum Pool Size=100;Connection Lifetime=0 ";
            //CONNECTION_STRING = "Host=localhost;Username=postgres;Password=docker;Database=ods;Port=5432;Pooling=true;Minimum Pool Size=0;Minimum Pool Size=100;Connection Lifetime=0 ";

            _logger.LogInformation($"CONNECTION_STRING: {CONNECTION_STRING}");
        }

        public void Add(BatchRunControl entity)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    try
                    {
                        command.Connection = connection;
                        command.CommandText = @"INSERT INTO odsmanager_table_matadata (table_name, api_startdate, api_enddate, min_date, max_date, table_row_count, payload)
                                                                                       VALUES (@table_name, @api_startdate, @api_enddate, @min_date, @max_date, @table_row_count, @payload)";

                        command.Parameters.AddWithValue("@table_name", entity.TableName);
                        command.Parameters.AddWithValue("@api_startdate", DBUtils.ToDB(entity.ApiStartDate));
                        command.Parameters.AddWithValue("@api_enddate", DBUtils.ToDB(entity.ApiEndDate));

                        if (entity.Payloads.Payload != null && entity.Payloads.Payload.Count == 1)
                        {
                            command.Parameters.AddWithValue("@table_row_count", DBUtils.ToDB(entity.Payloads.Payload.First().TableRowCount));

                            var isValid = DateTime.TryParse(entity.Payloads.Payload.First().MinDate, out DateTime minDate1);

                            if (isValid) command.Parameters.AddWithValue("@min_date", (object)minDate1 ?? DBNull.Value);
                            else command.Parameters.AddWithValue("@min_date", DBNull.Value);

                            isValid = DateTime.TryParse(entity.Payloads.Payload.First().MaxDate, out DateTime maxDate1);

                            if (isValid) command.Parameters.AddWithValue("@max_date", (object)maxDate1 ?? DBNull.Value);
                            else command.Parameters.AddWithValue("@max_date", DBNull.Value);

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@min_date", DBNull.Value);
                            command.Parameters.AddWithValue("@max_date", DBNull.Value);
                            command.Parameters.AddWithValue("@table_row_count", DBNull.Value);
                        }
                        command.Parameters.AddWithValue("@payload", NpgsqlDbType.Jsonb, DBUtils.ToDB(entity.Payload));

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in Saving");
                    }
                }
            }
        }
        public async Task AddAsync(BatchRunControl entity)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    try
                    {
                        command.Connection = connection;
                        command.CommandText = @"INSERT INTO odsmanager_table_matadata (table_name, api_startdate, api_enddate, min_date, max_date, table_row_count, payload)
                                                                                       VALUES (@table_name, @api_startdate, @api_enddate, @min_date, @max_date, @table_row_count, @payload)";

                        command.Parameters.AddWithValue("@table_name", entity.TableName);
                        command.Parameters.AddWithValue("@api_startdate", DBUtils.ToDB(entity.ApiStartDate));
                        command.Parameters.AddWithValue("@api_enddate", DBUtils.ToDB(entity.ApiEndDate));

                        if (entity.Payloads.Payload != null && entity.Payloads.Payload.Count == 1)
                        {
                            command.Parameters.AddWithValue("@table_row_count", DBUtils.ToDB(entity.Payloads.Payload.First().TableRowCount));

                            var isValid = DateTime.TryParse(entity.Payloads.Payload.First().MinDate, out DateTime minDate1);

                            if (isValid) command.Parameters.AddWithValue("@min_date", (object)minDate1 ?? DBNull.Value);
                            else command.Parameters.AddWithValue("@min_date", DBNull.Value);

                            isValid = DateTime.TryParse(entity.Payloads.Payload.First().MaxDate, out DateTime maxDate1);

                            if (isValid) command.Parameters.AddWithValue("@max_date", (object)maxDate1 ?? DBNull.Value);
                            else command.Parameters.AddWithValue("@max_date", DBNull.Value);

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@min_date", DBNull.Value);
                            command.Parameters.AddWithValue("@max_date", DBNull.Value);
                            command.Parameters.AddWithValue("@table_row_count", DBNull.Value);
                        }
                        command.Parameters.AddWithValue("@payload", NpgsqlDbType.Jsonb, DBUtils.ToDB(entity.Payload));


                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in Saving");
                    }
                }
            }
        }

        public async Task AddMultipleAsync(List<BatchRunControl> records)
        {
            using (var connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var entity in records)
                        {
                            using (var command = new NpgsqlCommand())
                            {
                                command.Transaction = transaction;
                                command.Connection = connection;
                                command.CommandText = @"INSERT INTO odsmanager_table_matadata (table_name, api_startdate, api_enddate, min_date, max_date, table_row_count, payload)
                                                                                       VALUES (@table_name, @api_startdate, @api_enddate, @min_date, @max_date, @table_row_count, @payload)";

                                command.Parameters.AddWithValue("@table_name", entity.TableName);
                                command.Parameters.AddWithValue("@api_startdate", DBUtils.ToDB(entity.ApiStartDate));
                                command.Parameters.AddWithValue("@api_enddate", DBUtils.ToDB(entity.ApiEndDate));

                                if (entity.Payloads.Payload != null && entity.Payloads.Payload.Count == 1)
                                {
                                    command.Parameters.AddWithValue("@table_row_count", DBUtils.ToDB(entity.Payloads.Payload.First().TableRowCount));

                                    var isValid = DateTime.TryParse(entity.Payloads.Payload.First().MinDate, out DateTime minDate1);

                                    if (isValid) command.Parameters.AddWithValue("@min_date", (object)minDate1 ?? DBNull.Value);
                                    else command.Parameters.AddWithValue("@min_date", DBNull.Value);

                                    isValid = DateTime.TryParse(entity.Payloads.Payload.First().MaxDate, out DateTime maxDate1);

                                    if (isValid) command.Parameters.AddWithValue("@max_date", (object)maxDate1 ?? DBNull.Value);
                                    else command.Parameters.AddWithValue("@max_date", DBNull.Value);

                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@min_date", DBNull.Value);
                                    command.Parameters.AddWithValue("@max_date", DBNull.Value);
                                    command.Parameters.AddWithValue("@table_row_count", DBNull.Value);
                                }
                                command.Parameters.AddWithValue("@payload", NpgsqlDbType.Jsonb, DBUtils.ToDB(entity.Payload));

                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Error in Saving");
                    }
                }
            }
        }

        public void AddMultiple(List<BatchRunControl> records)
        {
            using (var connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var entity in records)
                        {
                            using (var command = new NpgsqlCommand())
                            {
                                command.Transaction = transaction;
                                command.Connection = connection;
                                command.CommandText = @"INSERT INTO odsmanager_table_matadata (table_name, api_startdate, api_enddate, min_date, max_date, table_row_count, payload)
                                                                                       VALUES (@table_name, @api_startdate, @api_enddate, @min_date, @max_date, @table_row_count, @payload)";

                                command.Parameters.AddWithValue("@table_name", entity.TableName);
                                command.Parameters.AddWithValue("@api_startdate", DBUtils.ToDB(entity.ApiStartDate));
                                command.Parameters.AddWithValue("@api_enddate", DBUtils.ToDB(entity.ApiEndDate));

                                if (entity.Payloads.Payload != null && entity.Payloads.Payload.Count == 1)
                                {
                                    command.Parameters.AddWithValue("@table_row_count", DBUtils.ToDB(entity.Payloads.Payload.First().TableRowCount));

                                    var isValid = DateTime.TryParse(entity.Payloads.Payload.First().MinDate, out DateTime minDate1);

                                    if (isValid) command.Parameters.AddWithValue("@min_date", (object)minDate1 ?? DBNull.Value);
                                    else command.Parameters.AddWithValue("@min_date", DBNull.Value);

                                    isValid = DateTime.TryParse(entity.Payloads.Payload.First().MaxDate, out DateTime maxDate1);

                                    if (isValid) command.Parameters.AddWithValue("@max_date", (object)maxDate1 ?? DBNull.Value);
                                    else command.Parameters.AddWithValue("@max_date", DBNull.Value);

                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@min_date", DBNull.Value);
                                    command.Parameters.AddWithValue("@max_date", DBNull.Value);
                                    command.Parameters.AddWithValue("@table_row_count", DBNull.Value);
                                }
                                command.Parameters.AddWithValue("@payload", NpgsqlDbType.Jsonb, DBUtils.ToDB(entity.Payload));

                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Error in Saving");
                    }
                }
            }
        }

        public void Update(BatchRunControl entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(BatchRunControl entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<BatchRunControl, bool>> where)
        {
            throw new NotImplementedException();
        }
        public async Task<BatchRunControl> GetByIdAsync(int id)
        {
            BatchRunControl batchRunControl = null;

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, table_name, api_startdate, api_enddate, min_date, max_date, table_row_count,payload, created, updated FROM public.odsmanager_table_matadata where id=@Id ", connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            batchRunControl = new BatchRunControl
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                TableName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                ApiStartDate = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                ApiEndDate = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                JobStartTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                                JobEndTime = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                                NoOfRecords = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                Payload = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                Created = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                                Updated = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9)
                            };
                        }
                    }
                }
            }

            return batchRunControl;
        }
        public BatchRunControl GetById(int id)
        {
            BatchRunControl batchRunControl = null;

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, table_name, api_startdate, api_enddate, min_date, max_date, table_row_count,payload, created, updated FROM public.odsmanager_table_matadata where id=@Id ", connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            batchRunControl = new BatchRunControl
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                TableName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                ApiStartDate = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                ApiEndDate = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                JobStartTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                                JobEndTime = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                                NoOfRecords = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                Payload = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                Created = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                                Updated = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9)
                            };
                        }
                    }
                }
            }

            return batchRunControl;
        }

        public async Task<IEnumerable<BatchRunControl>> GetAllAsync()
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                //using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, table_name, api_startdate, api_enddate, min_date, max_date, table_row_count,payload, created, updated FROM public.odsmanager_table_matadata  where table_name = 'CONFIGURATION' order by id asc ", connection))
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, table_name, api_startdate, api_enddate, min_date, max_date, table_row_count,payload, created, updated FROM public.odsmanager_table_matadata WHERE min_date is not null order by id asc ", connection))
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        BatchRunControl batchRunControl = new BatchRunControl
                        {

                            Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            TableName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            ApiStartDate = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            ApiEndDate = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            JobStartTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                            JobEndTime = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                            NoOfRecords = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            Payload = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            Created = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                            Updated = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9)
                        };
                        batchRunControls.Add(batchRunControl);
                    }
                }
            }

            return batchRunControls;
        }
        public List<BatchRunControl> GetAll()
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, table_name, api_startdate, api_enddate, min_date, max_date, table_row_count,payload, created, updated FROM public.odsmanager_table_matadata where id=@Id ", connection))
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BatchRunControl batchRunControl = new BatchRunControl
                        {
                            Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            TableName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            ApiStartDate = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            ApiEndDate = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            JobStartTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                            JobEndTime = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                            NoOfRecords = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            Payload = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            Created = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                            Updated = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9)
                        };
                        batchRunControls.Add(batchRunControl);
                    }
                }
            }

            return batchRunControls;
        }

    }
}