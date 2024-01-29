using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.OracleInfrastructure.Repositories;

namespace XCRV.Infrastructure.Repositories
{
    public class MbsRepository : IMbsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _mbsHoConnectionString;
        private readonly string _sqlMBSConnectionArchive;

        public MbsRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.SqlMBSConnectionBranch);
            this._mbsHoConnectionString = _configuration.GetConnectionString(DatabaseConnection.SqlMBSConnectionHO);
            this._sqlMBSConnectionArchive = _configuration.GetConnectionString(DatabaseConnection.SqlMBSConnectionArchive);
        }

        public async Task<IEnumerable<MbsBufferCharge>> GetBufferChargeAmount(string acno)
        {
            try
            {
                var sql = DatabaseProcedure.MbsProcedure.SP_GetBufferChargeAmount;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@acNo", acno);
                    var result = await connection.QueryAsync<MbsBufferCharge>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<MbsBufferInterest>> GetBufferInterestRelated(string acno)
        {
            try
            {
                var sql = DatabaseProcedure.MbsProcedure.SP_GetBufferInterestRelated;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@acNo", acno);
                    var result = await connection.QueryAsync<MbsBufferInterest>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<MbsAccountInfo>> GetMbsStatementAccountName(string acno)
        {
            try
            {
                var sql = DatabaseProcedure.MbsProcedure.SP_Statement_AccountName;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ACN", acno);
                    var result = await connection.QueryAsync<MbsAccountInfo>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<MbsAccountInfo>> GetMbsHoStatementAccountName(string acno)
        {
            try
            {
                var sql = DatabaseProcedure.MbsProcedure.SP_Statement_AccountName_Voucher;
                using (var connection = new SqlConnection(_mbsHoConnectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ACN", acno);
                    var result = await connection.QueryAsync<MbsAccountInfo>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IEnumerable<MbsTransaction>> GetMbsTransaction(string acno, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var sql = DatabaseProcedure.MbsProcedure.SP_Statement_Voucher;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ACN", acno);
                    parameters.Add("@fromDate", fromDate);
                    parameters.Add("@toDate", toDate);
                    var result = await connection.QueryAsync<MbsTransaction>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<MbsTransaction>> GetMbsHoTransaction(string acno, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var sql = DatabaseProcedure.MbsProcedure.SP_Statement_Voucher;
                using (var connection = new SqlConnection(_mbsHoConnectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ACN", acno);
                    parameters.Add("@fromDate", fromDate);
                    parameters.Add("@toDate", toDate);
                    var result = await connection.QueryAsync<MbsTransaction>(sql, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<FinacleMbsMapping> GetMbsAccountMapping(string acno)
        {
            try
            {
                var sql = string.Empty;
                if(acno.Length == 12)
                {
                    sql = "select FINAccno,MBSAccno from Account_mapping where MBSAccno=@accNo";
                }
                else
                {
                    sql = "select FINAccno,MBSAccno  from Account_mapping where FINAccno =@accNo";
                }
                
                using (var connection = new SqlConnection(_sqlMBSConnectionArchive))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@accNo", acno);
                    var result = await connection.QueryAsync<FinacleMbsMapping>(sql, parameters, commandType: CommandType.Text);
                    connection.Close();
                    return result.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
