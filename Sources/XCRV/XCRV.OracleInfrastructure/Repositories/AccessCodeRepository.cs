using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.OracleInfrastructure.Repositories
{
    public class AccessCodeRepository : IAccessCodeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AccessCodeRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.XCRVFinConnection);
        }

        public async Task<IList<FinacleUser>> GetFinacleUserList()
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_FIN_USER_LIST;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                var result = (await connection.QueryAsync<FinacleUser>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result.OrderBy(x=>x.user_id).ToList();

            }
        }

        public async Task<IList<FinacleUser>> GetAccessInfoList()
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_ACCESSINFO_LIST;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                var result = (await connection.QueryAsync<FinacleUser>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();

                return result;

            }
        }


        public async Task<int> SaveAccessInfo(FinacleUser accessCode, string userName)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_XCRV_ACCESSINFOINSERT;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("ACS_ID", string.Empty);
                parameters.Add("U_ID", accessCode.user_id);
                parameters.Add("XCRV_UID", accessCode.xcrv_user_id);
                parameters.Add("AC_ACC_CODE", accessCode.acct_access_code);
                parameters.Add("ENTRY_U_ID", userName);
                parameters.Add("UPDID", string.Empty);

                var result = (await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;

            }
        }


        public async Task<FinacleUser> GetIsExistAccessCode(FinacleUser accessCode)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_CHECK_ACCESSINFO_IF_EXIST;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                
                parameters.Add("P_USER_ID", accessCode.user_id);
                parameters.Add("P_XCRV_USER_ID", accessCode.xcrv_user_id);
                parameters.Add("P_ACCESS_CODE", accessCode.acct_access_code);
                parameters.Add("CUR_OUT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                
                var result = (await connection.QueryAsync<FinacleUser>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result.FirstOrDefault();

            }
        }


        public async Task<int> UpdateAccessCode(FinacleUser accessCode, string userName)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_XCRV_ACCESSINFOUPDATE;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("ACS_ID", accessCode.access_id);
                parameters.Add("U_ID", accessCode.user_id);
                parameters.Add("XCRV_UID", accessCode.xcrv_user_id);
                parameters.Add("AC_ACC_CODE", accessCode.acct_access_code);
                parameters.Add("ENTRY_U_ID", string.Empty);
                parameters.Add("UPDID", userName);

                var result = (await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;

            }
        }


        public async Task<int> DeleteAccessCode(FinacleUser accessCode, string userName)
        {
            var sql = DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_XCRV_ACCESSINFODELETE;
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                var parameters = new OracleDynamicParameters();
                parameters.Add("inp_Access_id", accessCode.access_id);
                var result = (await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result;

            }
        }
    }
}
