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
    public class MobileVsAccountRepository : IMobileVsAccountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public MobileVsAccountRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);
        }

        public async Task<IEnumerable<MobileVsAccount>> GetMobileVsAccount(string pstrACNO, string pstrMobNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_MOBILE_VS_ACCOUNT;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", pstrACNO);
                parameters.Add("P_VC_MOBNO", pstrMobNo);
                var result = (await connection.QueryAsync<MobileVsAccount>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }
    }
}
