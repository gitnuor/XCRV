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
    public class CrvCardChqRepository : ICrvCardChqRepository
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CrvCardChqRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.DbOrclConnectionStringCHQ);
        }

        public async Task<ChqCustomer> GetChqCustInfoList(string accountNo)
        {
            var sql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.Crv_ChqCustInfo;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("v_gdetails", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("strACNo", accountNo);
                var result = (await connection.QueryAsync<ChqCustomer>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result.FirstOrDefault();
            }
        }

        public async Task<IList<ChqStatus>> GetChqstatusByRange(string accountNo, string leafFrom, string leafTo, string user_id)
        {
            var sql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.Crv_Chqstatus_range;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("v_details", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add(@"p_AccountNo", accountNo);
                parameters.Add(@"v_ChqNo", leafFrom);
                parameters.Add(@"v_endchqno", leafTo);
                parameters.Add(@"user_id", user_id);
                var result = (await connection.QueryAsync<ChqStatus>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result.ToList();
            }
        }


        public async Task<string> ChangeChqstatusByRange(ChqStopRange chqStopRange)
        {
            var sql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.Crv_Chqstatusupdate_range;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("p_AccountNo", chqStopRange.AccountNo);
                parameters.Add("v_ChqNo", chqStopRange.ChqNo);
                parameters.Add("v_endchqNo", chqStopRange.EndchqNo);
                parameters.Add("v_status", chqStopRange.Status);
                parameters.Add("struserid", chqStopRange.Struserid);
                parameters.Add("P_REMARKS", chqStopRange.Rerarks);
                parameters.Add("v_return", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 2000);

                var result = (await connection.QueryAsync(sql, parameters, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();
                connection.Close();
                return parameters.Get<string>("v_return") ;

            }
        }


        public async Task<IList<ChqReqLog>> GetChqLog(string accountNo, string user_id)
        {
            var sql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.Crv_proctmp_chqlog;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("v_details", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add(@"strforacid", accountNo);
                parameters.Add(@"user_id", user_id);
               
                var result = (await connection.QueryAsync<ChqReqLog>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result.ToList();
            }
        }

        public async Task<IList<ChqReqLog>> GetChqReqLog(string accountNo, string frmDate, string toDate, string makerUserID, string checkerUserID)
        {
            if (string.IsNullOrEmpty(accountNo))
            {
                accountNo = "-99";
            }
            var sql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.CRV_CHQUSER_REQLOG_GET;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("v_details", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                parameters.Add(@"p_AccountNo", accountNo);
                parameters.Add(@"P_FrmDate", frmDate);
                parameters.Add(@"P_ToDate", toDate);
                parameters.Add(@"P_MakerUserID", makerUserID);
                parameters.Add(@"P_ChekerUserID", checkerUserID);

                var result = (await connection.QueryAsync<ChqReqLog>(sql, parameters, commandTimeout: 120, commandType: CommandType.StoredProcedure)) ;
                connection.Close();
                return result.ToList();
            }
        }


        public async Task<string> VerifyCheque(IList<ChqStopRangeVerify> chqStopRangeList, string ChekerUserID)
        {
            var verifyLogsql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.CRV_CHQUSER_REQLOG_VERIFY;

            var verifiySql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.CRV_CHECK_VERIFY;

            var parameters = new OracleDynamicParameters();

            string message = string.Empty;
            string output = string.Empty;

            string checkValidationMessage = string.Empty;

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                foreach (var entity in chqStopRangeList)
                {
                    #region CRV_CHECK_VERIFY

                    parameters = new OracleDynamicParameters();

                    parameters.Add("P_AccountNo", entity.foracid);
                    parameters.Add("P_ACID", entity.acid);
                    parameters.Add("P_frmchqno",  entity.frmChqno);
                    parameters.Add("P_tochqno", entity.toChqno);
                    parameters.Add("P_MakerUserID",  entity.User_Id);
                    parameters.Add("P_Particular", entity.particular);

                    parameters.Add("v_return", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 2000);
                    var result = (await connection.QueryAsync(verifiySql, parameters, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();
                    message = parameters.Get<string>("v_return");

                    #endregion


                    if (!string.IsNullOrEmpty(message))
                    {
                        checkValidationMessage = checkValidationMessage == string.Empty ?
                            checkValidationMessage + message + "(" + entity.particular + ")"
                            : checkValidationMessage + " and " + message + "(" + entity.particular + ")";
                    }

                    if (string.IsNullOrEmpty(message))
                    {

                        #region CRV_CHQUSER_REQLOG_VERIFY

                        parameters = new OracleDynamicParameters();

                        parameters.Add("P_AccountNo", entity.foracid);
                        parameters.Add("P_ACID", entity.acid);
                        parameters.Add("P_frmchqno", entity.frmChqno);
                        parameters.Add("P_tochqno", entity.toChqno);
                        parameters.Add("P_isverify", entity.isverify);
                        parameters.Add("P_MakerUserID", entity.User_Id);
                        parameters.Add("P_CheckerUserID", ChekerUserID);
                        parameters.Add("P_Remarks", entity.Remarks);
                        parameters.Add("P_Particular", entity.particular);

                        parameters.Add("v_return", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 2000);
                        result = (await connection.QueryAsync(verifyLogsql, parameters, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();
                        output = parameters.Get<string>("v_return");

                        #endregion

                    }
                }
                connection.Close();

                checkValidationMessage = checkValidationMessage != string.Empty ? " But " + checkValidationMessage + " Already verifyed." : checkValidationMessage;
                return output + " " + checkValidationMessage;

            }
        }

        public async Task<IList<ChqStopReport>> GetChqStopReport(string accountNo, string chqno, string frmDate, string toDate, string user_id)
        {
            if (string.IsNullOrEmpty(accountNo))
            {
                accountNo = "";
            }

            if (string.IsNullOrEmpty(user_id))
            {
                user_id = "-1";
            }

            if (string.IsNullOrEmpty(chqno))
            {
                chqno = "";
            }

            var sql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.CRV_CHQ_STOP_REPORT;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("v_details", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                parameters.Add(@"P_AccountNo", accountNo);
                parameters.Add(@"P_chqno", chqno);
                parameters.Add(@"P_FrmDate", frmDate);
                parameters.Add(@"P_ToDate", toDate);
                parameters.Add(@"P_UserID", user_id);

                var result = (await connection.QueryAsync<ChqStopReport>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result.ToList();
            }
        }
        
        public async Task<IList<ChqStopReport>> GetMakerUserList()
        {
            var sql = "select distinct  C.User_Id AS Agent_ID from CRV_CHQ.CRV_CHQUSER_REQLOG C";
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<ChqStopReport>(sql, commandType: CommandType.Text);
                connection.Close();
                return result.OrderBy(x => x.Agent_ID).ToList();
            }
        }

        public async Task<IList<ChqStopSummaryReport>> GetChqStopSummaryReport(string frmDate, string toDate)
        {
            var sql = DatabasePackage.CARD_CHQ_PACKAGE_NAME + DatabaseProcedure.CrvCardChqProcedure.CRV_CHQ_STOP_REPORT_SUM;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("v_details", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                
                parameters.Add(@"P_FrmDate", frmDate);
                parameters.Add(@"P_ToDate", toDate);               

                var result = (await connection.QueryAsync<ChqStopSummaryReport>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();
                return result.ToList();
            }
        }
    }
}
