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
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // private OracleDataAdapter dbAdapter;

        public CreditCardRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.OraCardProStageConnection);

        }

        public async Task<IList<CreditCardInfo>> GetCardCustInfoList(string CustID, string CardNo, string MobileNo)
        {
            var sql = DatabasePackage.CARDPRO_PACKAGE_NAME + DatabaseProcedure.CardProProcedure.SP_GET_TDS_INFO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_CB_CUSTOMER_IDNO", CustID);
                parameters.Add("P_CB_CARDHOLDER_NO", CardNo);
                parameters.Add("P_CB_MOBILE_NO", MobileNo);
                var result = (await connection.QueryAsync<CreditCardInfo>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result.ToList();
            }
        }

        public async Task<IList<CreditCardInfo>> GetCardDetailsInfoList(string CustID, string CardNo)
        {
            var sql = DatabasePackage.CARDPRO_PACKAGE_NAME + DatabaseProcedure.CardProProcedure.SP_CUST_INFO_CUSTOMER_IDNO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_CB_CUSTOMER_IDNO", CustID);
                parameters.Add("P_CB_CARDHOLDER_NO", CardNo);                
                var result = (await connection.QueryAsync<CreditCardInfo>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result.ToList();
            }
        }
    }
}
