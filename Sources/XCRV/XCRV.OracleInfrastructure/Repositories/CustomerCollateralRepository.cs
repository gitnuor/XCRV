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
    public class CustomerCollateralRepository: ICustomerCollateralRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // private OracleDataAdapter dbAdapter;

        public CustomerCollateralRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);

        }

        public async Task<CustomerCollateral> GetCustomerCollateralByCustid(string stringCif)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_LIMIT_TOTAL;
            var parameters = new OracleDynamicParameters();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("V_VC_CUST_ID", stringCif);

                    var result = (await connection.QueryAsync<CustomerCollateral>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    connection.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                var test = ex.Message;
                throw;
            }

        }

        public async Task<IList<CustomerCollateral>> getCustomerCollateral(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_COLLATERAL_S;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("V_VC_CUST_ID", custid);
                    //parameters.Add("P_IsStatementTrue", isStatementTrue);
                    var result = (await connection.QueryAsync<CustomerCollateral>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                    //var trestult = new List<CustomerLimit>();
                    return result;
                }
            }
            catch (Exception ex)
            {
                var test = ex.Message;
                throw;
            }
        }
    }
}
