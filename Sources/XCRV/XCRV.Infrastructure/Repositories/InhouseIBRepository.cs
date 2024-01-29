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
    public class InhouseIBRepository : IInhouseIBRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public InhouseIBRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.InhouseIBDBConnection);
        }

        public async Task<IList<IbCustomerInfo>> GetIbCusotmerInfo(string acno)
        {
            try
            {
                var sql = DatabaseProcedure.InhouseIBDBConnectionProcedure.SP_UserInfo_Get_By_AccountNo;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("@AccNo", acno);
                    var result = (await connection.QueryAsync<IbCustomerInfo>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IList<IbTransaction>> GetTransactionByAccNo(string acno, string RecordToReturn)
        {
            try
            {
                var sql = DatabaseProcedure.InhouseIBDBConnectionProcedure.SP_Get_Transactions_By_AccNo;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("@AccNo", acno);
                    parameters.Add("@RecordToReturn", RecordToReturn);
                    var result = (await connection.QueryAsync<IbTransaction>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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
