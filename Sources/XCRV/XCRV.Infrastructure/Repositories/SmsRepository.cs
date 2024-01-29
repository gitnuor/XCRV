using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.OracleInfrastructure.Repositories;

namespace XCRV.Infrastructure.Repositories
{
    public class SmsRepository : ISmsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SmsRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.SqlSMSConnection);
        }


        public async Task<IEnumerable<SmsPush>> GetTopTenPushSMS(string mobileNo, string schmCode)
        {
            try
            {
                var sql = DatabaseProcedure.BblSmsProcedure.SP_XCRV360V2_GetTopTenPushSMS;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@mobileNo", mobileNo);
                    parameters.Add("@schmCode", schmCode);
                    var result = await connection.QueryAsync<SmsPush>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<SmsPull>> GetTopTenPullSMS(string mobileNo, string schmCode)
        {
            try
            {
                var sql = DatabaseProcedure.BblSmsProcedure.SP_XCRV360V2_GetTopTenPullSMS;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@mobileNo", mobileNo);
                    parameters.Add("@schmCode", schmCode);
                    var result = await connection.QueryAsync<SmsPull>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IEnumerable<SmsPull>> GetSMSLog(string mobileNo, string schmCode)
        {
            try
            {
                var sql = DatabaseProcedure.BblSmsProcedure.SP_XCRV360V2_GetUserRequestLogFinacle;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@mobileNo", mobileNo);
                    parameters.Add("@schmCode", schmCode);
                    var result = await connection.QueryAsync<SmsPull>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
