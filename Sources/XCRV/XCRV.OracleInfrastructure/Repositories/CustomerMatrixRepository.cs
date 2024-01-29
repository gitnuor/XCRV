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
    public class CustomerMatrixRepository : ICustomerMatrixRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CustomerMatrixRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);
        }

        public async Task<IEnumerable<CustomerMatrix>> GetTDA(string pstrCustID)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TDA_SELECT;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CUST_ID", pstrCustID);
                var result = (await connection.QueryAsync<CustomerMatrix>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<CustomerMatrix>> GetLAA(string pstrCustID)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_LAA_BY_CUST_ID_SELECT;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CUST_ID", pstrCustID);
                var result = (await connection.QueryAsync<CustomerMatrix>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<CustomerMatrix>> GetSBA(string pstrCustID)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_SBA_BY_CUST_ID_SELECT;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CUST_ID", pstrCustID);
                var result = (await connection.QueryAsync<CustomerMatrix>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<CustomerMatrix>> GetCAA(string pstrCustID)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CAA_BY_CUST_ID_SELECT;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CUST_ID", pstrCustID);
                var result = (await connection.QueryAsync<CustomerMatrix>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<CustomerTransactionFrequency>> GetTransactionFrequency(string pstrCustID)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_TRAN_FREQ_LAST_MON_BY_CIF;
            var parameters = new OracleDynamicParameters();
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CUST_ID", pstrCustID);
                var result = (await connection.QueryAsync<CustomerTransactionFrequency>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

        public async Task<IEnumerable<CustomerAgeClassification>> GetCustomerAgeClassification(string pstrCustID)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CUST_AGE_CLASSIFICATION;
            var parameters = new OracleDynamicParameters();
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("P_VC_CUST_ID", pstrCustID);
                var result = (await connection.QueryAsync<CustomerAgeClassification>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;
            }
        }

    }
}
