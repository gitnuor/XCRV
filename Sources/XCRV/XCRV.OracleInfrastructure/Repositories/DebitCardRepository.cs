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
    public class DebitCardRepository : IDebitCardRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // private OracleDataAdapter dbAdapter;

        public DebitCardRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);

        }

        public async Task<IEnumerable<DebitCardDetail>> GetDebitCardDetailInfo(string cardNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_DEBIT_CARD_DETAILS;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_ATM", cardNo);                
                var result = (await connection.QueryAsync<DebitCardDetail>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<DebitCardTransaction>> GetDebitCardTransaction(string pstrAccNo, string IsStatementTrue)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_DEBIT_CARD_TRANSACTION;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_ATM", pstrAccNo);
                parameters.Add("P_IsStatementTrue", IsStatementTrue);
                var result = (await connection.QueryAsync<DebitCardTransaction>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<DebitCardDetail>> GetDebitCardNo(string pstrAccNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_DEBIT_CARD_NO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_AccNo", pstrAccNo);                
                var result = (await connection.QueryAsync<DebitCardDetail>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<SIinformation>> GetSIinformation(string custId)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SI_INFO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CIF", custId);
                var result = (await connection.QueryAsync<SIinformation>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }
        
         public async Task<IEnumerable<LCinformation>> GetLCinformation(string accno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_MULT_LC;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACCT_ID", accno);
                var result = (await connection.QueryAsync<LCinformation>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<LCinformation> GetLcDetails(string accno, string LcNo)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_LC_INFO;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ACCT", accno);
                    parameters.Add("P_VC_LC_ID", LcNo);
                    var result = (await connection.QueryAsync<LCinformation>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    //var result = (await connection.QueryAsync<LCinformation>(sql, parameters, commandType: CommandType.StoredProcedure)).ToString();
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

        public async Task<IEnumerable<PremimunCustAvgBal>> GetAvgPremiumBal(string seachType, string seachString)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_AVG_BAL_RPT;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("NumOfMonths", seachType);
                parameters.Add("minBalance", seachString);
                var result = (await connection.QueryAsync<PremimunCustAvgBal>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        
       public async Task<PremimumPersonalInfo> GetPremimumPersonelInfo(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_PERS_INFO;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ID", custid);
                    var result = (await connection.QueryAsync<PremimumPersonalInfo>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                   
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
        
      public async Task<PremimumPersonalInfo> GetPremimumAccountTotal(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TOT_INT_CUST;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ID", custid);
                    var result = (await connection.QueryAsync<PremimumPersonalInfo>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();

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

        public async Task<PremimumPersonalInfo> GetPremimumTotDepositBal(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_DEPOSIT_TOT_BAL;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ID", custid);
                    var result = (await connection.QueryAsync<PremimumPersonalInfo>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();

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

        
     public async Task<PremimumPersonalInfo> GetPremimumTotLoanBal(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TOT_LOAN_AMT_CUST_ID;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ID", custid);
                    var result = (await connection.QueryAsync<PremimumPersonalInfo>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();

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

        public async Task<IEnumerable<PremimumPersonalInfo>> GetPremimumDeposits(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_DEPOSIT_BAL_BY_CUST_ID;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CUST_ID", custid);
                var result = (await connection.QueryAsync<PremimumPersonalInfo>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<IEnumerable<PremimumAsset>> GetPremimumAssets(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_DA_BO_LAA_BY_CUST_ID;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CUST_ID", custid);
                var result = (await connection.QueryAsync<PremimumAsset>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result;
            }
        }

        public async Task<PremimumAsset> GetPremimumTotAssetBal(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TOT_LOAN_AMT_CUST_ID;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ID", custid);
                    var result = (await connection.QueryAsync<PremimumAsset>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();

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

        public async Task<PremimumAsset> Get6monthavgTotal(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_AVG_BAL_6_MON_TOT;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ID", custid);
                    var result = (await connection.QueryAsync<PremimumAsset>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();

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

        public async Task<PremimumAsset> Get1yearavgTotal(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_AVG_BAL_12_MON_TOT;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ID", custid);
                    var result = (await connection.QueryAsync<PremimumAsset>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();

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

        public async Task<IEnumerable<PremimumAvgBal>>  GetDepMove12MonBySchm(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_AVG_BAL_12_MON_BY_SCHM;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CUST_ID", custid);
                    var result = (await connection.QueryAsync<PremimumAvgBal>(sql, parameters, commandType: CommandType.StoredProcedure));

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

        public async Task<PremimumAc> GetPremimumCustId(string custid)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_PREMIUM_AC;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new OracleDynamicParameters();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_VC_CIF", custid);
                    var result = (await connection.QueryAsync<PremimumAc>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();

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
