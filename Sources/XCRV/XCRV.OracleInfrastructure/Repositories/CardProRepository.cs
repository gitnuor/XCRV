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
    public class CardProRepository : ICardProRepository
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CardProRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.OraCardProArcvConnection);
        }

        public async Task<CradProStatementSummary> GetPreviousStatementSummary(string pstrCustID, string pstrMonthYear, string pstrPrevMonthYear, string pACCT_TYPE)
        {
            pACCT_TYPE = pACCT_TYPE == "1" ? "BDT" : "USD";

            var sql = DatabasePackage.OraCardProArcvConnectionPackageName + DatabaseProcedure.OraCardProArcvConnectionProcedure.SP_CardPro_GET_PSS_BY_CRDNO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();                
                parameters.Add("P_CARD_NO", pstrCustID);
                parameters.Add("P_MON_YEAR", pstrMonthYear);
                parameters.Add("P_PRV_MON_YEAR", pstrPrevMonthYear);
                parameters.Add("P_ACCT_TYPE", pACCT_TYPE);
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                var result = (await connection.QueryAsync<CradProStatementSummary>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                connection.Close();

                return result;
            }
        }

        public async Task<IList<CardProStatementDetails>> GetPreviousStatementDetails(string pstrCARDNO, string P_MON_YEAR, string pACCT_TYPE)
        {
            int bill_curr_cd;
            bill_curr_cd = Convert.ToInt32(pACCT_TYPE) == 1 ? 50 : 0;

            var sql = DatabasePackage.OraCardProArcvConnectionPackageName + DatabaseProcedure.OraCardProArcvConnectionProcedure.SP_CardPro_GET_PSD_BY_CRDNO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("P_CARD_NO", pstrCARDNO);
                parameters.Add("P_MON_YEAR", P_MON_YEAR);
                parameters.Add("P_ACCT_TYPE", bill_curr_cd);
                
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                var result = (await connection.QueryAsync<CardProStatementDetails>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();

                return result;
            }
        }

        public async Task<CardProCreditHistory> GetCreditHistorySummary(string pstrCardNo, string acct_type)
        {
            var sql = DatabasePackage.OraCardProArcvConnectionPackageName + DatabaseProcedure.OraCardProArcvConnectionProcedure.SP_CardPro_GET_CHS_BY_CRDNO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("P_CARD_NO", pstrCardNo);
                parameters.Add("P_ACCT_TYPE", acct_type);
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                var result = (await connection.QueryAsync<CardProCreditHistory>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                connection.Close();

                return result;
            }
        }

        public async Task<TransactionSummary> GetTransactionSummary(string CardNo, DateTime fromDate, DateTime toDate)
        {
            var sql = DatabasePackage.OraCardProArcvConnectionPackageName + DatabaseProcedure.OraCardProArcvConnectionProcedure.SP_CardPro_GET_TRN_BY_CRDNO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("P_CARD_NO", CardNo.Trim());
                parameters.Add("P_FROM_DATE", fromDate);
                parameters.Add("P_TO_DATE", toDate);
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                var result = (await connection.QueryAsync<TransactionSummary>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                connection.Close();

                return result;
            }
        }
    }
}
