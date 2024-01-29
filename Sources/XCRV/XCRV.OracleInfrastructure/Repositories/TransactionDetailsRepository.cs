using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.OracleInfrastructure.Repositories
{
    public class TransactionDetailsRepository : ITransactionDetailsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // private OracleDataAdapter dbAdapter;

        public TransactionDetailsRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);

        }

        public async Task<IList<TransactionDetails>> GetTransactionDetails(string custid, DateTime pdtimeFromDate, DateTime pdtimeToDate)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CORP_ACCT_STMT_V2;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_ACNO", custid);
                    parameters.Add("P_DT_FROM_DATE", pdtimeFromDate.ToString("dd-MMM-yyyy"));
                    parameters.Add("P_DT_TO_DATE", pdtimeToDate.ToString("dd-MMM-yyyy"));
                    var result = (await connection.QueryAsync<TransactionDetails>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<AccountADCTransaction>> GetADCTransactionDetails(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TRANSACTION_CDCI_CREDITCARD;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_ACNO", accno);
                    parameters.Add("P_DT_FROM_DATE", pdtimeFromDate.ToString("dd-MMM-yyyy"));
                    parameters.Add("P_DT_TO_DATE", pdtimeToDate.ToString("dd-MMM-yyyy"));
                    var result = (await connection.QueryAsync<AccountADCTransaction>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<AccountADCTransaction>> GetAccountTransactionDetails(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TRANSACTION;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_ACNO", accno);
                    parameters.Add("P_DT_FROM_DATE", pdtimeFromDate.ToString("dd-MMM-yyyy"));
                    parameters.Add("P_DT_TO_DATE", pdtimeToDate.ToString("dd-MMM-yyyy"));
                    var result = (await connection.QueryAsync<AccountADCTransaction>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<FinStatementDetails> GetCustInfo(string accno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_STATEMENT_CUST_DETAILS;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("p_Details", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("p_AccountNo", accno);
                    // parameters.Add("P_DT_FROM_DATE", pdtimeFromDate.ToString("dd-MMM-yyyy"));
                    // parameters.Add("P_DT_TO_DATE", pdtimeToDate.ToString("dd-MMM-yyyy"));
                    var result = (await connection.QueryAsync<FinStatementDetails>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
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

        public async Task<IList<FinStatementDetails>> GetFinTransactionDetails(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_FinStatement;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("p_Details", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("p_AccountNo", accno);
                    parameters.Add("p_FromDate", pdtimeFromDate.ToString("dd-MMM-yyyy"));
                    parameters.Add("p_ToDate", pdtimeToDate.ToString("dd-MMM-yyyy"));

                    var result = (await connection.QueryAsync<FinStatementDetails>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IEnumerable<InterestDetails>> GetAccInterestDetails(string accno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_AC_INT_DTL_INQ;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_AC_NO", accno);
                    var result = (await connection.QueryAsync<InterestDetails>(sql, parameters, commandType: CommandType.StoredProcedure));
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

        public async Task<IList<FinMiniStatement>> GetFinMiniStatementDetails(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_MINI_STATEMENT;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_FORACID", acno);

                var result = (await connection.QueryAsync<FinMiniStatement>(sql,  parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
               // IList<FinMiniStatement> finMiniDetails = new List<FinMiniStatement>();

               // FinStatementDetails tran;

                string[] StringTokens = new string[10];
                DataRow dtr;
                DataTable dtFormattedStm = this.GetStatementTableStructure();

                DataTable dataTable = new DataTable(typeof(FinMiniStatement).Name);

                //Get all the properties
                PropertyInfo[] Props = typeof(FinMiniStatement).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Defining type of data column gives proper data table 
                    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name, type);
                }
                foreach (FinMiniStatement item in result)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                dataTable.Columns.Remove("TranDate");
                dataTable.Columns.Remove("Particulars");
                dataTable.Columns.Remove("TranType");
                dataTable.Columns.Remove("Amount");

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(dataTable.Rows[i][0].ToString().Trim()))// Modified By Biplob; Dated:30-Aug-2010; purpose:Ommiting blunk record
                        continue;
                    StringTokens = dataTable.Rows[i]["Trans"].ToString().Split('|');
                    dtr = dtFormattedStm.NewRow();
                    dtr["TranDate"] = GetFormattedDate(StringTokens[0].ToString());
                    dtr["Particulars"] = StringTokens[3].ToString();
                    dtr["TranType"] = StringTokens[5].ToString();
                    dtr["Amount"] = StringTokens[4].ToString();
                    dtFormattedStm.Rows.Add(dtr);
                }
                IList<FinMiniStatement> list = new List<FinMiniStatement>();

                list = (from DataRow dr in dtFormattedStm.Rows
                        select new FinMiniStatement()
                        {
                            TranDate = dr["TranDate"].ToString(),
                            Particulars = dr["Particulars"].ToString(),
                            TranType = dr["TranType"].ToString(),
                            Amount = dr["Amount"].ToString(),
                        }).ToList();
                return list;
            }
        }

        protected DataTable GetStatementTableStructure()
        {
            DataTable dtStatement = new DataTable();
            dtStatement.Columns.Add("TranDate", System.Type.GetType("System.DateTime"));
            dtStatement.Columns.Add("Particulars", System.Type.GetType("System.String"));
            dtStatement.Columns.Add("TranType", System.Type.GetType("System.String"));
            dtStatement.Columns.Add("Amount", System.Type.GetType("System.String"));
            return dtStatement;
        }


        protected string GetFormattedDate(string pstrDate)
        {
            DateTime dtimeTemp;
            CultureInfo enUS = new CultureInfo("en-US");
            if (DateTime.TryParseExact(pstrDate, "ddMMyyyy", enUS, DateTimeStyles.None, out dtimeTemp))
            {
                return dtimeTemp.ToString("dd MMM yyyy");
            }
            else
            {
                return pstrDate;
            }
        }

        public async Task<IEnumerable<InterestDetails>> GetAccInterestBreakup(string accno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_INT_BREAKUP;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_AC_NO", accno);
                    var result = (await connection.QueryAsync<InterestDetails>(sql, parameters, commandType: CommandType.StoredProcedure));
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
        public async Task<IEnumerable<InterestDetails>> GetAccRelatedParty(string accno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_RELATED_PARTY_INQUERY;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_AC_NO", accno);
                    var result = (await connection.QueryAsync<InterestDetails>(sql, parameters, commandType: CommandType.StoredProcedure));
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

        public async Task<IList<ATMTransaction>> GetATMTransactionDetails(string cardno, DateTime pdtimeFromDate, DateTime pdtimeToDate)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_ATM_TRANSACTION;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CARD_NO", cardno);
                    parameters.Add("P_DT_FROM_DATE", pdtimeFromDate.ToString("dd-MMM-yyyy"));
                    parameters.Add("P_DT_TO_DATE", pdtimeToDate.ToString("dd-MMM-yyyy"));
                    var result = (await connection.QueryAsync<ATMTransaction>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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
      public async Task<BalanceDetails> GetBalanceDetails(string accno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.sp_account_balance_details;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("cur_out", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("p_vc_ac_no", accno);
                    var result = (await connection.QueryAsync<BalanceDetails>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
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

        public async Task<IndividualTransaction> GetIndividualTranDetails(string accountNo, string tranID, DateTime date)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TRAN_DTL_INDI;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("v_gdetails", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_ACNO", accountNo);
                    parameters.Add("P_VC_TRAN_ID", tranID);
                    parameters.Add("P_DT_TRAN_DATE", date.ToString("dd-MMM-yyyy"));
                    var result = (await connection.QueryAsync<IndividualTransaction>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
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

    }
}
