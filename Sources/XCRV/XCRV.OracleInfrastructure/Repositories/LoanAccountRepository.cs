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
    public class LoanAccountRepository : ILoanAccountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public LoanAccountRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);
        }

        public async Task<IEnumerable<LoanAccountInfo>> GetLoanAccountInfoByAcno(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.GET_LAA_AC_INFO_CORE;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", acno);
                var result = (await connection.QueryAsync<LoanAccountInfo>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<LoanAccountDetails>> GetLoanAccountDetailsInfoByAcno(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.CRV_LOAN_DETAILS;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("p_vc_stracno", acno);

                var result = (await connection.QueryAsync(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                IList<LoanAccountDetails> loanAccountDetails = new List<LoanAccountDetails>();

                LoanAccountDetails loan;
                foreach (var r in result)
                {
                    loan = new LoanAccountDetails();

                    var fields = r as IDictionary<string, object>;
                    loan.DemandRaised = fields["Demand Raised"].ToString();
                    loan.DemandToBeRaised = fields["Demand to be raised"].ToString();
                    loan.InstallmentAmt = fields["Installment Amt"].ToString();
                    loan.InterestRate = fields["Interest Rate"].ToString();
                    loan.SchdlNo = fields["Schdl No"].ToString();
                    loan.TotalInstallment = fields["Total Installment"].ToString();

                    loanAccountDetails.Add(loan);
                }

                return loanAccountDetails;
            }
        }


        public async Task<IEnumerable<RepaymentHistory>> GetRepaymentHistoryByAcno(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CRV_LOAN_REPAYMENTHISTORY;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("p_vc_stracno", acno);
                var result = (await connection.QueryAsync<RepaymentHistory>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<LoanPayoffInfo>> GetLoanPayOff(string acno, string payOffDate)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_LOAN_PAYOFF_Info;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACCNO", acno);
                parameters.Add("p_REQ_ID", acno);
                parameters.Add("p_PAYOFF_DATE", payOffDate);

                var result = (await connection.QueryAsync<LoanPayoffInfo>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<LoanPayOff>> GetLiveLoanPayOff(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_LOAN_PAYOFF;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", acno);

                var result = (await connection.QueryAsync<LoanPayOff>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<Guarantor>> GetLoanGuarantorByLoanNumber(string loanNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_LOAN_GURANTOR_CORE;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", loanNo);
                var result = (await connection.QueryAsync<Guarantor>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<LoanDocument>> GetLoanDocumentsByLoanNumber(string loanNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_LOAN_DOC;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", loanNo);
                var result = (await connection.QueryAsync<LoanDocument>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<LoanAccountLimit>> GetLoanAccountLimitByLoanNumber(string loanNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_AC_LIMIT_DETAILS;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", loanNo);
                var result = (await connection.QueryAsync<LoanAccountLimit>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<CorporateCustomer>> GetLoanCustomerInfo(string cif)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_REPEAT_LOAN_CUSTOMER;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_CIF", cif);
                var result = (await connection.QueryAsync<CorporateCustomer>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<Proprietor>> GetLoanProprietorInfo(string cif)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_REPEAT_LOAN_PROPRIETOR;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_CIF", cif);
                var result = (await connection.QueryAsync<Proprietor>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<LoanAccountInfo>> GetLoanAccountInfoByCif(string cif)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_REPEAT_LOAN_INFO;
            var parameters = new OracleDynamicParameters();
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_CIF", cif);
                var result = (await connection.QueryAsync<LoanAccountInfo>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<Guarantor>> GetLoanAccountGuarantorInfoByLoanNo(string loanNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_REPEAT_LOAN_GUARANTOR;
            var parameters = new OracleDynamicParameters();
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_ACID", loanNo);
                var result = (await connection.QueryAsync<Guarantor>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }
    }

}
    
