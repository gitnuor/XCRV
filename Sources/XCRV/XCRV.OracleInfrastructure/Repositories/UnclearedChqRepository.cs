using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.OracleInfrastructure.Repositories
{
    public class UnclearedChqRepository : IUnclearedChqRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UnclearedChqRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);
        }

        public async Task<IEnumerable<UnclearedChqDr>> GetUnclearedChqAmtDr(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_UNCLEARED_CHQ_AMT_DR;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_AccNo", acno);
                var result = (await connection.QueryAsync<UnclearedChqDr>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<UnclearedChqCr>> GetUnclearedChqAmtCr(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_UNCLEARED_CHQ_AMT_CR;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_AccNo", acno);
                var result = (await connection.QueryAsync<UnclearedChqCr>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }
    }
}
