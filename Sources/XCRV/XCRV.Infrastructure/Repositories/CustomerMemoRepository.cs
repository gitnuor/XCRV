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
    public class CustomerMemoRepository : ICustomerMemoRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        
        public CustomerMemoRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.SqlCRMConnection);
        }

        public async Task<IReadOnlyList<CustomerMemo>> GetCustomerMemosByCif(string cif)
        {
            try
            {
                var sql = DatabaseProcedure.CspmProcedure.XCRV360_V2_CustomerMemoGetByCustomeID;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@CustomerID", cif);
                    var result = await connection.QueryAsync<CustomerMemo>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result.ToList();
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertCustomerMemo(string MemoText, string UserID, string CustomerID)
        {
            try
            {
                var sql = DatabaseProcedure.CspmProcedure.XCRV360_V2_CustomerMemoInsert;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@MemoText", MemoText);
                    parameters.Add("@UserID", UserID);
                    parameters.Add("@CustomerID", CustomerID);
                    var result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> UpdateCustomerMemo(long ID, string MemoText, string UserID )
        {
            try
            {
                var sql = DatabaseProcedure.CspmProcedure.XCRV360_V2_CustomerMemoUpdate;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@MemoText", MemoText);
                    parameters.Add("@UserID", UserID);
                    parameters.Add("@ID", ID);
                    var result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> InsertXCRVLog(string LogType, string Method, string RequestPath, string RequestString, string StatusCode,
            string ElapsedTime, string UserId, string UserName, string ProjectId, string ExceptionDetails)
        {
            try
            {
                var sql = DatabaseProcedure.CspmProcedure.XCRV_Log_Details;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@LogType", LogType);
                    parameters.Add("@Method", Method);
                    parameters.Add("@RequestPath", RequestPath);
                    parameters.Add("@RequestString", RequestString);
                    parameters.Add("@StatusCode", StatusCode);
                    parameters.Add("@ElapsedTime", ElapsedTime);
                    parameters.Add("@UserId", UserId);
                    parameters.Add("@UserName", UserName);
                    parameters.Add("@ProjectId", ProjectId);
                    parameters.Add("@ExceptionDetails", ExceptionDetails);
                    var result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
