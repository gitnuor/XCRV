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
using XCRV.Domain.CommonEnums;
using XCRV.Domain.Entities;

namespace XCRV.OracleInfrastructure.Repositories
{
    public class CustomerSearchRepository : ICustomerSearchRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _connectionString68;
        private readonly string _cardProCustomer360Analyzer;

        public CustomerSearchRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);
            this._connectionString68 = _configuration.GetConnectionString(DatabaseConnection.FinacleReportDB);
            this._cardProCustomer360Analyzer = _configuration.GetConnectionString(DatabaseConnection.CardProCustomer360Analyzer);
        }

        public async Task<IList<CustomerSearch>> getAsthaStatus(string custid, string isStatementTrue)
        {
            var sql = DatabaseProcedure.FinacalProcedure.SP_astha_status;
            IList <CustomerSearch> res=new List<CustomerSearch>();
            CustomerSearch tresult=null;
            try
            {
                using (var connection = new OracleConnection(_connectionString68))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CUST_ID", custid);
                    parameters.Add("P_IsStatementTrue", isStatementTrue);
                    var result = (await connection.QueryAsync<CustomerSearch>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();

                    if (result.Count <= 0)
                    {
                        tresult = new CustomerSearch();
                        res.Add(tresult);
                        return res;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                var test = ex.Message;
                throw;
            }
        }

        public async Task<string> GetProbasiFlagByUser(string custId)
        {
            var sql = "SELECT CUSTOMERNREFLG  FROM CRMUSER.ACCOUNTS WHERE ORGKEY='" + custId + "'";
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
               // parameters.Add("@custID", custId);
                var result = await connection.QueryAsync(sql, parameters, commandType: CommandType.Text);
                connection.Close();
                return result.FirstOrDefault().CUSTOMERNREFLG;
            }
        }

        public async Task<IList<CustomerGurantorStaticData>> SearchByCustomerGurantorStatic(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_GUARANT_DATA;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<CustomerGurantorStaticData>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<DepositSummary>> SearchCustomerDeposit(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_DEPOSIT_SUMMARY;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<DepositSummary>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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
        public async Task<IList<CurrentAccSummary>> SearchCustomerCurrent(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_CURRENT_SUMMARY;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<CurrentAccSummary>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<TermLoanDisbursment>> SearchTermLoanDisbursment(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_TERM_LOAN_DISBURSMENT;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<TermLoanDisbursment>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<TermLoanPerformance>> SearchTermLoanPerformance(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_TERM_LOAN_Performance;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<TermLoanPerformance>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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
        public async Task<IList<FinStatementDetails>> SearchByTranId(string TranId, DateTime valueDate)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TRANS_BY_TRANSID;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_TRANS_ID", TranId);
                    parameters.Add("P_TRANS_DATE", valueDate.ToString("dd-MMM-yyyy"));
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

        public async Task<IList<Customer360Summary>> SearchCust360Summary(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_360_SUMMARY;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<Customer360Summary>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<FundedLoan>> SearchFundedLoanSummary(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_FUNDED_Loan_SUMMARY;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<FundedLoan>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<NonFunded>> SearchNonFundedLoanSummary(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_NonFUNDED_Loan_SUMMARY;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<NonFunded>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<CompositeLimit>> SearchCompositeLoanSummary(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_Composite_Loan_SUMMARY;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<CompositeLimit>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<LiabilitiesSummary>> SearchLiabilitiesSummary(string searchString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_Liabilities_SUMMARY;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CIF_ID", searchString);
                    var result = (await connection.QueryAsync<LiabilitiesSummary>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<CardIssuence>> SearchcardIssuenceData(string searchString)
        {
            var sql = DatabasePackage.CardPro_XCRV360_CUSTOM_ANALYZER + DatabaseProcedure.FinacalProcedure.SP_CREDIT_CARD_ISSUANCE;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_cardProCustomer360Analyzer))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("P_CIF_ID", searchString);
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);                    
                    var result = (await connection.QueryAsync<CardIssuence>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<CreditPerformance>> SearchcardPerformanceData(string searchString)
        {
            var sql = DatabasePackage.CardPro_XCRV360_CUSTOM_ANALYZER + DatabaseProcedure.FinacalProcedure.SP_CREDIT_CARD_PERFORMANCE;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_cardProCustomer360Analyzer))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("P_CIF_ID", searchString);
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    var result = (await connection.QueryAsync<CreditPerformance>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<GraphData>> getGraphData(string param,string searchString)
        {
            var sql = DatabasePackage.CardPro_XCRV360_CUSTOM_ANALYZER + DatabaseProcedure.FinacalProcedure.SP_GRAPH_DATA;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_cardProCustomer360Analyzer))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("P_INP", param);
                    parameters.Add("P_CIF_ID", searchString);
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    var result = (await connection.QueryAsync<GraphData>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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
        public async Task<IList<GraphData2>> getGraphData2(string param, string searchString)
        {
            var sql = DatabasePackage.CardPro_XCRV360_CUSTOM_ANALYZER + DatabaseProcedure.FinacalProcedure.SP_GRAPH_DATA;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_cardProCustomer360Analyzer))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("P_INP", param);
                    parameters.Add("P_CIF_ID", searchString);
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    var result = (await connection.QueryAsync<GraphData2>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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
        public async Task<IList<GraphData3>> getGraphData3(string param, string searchString)
        {
            var sql = DatabasePackage.CardPro_XCRV360_CUSTOM_ANALYZER + DatabaseProcedure.FinacalProcedure.SP_GRAPH_DATA;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_cardProCustomer360Analyzer))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("P_INP", param);
                    parameters.Add("P_CIF_ID", searchString);
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    var result = (await connection.QueryAsync<GraphData3>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<GraphData4>> getGraphData4(string param, string searchString)
        {
            var sql = DatabasePackage.CardPro_XCRV360_CUSTOM_ANALYZER + DatabaseProcedure.FinacalProcedure.SP_GRAPH_DATA;
            IList<CustomerGurantorStaticData> res = new List<CustomerGurantorStaticData>();
            try
            {
                using (var connection = new OracleConnection(_cardProCustomer360Analyzer))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("P_INP", param);
                    parameters.Add("P_CIF_ID", searchString);
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    var result = (await connection.QueryAsync<GraphData4>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
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

        public async Task<IList<CustomerSearch>> SearchCustomerBySearchCriteria(CustomerSearchType searchType, string searchString, string isStatementTrue, string extensiveType, string _pbsFlag)
        {
            var sql = "";
            var parameters = new OracleDynamicParameters();
            if (_pbsFlag == "1")
            {
                switch (searchType)
                {
                    case CustomerSearchType.SP_SEARCH_CUST_BY_ACNO_HR:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SEARCH_CUST_BY_ACNO_HR;
                        parameters.Add("P_AC_NO", searchString);
                        break;

                    case CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SEARCH_CUST_BY_CUST_ID_HR;
                        parameters.Add("P_CUST_ID", searchString);
                        break;

                    case CustomerSearchType.SP_SEARCH_CUST_BY_ATM_HR:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SEARCH_CUST_BY_ATM_HR;
                        parameters.Add("P_ATM", searchString);
                        break;
                    case CustomerSearchType.SP_SEARCH_CUST_BY_NAME_HR:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SEARCH_CUST_BY_NAME_HR;
                        parameters.Add("P_CUSTOMER_NAME", searchString);
                        break;
                    case CustomerSearchType.SP_EXTENSIVE_SEARCH_PBS:
                        sql = DatabaseProcedure.FinacalProcedure.SP_EXTENSIVE_SEARCH_PBS;
                        parameters.Add("P_MOBILE", searchString);
                        break;

                    case CustomerSearchType.SP_EXTENSIVE_SEARCH:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_EXTENSIVE_SEARCH;
                        parameters.Add("P_TYPE", extensiveType.ToUpper());
                        parameters.Add("P_SEARCH", searchString);
                        break;
                }
                if (sql == DatabaseProcedure.FinacalProcedure.SP_EXTENSIVE_SEARCH_PBS)
                {
                    using (var connection = new OracleConnection(_connectionString68))
                    {
                        connection.Open();
                        parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                        parameters.Add("P_IsStatementTrue", isStatementTrue);
                        var result = (await connection.QueryAsync<CustomerSearch>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                        connection.Close();

                        return result;
                    }
                }
                else
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        connection.Open();
                        parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                        parameters.Add("P_IsStatementTrue", isStatementTrue);
                        var result = (await connection.QueryAsync<CustomerSearch>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                        connection.Close();

                        return result;
                    }

                }
            }
            else {
                switch (searchType)
                {
                    case CustomerSearchType.SP_SEARCH_CUST_BY_ACNO_HR:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SEARCH_CUST_BY_ACNO_PBS;
                        parameters.Add("P_AC_NO", searchString);
                        break;

                    case CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SEARCH_CUST_BY_CUST_ID_PBS;
                        parameters.Add("P_CUST_ID", searchString);
                        break;

                    case CustomerSearchType.SP_SEARCH_CUST_BY_ATM_HR:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SEARCH_CUST_BY_ATM_PBS;
                        parameters.Add("P_ATM", searchString);
                        break;
                    case CustomerSearchType.SP_SEARCH_CUST_BY_NAME_HR:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SEARCH_CUST_BY_NAME_PBS;
                        parameters.Add("P_CUSTOMER_NAME", searchString);
                        break;
                    case CustomerSearchType.SP_EXTENSIVE_SEARCH_PBS:
                        sql = DatabaseProcedure.FinacalProcedure.SP_EXTENSIVE_SEARCH_PBS;
                        parameters.Add("P_MOBILE", searchString);
                        break;

                    case CustomerSearchType.SP_EXTENSIVE_SEARCH:
                        sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_EXTENSIVE_SEARCH;
                        parameters.Add("P_TYPE", extensiveType.ToUpper());
                        parameters.Add("P_SEARCH", searchString);
                        break;
                }
                if (sql == DatabaseProcedure.FinacalProcedure.SP_EXTENSIVE_SEARCH_PBS)
                {
                    using (var connection = new OracleConnection(_connectionString68))
                    {
                        connection.Open();
                        parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                        parameters.Add("P_IsStatementTrue", isStatementTrue);
                        var result = (await connection.QueryAsync<CustomerSearch>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                        connection.Close();

                        return result;
                    }
                }
                else
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        connection.Open();
                        parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                        parameters.Add("P_IsStatementTrue", isStatementTrue);
                        var result = (await connection.QueryAsync<CustomerSearch>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                        connection.Close();

                        return result;
                    }

                }


            }
          
            
        }

    }
}
