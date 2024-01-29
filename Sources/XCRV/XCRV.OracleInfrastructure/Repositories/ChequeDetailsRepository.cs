using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.OracleInfrastructure.Repositories
{
    public class ChequeDetailsRepository : IChequeDetailsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // private OracleDataAdapter dbAdapter;

        public ChequeDetailsRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);

        }

        public async Task<IList<InwardCheque>> GetTop20InwardCheques(string accNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.INWARD_CHECK_TOP20;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", accNo);
                var result = (await connection.QueryAsync<InwardCheque>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result.ToList();
            }
        }

        public async Task<IList<OutwardCheque>> GetTop20OutwardCheques(string accNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_OUTWARD_CHECK_TOP20;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", accNo);                
                var result = (await connection.QueryAsync<OutwardCheque>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result.ToList();
            }
        }
    }
}
