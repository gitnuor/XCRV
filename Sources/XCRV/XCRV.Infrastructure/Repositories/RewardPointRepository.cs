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
    public class RewardPointRepository : IRewardPointRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _rewardPointServiceUrl;
        private readonly string _rewardPointServiceUser;
        private readonly string _rewardPointServicePW;
        private readonly string _connectionString;

        RewardPointApi.DebitCardRewardPointAPIClient DebitCardRewardPointAPIClient;

        public RewardPointRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._rewardPointServiceUrl = _configuration.GetSection("AppSettings").GetSection("RewardPointServiceUrl").Value;
            this._rewardPointServiceUser = _configuration.GetSection("AppSettings").GetSection("RewardPointServiceUser").Value;
            this._rewardPointServicePW = _configuration.GetSection("AppSettings").GetSection("RewardPointServicePW").Value;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.RewardPointConnection);


            DebitCardRewardPointAPIClient = new RewardPointApi.DebitCardRewardPointAPIClient(RewardPointApi.DebitCardRewardPointAPIClient.EndpointConfiguration.DebitCardRewardPointAPIQSPort, _rewardPointServiceUrl);

        }


        public async Task<RewardPoint> CurrentMonthStmtRPBal(string Foracid)
        {
            try
            {
                RewardPoint rewardPoint = new RewardPoint();
                RewardPointApi.CurrentMonthStmtRPBalRequestRequestHeader header = new RewardPointApi.CurrentMonthStmtRPBalRequestRequestHeader();
                RewardPointApi.CurrentMonthStmtRPBalRequestRequestBody body = new RewardPointApi.CurrentMonthStmtRPBalRequestRequestBody();
                RewardPointApi.CurrentMonthStmtRPBalRequest request = new RewardPointApi.CurrentMonthStmtRPBalRequest();
                //RewardPointApi.CurrentMonthStmtRPBalResponse response;
                RewardPointApi.CurrentMonthStmtRPBalResponseTranDetails result;

                //header
                header.userName = _rewardPointServiceUser;
                header.userPassword = _rewardPointServicePW;
                //body
                body.accountno = Foracid;
                //request
                request.RequestHeader = header;
                request.RequestBody = body;
                //response
                var response = await DebitCardRewardPointAPIClient.CurrentMonthStmtRPBalAsync(request);


                if (response.CurrentMonthStmtRPBalResponse.ResponseHeader.responseMessage == "Success")
                {
                    foreach (RewardPointApi.CurrentMonthStmtRPBalResponseTranDetails item in response.CurrentMonthStmtRPBalResponse.ResponseBody)
                    {
                        rewardPoint.FORACID = item.accountNumber;
                        rewardPoint.MonthlyEarned = item.monthlyEarned;
                        rewardPoint.MonthlyRedeem = item.monthlyRedeem;
                        rewardPoint.MonthlyExpiry = item.monthlyExpiry;
                        rewardPoint.NextMonthExpiry = item.nextMonthExpiry;
                        rewardPoint.AvailableBalance = item.availableBalance;
                        rewardPoint.ResponseMessege = response.CurrentMonthStmtRPBalResponse.ResponseHeader.responseMessage.ToString();
                    }
                }
                else
                {
                    rewardPoint.ResponseMessege = response.CurrentMonthStmtRPBalResponse.ResponseHeader.responseMessage.ToString();

                }
                return rewardPoint;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TuesdayRewardPoint> GetRewardPointByDebitCard(string cardno,string fdate,string tdate)
        {
            var sql = DatabaseProcedure.TaraRewardPoint.SP_TUESDAY_CARDREWARD_POINT;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@P_CARDNO", cardno);
                    parameters.Add("@FromDate", fdate);
                    parameters.Add("@ToDate", tdate);
                    var result =( await connection.QueryAsync<TuesdayRewardPoint>(sql, parameters,commandTimeout:0, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex) { 
              throw ex;
            }
           
        }

        public async Task<IEnumerable<TuesdayRewardPoint>> GetRewardPointByDebitCardOnDate(string cardno, string OnDate)
        {
            var sql = DatabaseProcedure.TaraRewardPoint.SP_TUESDAY_CARDREWARD_POINT_ONDate;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@P_CARDNO", cardno);
                    parameters.Add("@ONDate", OnDate);                   
                    var result = (await connection.QueryAsync<TuesdayRewardPoint>(sql, parameters, commandTimeout: 0, commandType: CommandType.StoredProcedure));
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }    
        public async Task<IEnumerable<TuesdayRewardContact>> GetCardHolderContactNo(string cardno)
        {
            var sql = DatabaseProcedure.TaraRewardPoint.SP_Debit_Card_Info;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@p_cardno", cardno);                                      
                    var result = (await connection.QueryAsync<TuesdayRewardContact>(sql, parameters, commandTimeout: 0, commandType: CommandType.StoredProcedure));
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<TuesdayRewardContact> GetCardValidate(string cardno)
        {
            var sql = DatabaseProcedure.TaraRewardPoint.VALIDATE_CREDIT_CARD;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@p_cardno", cardno);
                    var result = (await connection.QueryAsync<TuesdayRewardContact>(sql, parameters, commandTimeout: 0, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

  public async Task<TuesdayRewardPoint> GetRewardPointByCreditCard(string cardno,string fdate,string tdate)
        {
            var sql = DatabaseProcedure.TaraRewardPoint.SP_TUESDAY_CCARDREWARD_POINT;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@P_CARDNO", cardno);
                    parameters.Add("@FromDate", fdate);
                    parameters.Add("@ToDate", tdate);
                    var result =( await connection.QueryAsync<TuesdayRewardPoint>(sql, parameters,commandTimeout:0, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex) { 
              throw ex;
            }
           
        }
        public async Task<TuesdayRewardPoint> GetRewardPointByCreditCardOnDate(string cardno, string OnDate)
        {
            var sql = DatabaseProcedure.TaraRewardPoint.SP_TUESDAY_CCARDREWARD_POINT_ONDate;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@P_CARDNO", cardno);
                    parameters.Add("@ONDate", OnDate);
                    var result = (await connection.QueryAsync<TuesdayRewardPoint>(sql, parameters, commandTimeout: 0, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
     

    }
}
