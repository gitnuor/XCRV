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
    public class OracleBaseRepository : IOracleBaseRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public OracleBaseRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);
        }

        public async Task<DateTime> GetLastUpdatedDate()
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_BOD;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                var result = (await connection.QueryAsync(sql, parameters, commandType: CommandType.StoredProcedure)).ToList().First();
                connection.Close();
                string lastUpdatedDate = result.LAST_UPD_DATE.ToString();
                DateTime date = new DateTime();
                DateTime.TryParse(lastUpdatedDate, out date);
                return date;
            }
        }


        public async Task<bool> IsAccountAccessableByUser(string accno,string username)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_getAccAccessibilityStatus;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("P_AccNo",accno);
                parameters.Add("UserName", username);                
                parameters.Add("status", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 10);

                var result = (await connection.QueryAsync(sql, parameters, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();
                connection.Close();
                return parameters.Get<string>("status") == "TRUE" ? true : false;
                
            }
        }

        public async Task<string> GetAccountSchemCodeByAccountNumber(string accountNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_INFO_BY_ACCNO;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_AccNo", accountNo);
                var result = (await connection.QueryAsync(sql, parameters, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();
                connection.Close();
                string schemCode = string.Empty;
                if (result != null)
                {
                    result.schm_code.ToString();
                }
                return schemCode;
            }
        }

        public async Task<string> GetSchemCodeByATMCardNumber(string cardno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_INFO_BY_ATM;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_ATM", cardno);
                var result = (await connection.QueryAsync(sql, parameters, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();
                connection.Close();
                string schemCode = string.Empty;
                if (result != null)
                {
                    result.schm_code.ToString();
                }
                return schemCode;
            }
        }

        public async Task<IList<string>> GetDebitCardByCustomerNumber(string custNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_DEBIT_CARD_NO_BY_CUSTID;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_CUST_ID", custNo);
                var result = (await connection.QueryAsync<string>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();

                return result;

                //IList<string> list = new List<string>();
                //if (result.Count >0)
                //{
                //    list = (from p in result
                //           select p.card_number)
                //}
                //return schemCode;
            }
        }


    }
}
