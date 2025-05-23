﻿using Extension.Methods;
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
    public class BatchRunControlRepository : IBatchRunControlRepository<BatchRunControl>
    {
        private readonly ILogger<MedidataRWSService> _logger;
        private readonly AppSettings _appSettings;
        private readonly ODSSettings _odsSettings;
        protected readonly IAWSCoreHelper _awsCoreHelper;
        private readonly string defalutConnectionString = "Host={0};Username={1};Password={2};Database={3};Port={4};Pooling=true;Minimum Pool Size=0;Minimum Pool Size=100;Connection Lifetime=0 ";
        private readonly string CONNECTION_STRING;

        public BatchRunControlRepository(ILogger<MedidataRWSService> logger, IOptions<AppSettings> appOptions, IOptions<ODSSettings> odsOptions, IAWSCoreHelper awsCoreHelper)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _odsSettings = odsOptions.Value;
            _awsCoreHelper = awsCoreHelper;

            CONNECTION_STRING = string.Format(defalutConnectionString, _odsSettings.Host, _odsSettings.Username, _odsSettings.Password, _odsSettings.Database, _odsSettings.Port);
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
                        command.CommandText = @"INSERT INTO batch_run_control (
                        table_name, api_startdate, api_enddate, slot, no_of_records, url_to_pull_data, 
                        rave_username, rave_password, is_run_complete_flag, job_starttime, job_endtime, 
                        url_used_to_get_interval, created, updated, error_message, no_of_records_retrieved, 
                        rave_data_url, http_status_code, success, no_of_retry, next_retry_time, payload)
                        VALUES (
                        @table_name, @api_startdate, @api_enddate, @slot, @no_of_records, @url_to_pull_data, 
                        @rave_username, @rave_password, @is_run_complete_flag, @job_starttime, @job_endtime, 
                        @url_used_to_get_interval, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @error_message, 
                        @no_of_records_retrieved, @rave_data_url, @http_status_code, @success, 
                        @no_of_retry, @next_retry_time, @payload)";

                        command.Parameters.AddWithValue("@table_name", entity.TableName);
                        command.Parameters.AddWithValue("@api_startdate", DBUtils.ToDB(entity.ApiStartDate));
                        command.Parameters.AddWithValue("@api_enddate", DBUtils.ToDB(entity.ApiEndDate));
                        command.Parameters.AddWithValue("@slot", DBUtils.ToDB(entity.Slot));
                        command.Parameters.AddWithValue("@no_of_records", DBUtils.ToDB(entity.NoOfRecords));
                        command.Parameters.AddWithValue("@url_to_pull_data", DBUtils.ToDB(entity.UrlToPullData));
                        command.Parameters.AddWithValue("@rave_username", DBUtils.ToDB(entity.RaveUsername));
                        command.Parameters.AddWithValue("@rave_password", DBUtils.ToDB(entity.RavePassword));
                        command.Parameters.AddWithValue("@is_run_complete_flag", DBUtils.ToDB(entity.IsRunCompleteFlag));
                        command.Parameters.AddWithValue("@job_starttime", (object)entity.JobStartTime ?? DBNull.Value);
                        command.Parameters.AddWithValue("@job_endtime", (object)entity.JobEndTime ?? DBNull.Value);
                        command.Parameters.AddWithValue("@url_used_to_get_interval", DBUtils.ToDB(entity.UrlUsedToGetInterval));
                        command.Parameters.AddWithValue("@error_message", DBUtils.ToDB(entity.ErrorMessage));
                        command.Parameters.AddWithValue("@no_of_records_retrieved", DBUtils.ToDB(entity.NoOfRecordsRetrieved));
                        command.Parameters.AddWithValue("@rave_data_url", DBUtils.ToDB(entity.RaveDataUrl));
                        command.Parameters.AddWithValue("@http_status_code", DBUtils.ToDB(entity.HttpStatusCode));
                        command.Parameters.AddWithValue("@success", DBUtils.ToDB(entity.Success));
                        command.Parameters.AddWithValue("@no_of_retry", DBUtils.ToDB(entity.NoOfRetry));
                        command.Parameters.AddWithValue("@next_retry_time", (object)entity.NextRetryTime ?? DBNull.Value);
                        command.Parameters.AddWithValue("@payload", DBUtils.ToDB(entity.Payload));

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
                        command.CommandText = @"INSERT INTO batch_run_control (
                        table_name, api_startdate, api_enddate, slot, no_of_records, url_to_pull_data, 
                        rave_username, rave_password, is_run_complete_flag, job_starttime, job_endtime, 
                        url_used_to_get_interval, created, updated, error_message, no_of_records_retrieved, 
                        rave_data_url, http_status_code, success, no_of_retry, next_retry_time, payload)
                        VALUES (
                        @table_name, @api_startdate, @api_enddate, @slot, @no_of_records, @url_to_pull_data, 
                        @rave_username, @rave_password, @is_run_complete_flag, @job_starttime, @job_endtime, 
                        @url_used_to_get_interval, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @error_message, 
                        @no_of_records_retrieved, @rave_data_url, @http_status_code, @success, 
                        @no_of_retry, @next_retry_time, @payload)";

                        command.Parameters.AddWithValue("@table_name", entity.TableName);
                        command.Parameters.AddWithValue("@api_startdate", DBUtils.ToDB(entity.ApiStartDate));
                        command.Parameters.AddWithValue("@api_enddate", DBUtils.ToDB(entity.ApiEndDate));
                        command.Parameters.AddWithValue("@slot", DBUtils.ToDB(entity.Slot));
                        command.Parameters.AddWithValue("@no_of_records", DBUtils.ToDB(entity.NoOfRecords));
                        command.Parameters.AddWithValue("@url_to_pull_data", DBUtils.ToDB(entity.UrlToPullData));
                        command.Parameters.AddWithValue("@rave_username", DBUtils.ToDB(entity.RaveUsername));
                        command.Parameters.AddWithValue("@rave_password", DBUtils.ToDB(entity.RavePassword));
                        command.Parameters.AddWithValue("@is_run_complete_flag", DBUtils.ToDB(entity.IsRunCompleteFlag));
                        command.Parameters.AddWithValue("@job_starttime", (object)entity.JobStartTime ?? DBNull.Value);
                        command.Parameters.AddWithValue("@job_endtime", (object)entity.JobEndTime ?? DBNull.Value);
                        command.Parameters.AddWithValue("@url_used_to_get_interval", DBUtils.ToDB(entity.UrlUsedToGetInterval));
                        command.Parameters.AddWithValue("@error_message", DBUtils.ToDB(entity.ErrorMessage));
                        command.Parameters.AddWithValue("@no_of_records_retrieved", DBUtils.ToDB(entity.NoOfRecordsRetrieved));
                        command.Parameters.AddWithValue("@rave_data_url", DBUtils.ToDB(entity.RaveDataUrl));
                        command.Parameters.AddWithValue("@http_status_code", DBUtils.ToDB(entity.HttpStatusCode));
                        command.Parameters.AddWithValue("@success", DBUtils.ToDB(entity.Success));
                        command.Parameters.AddWithValue("@no_of_retry", DBUtils.ToDB(entity.NoOfRetry));
                        command.Parameters.AddWithValue("@next_retry_time", (object)entity.NextRetryTime ?? DBNull.Value);
                        command.Parameters.AddWithValue("@payload", DBUtils.ToDB(entity.Payload));

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
                                command.CommandText = @"INSERT INTO batch_run_control (
                        table_name, api_startdate, api_enddate, slot, no_of_records, url_to_pull_data, 
                        rave_username, rave_password, is_run_complete_flag, job_starttime, job_endtime, 
                        url_used_to_get_interval, created, updated, error_message, no_of_records_retrieved, 
                        rave_data_url, http_status_code, success, no_of_retry, next_retry_time, payload)
                        VALUES (
                        @table_name, @api_startdate, @api_enddate, @slot, @no_of_records, @url_to_pull_data, 
                        @rave_username, @rave_password, @is_run_complete_flag, @job_starttime, @job_endtime, 
                        @url_used_to_get_interval, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @error_message, 
                        @no_of_records_retrieved, @rave_data_url, @http_status_code, @success, 
                        @no_of_retry, @next_retry_time, @payload)";

                                command.Parameters.AddWithValue("@table_name", entity.TableName);
                                command.Parameters.AddWithValue("@api_startdate", DBUtils.ToDB(entity.ApiStartDate));
                                command.Parameters.AddWithValue("@api_enddate", DBUtils.ToDB(entity.ApiEndDate));
                                command.Parameters.AddWithValue("@slot", DBUtils.ToDB(entity.Slot));
                                command.Parameters.AddWithValue("@no_of_records", DBUtils.ToDB(entity.NoOfRecords));
                                command.Parameters.AddWithValue("@url_to_pull_data", DBUtils.ToDB(entity.UrlToPullData));
                                command.Parameters.AddWithValue("@rave_username", DBUtils.ToDB(entity.RaveUsername));
                                command.Parameters.AddWithValue("@rave_password", DBUtils.ToDB(entity.RavePassword));
                                command.Parameters.AddWithValue("@is_run_complete_flag", DBUtils.ToDB(entity.IsRunCompleteFlag));
                                command.Parameters.AddWithValue("@job_starttime", (object)entity.JobStartTime ?? DBNull.Value);
                                command.Parameters.AddWithValue("@job_endtime", (object)entity.JobEndTime ?? DBNull.Value);
                                command.Parameters.AddWithValue("@url_used_to_get_interval", DBUtils.ToDB(entity.UrlUsedToGetInterval));
                                command.Parameters.AddWithValue("@error_message", DBUtils.ToDB(entity.ErrorMessage));
                                command.Parameters.AddWithValue("@no_of_records_retrieved", DBUtils.ToDB(entity.NoOfRecordsRetrieved));
                                command.Parameters.AddWithValue("@rave_data_url", DBUtils.ToDB(entity.RaveDataUrl));
                                command.Parameters.AddWithValue("@http_status_code", DBUtils.ToDB(entity.HttpStatusCode));
                                command.Parameters.AddWithValue("@success", DBUtils.ToDB(entity.Success));
                                command.Parameters.AddWithValue("@no_of_retry", DBUtils.ToDB(entity.NoOfRetry));
                                command.Parameters.AddWithValue("@next_retry_time", (object)entity.NextRetryTime ?? DBNull.Value);
                                command.Parameters.AddWithValue("@payload", DBUtils.ToDB(entity.Payload));

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
                                command.CommandText = @"INSERT INTO batch_run_control (
                        table_name, api_startdate, api_enddate, slot, no_of_records, url_to_pull_data, 
                        rave_username, rave_password, is_run_complete_flag, job_starttime, job_endtime, 
                        url_used_to_get_interval, created, updated, error_message, no_of_records_retrieved, 
                        rave_data_url, http_status_code, success, no_of_retry, next_retry_time, payload)
                        VALUES (
                        @table_name, @api_startdate, @api_enddate, @slot, @no_of_records, @url_to_pull_data, 
                        @rave_username, @rave_password, @is_run_complete_flag, @job_starttime, @job_endtime, 
                        @url_used_to_get_interval, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @error_message, 
                        @no_of_records_retrieved, @rave_data_url, @http_status_code, @success, 
                        @no_of_retry, @next_retry_time, @payload)";

                                command.Parameters.AddWithValue("@table_name", entity.TableName);
                                command.Parameters.AddWithValue("@api_startdate", DBUtils.ToDB(entity.ApiStartDate));
                                command.Parameters.AddWithValue("@api_enddate", DBUtils.ToDB(entity.ApiEndDate));
                                command.Parameters.AddWithValue("@slot", DBUtils.ToDB(entity.Slot));
                                command.Parameters.AddWithValue("@no_of_records", DBUtils.ToDB(entity.NoOfRecords));
                                command.Parameters.AddWithValue("@url_to_pull_data", DBUtils.ToDB(entity.UrlToPullData));
                                command.Parameters.AddWithValue("@rave_username", DBUtils.ToDB(entity.RaveUsername));
                                command.Parameters.AddWithValue("@rave_password", DBUtils.ToDB(entity.RavePassword));
                                command.Parameters.AddWithValue("@is_run_complete_flag", DBUtils.ToDB(entity.IsRunCompleteFlag));
                                command.Parameters.AddWithValue("@job_starttime", DBNull.Value);
                                command.Parameters.AddWithValue("@job_endtime", DBNull.Value);
                                command.Parameters.AddWithValue("@url_used_to_get_interval", DBUtils.ToDB(entity.UrlUsedToGetInterval));
                                command.Parameters.AddWithValue("@error_message", DBUtils.ToDB(entity.ErrorMessage));
                                command.Parameters.AddWithValue("@no_of_records_retrieved", DBUtils.ToDB(entity.NoOfRecordsRetrieved));
                                command.Parameters.AddWithValue("@rave_data_url", DBUtils.ToDB(entity.RaveDataUrl));
                                command.Parameters.AddWithValue("@http_status_code", DBUtils.ToDB(entity.HttpStatusCode));
                                command.Parameters.AddWithValue("@success", DBUtils.ToDB(entity.Success));
                                command.Parameters.AddWithValue("@no_of_retry", DBUtils.ToDB(entity.NoOfRetry));
                                command.Parameters.AddWithValue("@next_retry_time", (object)entity.NextRetryTime ?? DBNull.Value);
                                command.Parameters.AddWithValue("@payload", DBUtils.ToDB(entity.Payload));

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
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control where id=@Id ", connection))
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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
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
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control where id=@Id ", connection))
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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
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
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control", connection))
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
                            Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                            JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                            UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                            Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                            Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
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
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control", connection))
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
                            Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                            JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                            UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                            Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                            Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
                        };
                        batchRunControls.Add(batchRunControl);
                    }
                }
            }

            return batchRunControls;
        }

        public void Started(BatchRunControl batchRunControl)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE batch_run_control SET is_run_complete_flag = @IsRunCompleteFlag, job_starttime = @JobStartTime, updated = @Updated WHERE id = @Id", connection))
                {
                    cmd.Parameters.AddWithValue("@IsRunCompleteFlag", true);
                    cmd.Parameters.AddWithValue("@JobStartTime", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Id", batchRunControl.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public async Task StartedAsync(BatchRunControl batchRunControl)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE batch_run_control SET is_run_complete_flag = @IsRunCompleteFlag, job_starttime = @JobStartTime, updated = @Updated WHERE id = @Id", connection))
                {
                    cmd.Parameters.AddWithValue("@IsRunCompleteFlag", true);
                    cmd.Parameters.AddWithValue("@JobStartTime", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Id", batchRunControl.Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public void Completed(BatchRunControl batchRunControl)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE batch_run_control SET is_run_complete_flag = @IsRunCompleteFlag, job_endtime = @JobEndTime, " +
                                                                                            "updated = @Updated , payload = @Payload  , http_status_code = @HttpStatusCode , " +
                                                                                            "rave_data_url = @RaveDataUrl , no_of_records_retrieved = @NoofRecordsRetrieved , " +
                                                                                            "success = @Success , no_of_retry = @NoofRetry, next_retry_time = @NextRetryTime , error_message=@ErrorMessage " +
                                                                                            "WHERE id = @Id", connection))
                {
                    cmd.Parameters.AddWithValue("@IsRunCompleteFlag", false);
                    cmd.Parameters.AddWithValue("@JobEndTime", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Payload", NpgsqlDbType.Jsonb, batchRunControl.Payload);
                    cmd.Parameters.AddWithValue("@HttpStatusCode", batchRunControl.HttpStatusCode);
                    cmd.Parameters.AddWithValue("@RaveDataUrl", batchRunControl.RaveDataUrl);
                    cmd.Parameters.AddWithValue("@NoofRecordsRetrieved", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Success", batchRunControl.Success);
                    cmd.Parameters.AddWithValue("@NoofRetry", batchRunControl.NoOfRetry);
                    cmd.Parameters.AddWithValue("@NextRetryTime", batchRunControl.NextRetryTime != null ? (object)batchRunControl.NextRetryTime : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);

                    cmd.Parameters.AddWithValue("@Id", batchRunControl.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public async Task CompletedAsync(BatchRunControl batchRunControl)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE batch_run_control SET is_run_complete_flag = @IsRunCompleteFlag, job_endtime = @JobEndTime, " +
                                                                                            "updated = @Updated , payload = @Payload  , http_status_code = @HttpStatusCode , " +
                                                                                            "rave_data_url = @RaveDataUrl , no_of_records_retrieved = @NoofRecordsRetrieved , " +
                                                                                            "success = @Success , no_of_retry = @NoofRetry, next_retry_time = @NextRetryTime , error_message=@ErrorMessage " +
                                                                                            "WHERE id = @Id", connection))
                {
                    cmd.Parameters.AddWithValue("@IsRunCompleteFlag", false);
                    cmd.Parameters.AddWithValue("@JobEndTime", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Payload", NpgsqlDbType.Jsonb, batchRunControl.Payload);
                    cmd.Parameters.AddWithValue("@HttpStatusCode", batchRunControl.HttpStatusCode);
                    cmd.Parameters.AddWithValue("@RaveDataUrl", batchRunControl.RaveDataUrl);
                    cmd.Parameters.AddWithValue("@NoofRecordsRetrieved", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Success", batchRunControl.Success);
                    cmd.Parameters.AddWithValue("@NoofRetry", batchRunControl.NoOfRetry);
                    cmd.Parameters.AddWithValue("@NextRetryTime", batchRunControl.NextRetryTime != null ? (object)batchRunControl.NextRetryTime : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);

                    cmd.Parameters.AddWithValue("@Id", batchRunControl.Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public void CompletedError(BatchRunControl batchRunControl, string error)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE batch_run_control SET is_run_complete_flag = @IsRunCompleteFlag, job_endtime = @JobEndTime, " +
                                                                                            "updated = @Updated , payload = @Payload  , http_status_code = @HttpStatusCode , " +
                                                                                            "rave_data_url = @RaveDataUrl , no_of_records_retrieved = @NoofRecordsRetrieved , " +
                                                                                            "success = @Success , no_of_retry = @NoofRetry, next_retry_time = @NextRetryTime , error_message=@ErrorMessage " +
                                                                                            "WHERE id = @Id", connection))
                {
                    cmd.Parameters.AddWithValue("@IsRunCompleteFlag", false);
                    cmd.Parameters.AddWithValue("@JobEndTime", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Payload", NpgsqlDbType.Jsonb, batchRunControl.Payload);
                    cmd.Parameters.AddWithValue("@HttpStatusCode", batchRunControl.HttpStatusCode);
                    cmd.Parameters.AddWithValue("@RaveDataUrl", batchRunControl.RaveDataUrl);
                    cmd.Parameters.AddWithValue("@NoofRecordsRetrieved", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Success", batchRunControl.Success);
                    cmd.Parameters.AddWithValue("@NoofRetry", batchRunControl.NoOfRetry);
                    cmd.Parameters.AddWithValue("@NextRetryTime", batchRunControl.NextRetryTime != null ? (object)batchRunControl.NextRetryTime : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ErrorMessage", error);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public async Task CompletedErrorAsync(BatchRunControl batchRunControl, string error)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE batch_run_control SET is_run_complete_flag = @IsRunCompleteFlag, job_endtime = @JobEndTime, " +
                                                                                            "updated = @Updated , payload = @Payload  , http_status_code = @HttpStatusCode , " +
                                                                                            "rave_data_url = @RaveDataUrl , no_of_records_retrieved = @NoofRecordsRetrieved , " +
                                                                                            "success = @Success , no_of_retry = @NoofRetry, next_retry_time = @NextRetryTime , error_message=@ErrorMessage " +
                                                                                            "WHERE id = @Id", connection))
                {
                    cmd.Parameters.AddWithValue("@IsRunCompleteFlag", false);
                    cmd.Parameters.AddWithValue("@JobEndTime", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Payload", NpgsqlDbType.Jsonb, batchRunControl.Payload);
                    cmd.Parameters.AddWithValue("@HttpStatusCode", batchRunControl.HttpStatusCode);
                    cmd.Parameters.AddWithValue("@RaveDataUrl", batchRunControl.RaveDataUrl);
                    cmd.Parameters.AddWithValue("@NoofRecordsRetrieved", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Success", batchRunControl.Success);
                    cmd.Parameters.AddWithValue("@NoofRetry", batchRunControl.NoOfRetry);
                    cmd.Parameters.AddWithValue("@NextRetryTime", batchRunControl.NextRetryTime != null ? (object)batchRunControl.NextRetryTime : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ErrorMessage", error);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public BatchRunControl GetNextBatch(string tableName)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control WHERE table_name = @TableName AND is_run_complete_flag = 'false' and job_starttime is null and job_endtime is null  ORDER BY api_startdate::timestamp", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);

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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
                            };
                            batchRunControls.Add(batchRunControl);
                        }
                    }
                }
            }

            return batchRunControls.FirstOrDefault();
        }

        public async Task<BatchRunControl> GetNextBatchAsync(string tableName)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control WHERE table_name = @TableName AND is_run_complete_flag = 'false' and job_starttime is null and job_endtime is null  ORDER BY api_startdate::timestamp", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);

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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
                            };
                            batchRunControls.Add(batchRunControl);
                        }
                    }
                }
            }

            return batchRunControls.FirstOrDefault();
        }
        public BatchRunControl GetByTableNameAndId(string tableName, int Id)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control WHERE table_name = @TableName AND is_run_complete_flag = 'false'  AND job_starttime is null AND job_endtime is null AND id > @Id ORDER BY api_startdate::timestamp", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    cmd.Parameters.AddWithValue("@Id", Id);

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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
                            };
                            batchRunControls.Add(batchRunControl);
                        }
                    }
                }
            }

            return batchRunControls.FirstOrDefault();
        }
        public async Task<BatchRunControl> GetByTableNameAndIdAsync(string tableName, int Id)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control WHERE table_name = @TableName AND is_run_complete_flag = 'false'  AND job_starttime is null AND job_endtime is null AND id > @Id ORDER BY api_startdate::timestamp", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    cmd.Parameters.AddWithValue("@Id", Id);

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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
                            };
                            batchRunControls.Add(batchRunControl);
                        }
                    }
                }
            }

            return batchRunControls.FirstOrDefault();
        }


        public async Task<List<BatchRunControl>> HasActiveJobsAsync(string tableName)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM batch_run_control WHERE table_name = @TableName AND job_starttime IS NOT NULL AND job_endtime IS NULL AND is_run_complete_flag = 'true'", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName.ToUpper());
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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
                            };
                            batchRunControls.Add(batchRunControl);
                        }
                    }
                }
            }
            return batchRunControls;
        }
        public List<BatchRunControl> HasActiveJobs(string tableName)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM batch_run_control WHERE table_name = @TableName AND job_starttime IS NOT NULL AND job_endtime IS NULL AND is_run_complete_flag = 'true'", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName.ToUpper());
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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
                            };
                            batchRunControls.Add(batchRunControl);
                        }
                    }
                }
            }
            return batchRunControls;
        }

        public BatchRunControl GetByLastExtractionInfo(string tableName)
        {
            throw new NotImplementedException();
        }

        public async Task<BatchRunControl> GetByLastExtractionInfoAsync(string tableName)
        {
            List<BatchRunControl> batchRunControls = new List<BatchRunControl>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.batch_run_control WHERE table_name = @TableName  ORDER BY api_startdate::timestamp desc", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);

                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            BatchRunControl batchRunControl = new BatchRunControl
                            {
                                Id = reader.GetInt32("id"),
                                TableName = reader.GetString("table_name"),
                                ApiStartDate = reader.GetValueOrDefault<string>("api_startdate"),
                                ApiEndDate = reader.GetValueOrDefault<string>("api_enddate"),
                                Slot = reader.GetValueOrDefault<string>("slot"),
                                NoOfRecords = reader.GetValueOrDefault<int>("no_of_records"),
                                UrlToPullData = reader.GetValueOrDefault<string>("url_to_pull_data"),
                                RaveUsername = reader.GetValueOrDefault<string>("rave_username"),
                                RavePassword = reader.GetValueOrDefault<string>("rave_password"),
                                IsRunCompleteFlag = reader.GetValueOrDefault<string>("is_run_complete_flag"),
                                JobStartTime = reader.GetValueOrDefault<DateTime?>("job_starttime"),
                                JobEndTime = reader.GetValueOrDefault<DateTime?>("job_endtime"),
                                UrlUsedToGetInterval = reader.GetValueOrDefault<string>("url_used_to_get_interval"),
                                Created = reader.GetValueOrDefault<DateTime>("created"),
                                Updated = reader.GetValueOrDefault<DateTime>("updated"),
                                ErrorMessage = reader.GetValueOrDefault<string>("error_message"),
                                NoOfRecordsRetrieved = reader.GetValueOrDefault<int>("no_of_records_retrieved"),
                                RaveDataUrl = reader.GetValueOrDefault<string>("rave_data_url"),
                                HttpStatusCode = reader.GetValueOrDefault<string>("http_status_code"),
                                Success = reader.GetValueOrDefault<string>("success"),
                                NoOfRetry = reader.GetValueOrDefault<int>("no_of_retry"),
                                NextRetryTime = reader.GetValueOrDefault<DateTime>("next_retry_time"),
                                Payload = reader.GetValueOrDefault<string>("payload"),
                                ExtractedFileName = reader.GetValueOrDefault<string>("extracted_file_name")
                            };
                            batchRunControls.Add(batchRunControl);
                        }
                    }
                }
            }
            return batchRunControls.FirstOrDefault();

        }

        public BatchRunControl GetByLastExtractionInfoAfter(string tableName, DateTime input)
        {
            throw new NotImplementedException();
        }

        public async Task<BatchRunControl> GetByLastExtractionInfoAfterAsync(string tableName, DateTime input)
        {
            throw new NotImplementedException();
        }

        public BatchRunControl GetByMaxApiEndDate(string tableName)
        {
            BatchRunControl batchRunControl = null;

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM batch_run_control WHERE table_name = @TableName ORDER BY api_enddate::timestamp DESC LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName.ToUpper());
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
                                Slot = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                NoOfRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                UrlToPullData = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                RaveUsername = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RavePassword = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                IsRunCompleteFlag = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                JobStartTime = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10),
                                JobEndTime = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                UrlUsedToGetInterval = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                Created = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                                Updated = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
                            };

                        }
                    }
                }
            }
            return batchRunControl;
        }

        public async Task<BatchRunControl> GetByMaxApiEndDateAsync(string tableName)
        {
            BatchRunControl batchRunControl = null;

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                
                string query = @"SELECT *
                                        FROM batch_run_control
                                        WHERE ID = (WITH OrderedRuns AS (SELECT id,
                                                                                CAST(api_startdate AS TIMESTAMP)                         AS api_startdate,
                                                                                CAST(api_enddate AS TIMESTAMP)                           AS api_enddate,
                                                                                LAG(CAST(api_enddate AS TIMESTAMP))
                                                                                OVER (ORDER BY CAST(api_startdate AS TIMESTAMP))         AS previous_api_enddate,
                                                                                LAG(id) OVER (ORDER BY CAST(api_startdate AS TIMESTAMP)) AS previous_id
                                                                         FROM batch_run_control
                                                                         WHERE table_name = 'DATAPOINTS'
                                                                           AND CAST(api_startdate AS TIMESTAMP) >= CURRENT_DATE - INTERVAL '1000 days')
                                                    SELECT previous_id
                                                    FROM OrderedRuns
                                                    WHERE api_startdate IS NOT NULL
                                                      AND previous_api_enddate IS NOT NULL
                                                      AND api_startdate > previous_api_enddate + INTERVAL '1 day'
                                                    ORDER BY api_startdate DESC LIMIT 1)
                                    ";
                query =
                    @"SELECT * FROM batch_run_control WHERE table_name = @TableName ORDER BY api_enddate::timestamp DESC LIMIT 1";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName.ToUpper());
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            // Instantiate the object
                            batchRunControl = new BatchRunControl();

                            batchRunControl.Id = reader.GetValueOrDefault(0, 0);
                            batchRunControl.TableName = reader.GetValueOrDefault(1, string.Empty);
                            batchRunControl.ApiStartDate = reader.GetValueOrDefault(2, string.Empty);
                            batchRunControl.ApiEndDate = reader.GetValueOrDefault(3, string.Empty);
                            batchRunControl.Slot = reader.GetValueOrDefault(4, string.Empty);
                            batchRunControl.NoOfRecords = reader.GetValueOrDefault(5, 0);
                            batchRunControl.UrlToPullData = reader.GetValueOrDefault(6, string.Empty);
                            batchRunControl.RaveUsername = reader.GetValueOrDefault(7, string.Empty);
                            batchRunControl.RavePassword = reader.GetValueOrDefault(8, string.Empty);
                            batchRunControl.IsRunCompleteFlag = reader.GetValueOrDefault(9, string.Empty);
                            batchRunControl.JobStartTime = reader.GetValueOrDefault(10, DateTime.MinValue);
                            batchRunControl.JobEndTime = reader.GetValueOrDefault(11, DateTime.MinValue);
                            batchRunControl.UrlUsedToGetInterval = reader.GetValueOrDefault(12, string.Empty);
                            batchRunControl.Created = reader.GetValueOrDefault(13, DateTime.MinValue);
                            batchRunControl.Updated = reader.GetValueOrDefault(14, DateTime.MinValue);


                        }
                    }
                }
            }
            return batchRunControl;
        }

    }
}