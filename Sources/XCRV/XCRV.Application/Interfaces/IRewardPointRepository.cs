using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IRewardPointRepository
    {
        Task<RewardPoint> CurrentMonthStmtRPBal(string Foracid);
        Task<TuesdayRewardPoint> GetRewardPointByDebitCard(string cardno, string fdate, string tdate);
        Task<IEnumerable<TuesdayRewardPoint>> GetRewardPointByDebitCardOnDate(string cardno, string OnDate);
        Task<TuesdayRewardPoint> GetRewardPointByCreditCard(string cardno, string fdate, string tdate);
        Task<TuesdayRewardPoint> GetRewardPointByCreditCardOnDate(string cardno, string OnDate);
        Task<IEnumerable<TuesdayRewardContact>> GetCardHolderContactNo(string cardno);
        Task<TuesdayRewardContact> GetCardValidate(string cardno); 
    }
}
