using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.OracleInfrastructure.Repositories;

namespace XCRV.Infrastructure.Repositories
{
    public class CardChqRepository : ICardChqRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        private readonly string _mbsHoConnectionString;

        public CardChqRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.DbConnectionStringCardCHQ);
        }

        public async Task<ChqLeafLimit> GetCardChqLeafSearchLimit(string CHQ_Type)
        {
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_GET_CHQ_Leaf_Search_Limit;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@CHQ_Type", CHQ_Type);
                    var result = (await connection.QueryAsync<ChqLeafLimit>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IList<ChqBookInfo>> GetUpdatedChqBookData(string refNo, string agentID, string supervisorID, string subOrdinateID, string cardno,
            string chqno, string fromdate, string todate, string remarks, string intQueryType)
        {
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_Get_update_ChqBookData;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    
                    parameters.Add("@RefNo", refNo);
                    parameters.Add("@UserID", agentID);
                    parameters.Add("@supervisorID", supervisorID);
                    parameters.Add("@subOrdinateID", subOrdinateID);
                    parameters.Add("@cardno", cardno);
                    parameters.Add("@chqno", chqno);
                    parameters.Add("@frmdate", fromdate);
                    parameters.Add("@todate", todate);
                    parameters.Add("@Remarks", remarks);
                    parameters.Add("@intQueryType", intQueryType);

                    var result = (await connection.QueryAsync<ChqBookInfo>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IList<CardChqActiveVerify>> GetChqBookDataForVerify(string refNo, string agentID, string supervisorID, string subOrdinateID, string cardno,
            string chqno, string fromdate, string todate, string remarks)
        {
            try
            {
                string intQueryType = "3";
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_Get_update_ChqBookData;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("@RefNo", string.IsNullOrEmpty(refNo)?"": refNo);
                    parameters.Add("@UserID", string.IsNullOrEmpty(agentID)?"0":agentID);
                    parameters.Add("@supervisorID", string.IsNullOrEmpty(supervisorID) ? "0" : supervisorID);
                    parameters.Add("@subOrdinateID",  string.IsNullOrEmpty(subOrdinateID) ? "0" : subOrdinateID);
                    parameters.Add("@cardno",  string.IsNullOrEmpty(cardno) ? "" : cardno);
                    parameters.Add("@chqno",  string.IsNullOrEmpty(chqno) ? "" : chqno);
                    parameters.Add("@frmdate",  string.IsNullOrEmpty(fromdate) ? "" : fromdate);
                    parameters.Add("@todate",  string.IsNullOrEmpty(todate) ? "" : todate);
                    parameters.Add("@Remarks",  string.IsNullOrEmpty(remarks) ? "" : remarks);
                    parameters.Add("@intQueryType", intQueryType);

                    var result = (await connection.QueryAsync<CardChqActiveVerify>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<bool> VerifyChqBookData(string refNo, string agentID, string supervisorID, string subOrdinateID, string cardno,
            string chqno, string fromdate, string todate, string remarks)
        {
            try
            {
                string intQueryType = "5";
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_Get_update_ChqBookData;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("@RefNo", string.IsNullOrEmpty(refNo) ? "" : refNo);
                    parameters.Add("@UserID", string.IsNullOrEmpty(agentID) ? "0" : agentID);
                    parameters.Add("@supervisorID", string.IsNullOrEmpty(supervisorID) ? "0" : supervisorID);
                    parameters.Add("@subOrdinateID", string.IsNullOrEmpty(subOrdinateID) ? "0" : subOrdinateID);
                    parameters.Add("@cardno", string.IsNullOrEmpty(cardno) ? "" : cardno);
                    parameters.Add("@chqno", string.IsNullOrEmpty(chqno) ? "" : chqno);
                    parameters.Add("@frmdate", string.IsNullOrEmpty(fromdate) ? "" : fromdate);
                    parameters.Add("@todate", string.IsNullOrEmpty(todate) ? "" : todate);
                    parameters.Add("@Remarks", string.IsNullOrEmpty(remarks) ? "" : remarks);
                    parameters.Add("@intQueryType", intQueryType);

                    var result = (await connection.QueryAsync<CardChqActiveVerify>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();

                    

                    connection.Close();
                    return (result.Count > 0);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ApproveChqBookData(string refNo, string agentID, string supervisorID, string subOrdinateID, string cardno,
            string chqno, string fromdate, string todate, string remarks)
        {
            try
            {
                string intQueryType = "4";
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_Get_update_ChqBookData;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("@RefNo", string.IsNullOrEmpty(refNo) ? "" : refNo);
                    parameters.Add("@UserID", string.IsNullOrEmpty(agentID) ? "0" : agentID);
                    parameters.Add("@supervisorID", string.IsNullOrEmpty(supervisorID) ? "0" : supervisorID);
                    parameters.Add("@subOrdinateID", string.IsNullOrEmpty(subOrdinateID) ? "0" : subOrdinateID);
                    parameters.Add("@cardno", string.IsNullOrEmpty(cardno) ? "" : cardno);
                    parameters.Add("@chqno", string.IsNullOrEmpty(chqno) ? "" : chqno);
                    parameters.Add("@frmdate", string.IsNullOrEmpty(fromdate) ? "" : fromdate);
                    parameters.Add("@todate", string.IsNullOrEmpty(todate) ? "" : todate);
                    parameters.Add("@Remarks", string.IsNullOrEmpty(remarks) ? "" : remarks);
                    parameters.Add("@intQueryType", intQueryType);

                    var result = (await connection.QueryAsync<CardChqActiveVerify>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                    return (result.Count > 0);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IList<CardChqEntity>> GetIssuedChqBookData(string refNo, string chqBookSL, string remarks, string chqStatus, string userID, string queryType)
        {
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_ChqBookCancelLostEntry;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("@strRefNo", refNo);
                    parameters.Add("@numChqBookSL", chqBookSL);
                    parameters.Add("@strRemarks", remarks);
                    parameters.Add("@intChqStatus", chqStatus);
                    parameters.Add("@intUserID", userID);
                    parameters.Add("@intQueryType", queryType);
                    
                    var result = (await connection.QueryAsync<CardChqEntity>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<long> ChequeBookCancelEntry(CardDeactiveEntity entity)
        {
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_CancelEntry;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("@numChqBookSL", entity.numChqBookSL);
                    parameters.Add("@intChqStatus", entity.intChqStatus);
                    parameters.Add("@intUserID", entity.intUserID);
                    parameters.Add("@strCancelRemarks", entity.strCancelRemarks);
                    parameters.Add("@intQueryType", entity.intQueryType);


                    var result = (await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure));
                    connection.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ChequeBookCancelEntry(IList<CardDeactiveEntity> entityList)
        {
            
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_CancelEntry;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var entity in entityList)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("@numChqBookSL", entity.numChqBookSL);
                                parameters.Add("@intChqStatus", entity.intChqStatus);
                                parameters.Add("@intUserID", entity.intUserID);
                                parameters.Add("@strCancelRemarks", entity.strCancelRemarks);
                                parameters.Add("@intQueryType", entity.intQueryType);
                                var result = await connection.ExecuteAsync(sql, parameters, transaction: transaction, commandTimeout: 0, CommandType.StoredProcedure);
                            }

                            transaction.Commit();
                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                            connection.Close();
                            throw;
                        }
                    }                       
                    
                    connection.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IList<CardChequeDeactiveVerifyEntity>> GetUserList()
        {
            var sql = "SELECT distinct	UA.UsersID AS UserID,UA.UserName + '(' + CAST(UA.UsersID as VARCHAR) + ')' AS UserName FROM tbl_CARDCHQBOOK_CustomerChqBook c INNER JOIN tbl_CARDCHQBOOK_CustomerInfo CU on C.numCustomerId = CU.numCustomerId INNER JOIN tbl_CARDCHQBOOK_CustomerChqBookDetail d ON c.numChqBookSL = d.numChqBookSL LEFT JOIN [CSPM].[CSPM].[dbo].[Users] UA ON UA.[UsersId]=d.intCancelBy where UA.UsersID is not NULL";
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var result = await connection.QueryAsync<CardChequeDeactiveVerifyEntity>(sql, commandType: CommandType.Text);
                    connection.Close();
                    return result.OrderBy(x => x.UserName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        public async Task<IList<CardChequeDeactiveVerifyEntity>> GetPendingCancelChequeBookData( string checkerId,string frmDate,string toDate)
        {
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_CancelVerifyData;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    // parameters.Add("@MakerUserID", makerId);
                   // parameters.Add("@CardNumber", checkerId);
                    parameters.Add("@FromDate", frmDate);
                    parameters.Add("@ToDate", toDate);
                   
                    var result = await connection.QueryAsync<CardChequeDeactiveVerifyEntity>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IList<CardChequeDeactiveVerifyEntity>> CancelVerifyCheck(string numChqBookSL, string checkerId)
        {
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_CancelVerifyCheck;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@numChqBookSL", numChqBookSL);
                    parameters.Add("@numChqNo", checkerId);
                    var result = await connection.QueryAsync<CardChequeDeactiveVerifyEntity>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CancelVerifyEntry(CardChequeDeactiveVerifyEntity entity)
        {
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_CancelVerifyEntry;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@numChqBookSL", entity.numChqBookSL);
                    parameters.Add("@numChqNo", entity.numChqNo);
                    parameters.Add("@intUserID", entity.intUserID);
                    parameters.Add("@strCancelRemarks", entity.strCancelRemarks);
                    var result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IList<CardChqReport>> CardChqActiveDeactiveReport(string cardNumber, string chqueNumber, string fromDate, string toDate, string userID, string reportType)
        {
            try
            {
                var sql = DatabaseProcedure.CardChqProcedure.SP_CARDCHQBOOK_Report_Active_Deactive;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@CardNumber", cardNumber);
                    parameters.Add("@ChqueNumber", chqueNumber);
                    parameters.Add("@FromDate", fromDate);
                    parameters.Add("@ToDate", toDate);
                    parameters.Add("@UserID", userID);
                    parameters.Add("@ReportType", reportType);
                    
                    var result = await connection.QueryAsync<CardChqReport>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
