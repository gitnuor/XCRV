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
    public class AccountSchemeRepository : IAccountSchemeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AccountSchemeRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);
        }

        public async Task<IEnumerable<TermDepositScheme>> GetTermDepositSchemByAcno(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME+ DatabaseProcedure.FinacalProcedure.SP_GET_TDS_INFO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", acno);
                var result = (await connection.QueryAsync<TermDepositScheme>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<CaSaAccountInfo>> GetCaSaAccountInfoByAcno(string acno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_SBA_CCA_INFO;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", acno);
                var result = (await connection.QueryAsync<CaSaAccountInfo>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<EffectiveBal> GetCustomerBal(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TRANSACTION_CURRENT;
            var parameters = new OracleDynamicParameters();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_ACNO", accno);

                    var result = (await connection.QueryAsync<EffectiveBal>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
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

        public async Task<EffectiveBal> GetAccBal(string accno)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TRANSACTION_CURRENT;
            var parameters = new OracleDynamicParameters();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_ACNO", accno);

                    var result = (await connection.QueryAsync<EffectiveBal>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
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

        public async Task<IEnumerable<AcMobile>> GetAcMobile(string pstrACNO, string pstrMobNo, string pstrQeryType)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_AC_MOBILE;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_ACNO", pstrACNO);
                parameters.Add("P_VC_MOBNO", pstrMobNo);
                parameters.Add("P_VC_QRY_TYPE", pstrQeryType);
                var result = (await connection.QueryAsync<AcMobile>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<AccSignatory>> GetACCSignatoryInfo(string pstrACNO)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_ACC_SIGNATORY_INFO;
            var parameters = new OracleDynamicParameters();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_ACC_Num", pstrACNO);

                    var result = (await connection.QueryAsync<AccSignatory>(sql, parameters, commandType: CommandType.StoredProcedure));
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
