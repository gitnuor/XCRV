using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IDebitCardRepository
    {
        Task<IEnumerable<DebitCardDetail>> GetDebitCardDetailInfo(string cardNo);
        Task<IEnumerable<DebitCardTransaction>> GetDebitCardTransaction(string pstrAccNo, string IsStatementTrue);
        Task<IEnumerable<DebitCardDetail>> GetDebitCardNo(string pstrAccNo);
        Task<IEnumerable<SIinformation>> GetSIinformation(string custId);
        Task<IEnumerable<LCinformation>> GetLCinformation(string accno);
        Task<LCinformation> GetLcDetails(string accno,string LcNo);
        Task<IEnumerable<PremimunCustAvgBal>> GetAvgPremiumBal(string seachType, string seachString);
        Task<PremimumPersonalInfo> GetPremimumPersonelInfo(string custid);
        Task<PremimumPersonalInfo> GetPremimumAccountTotal(string custid);
        Task<PremimumPersonalInfo> GetPremimumTotDepositBal(string custid);
        Task<PremimumPersonalInfo> GetPremimumTotLoanBal(string custid);
        Task<IEnumerable<PremimumPersonalInfo>> GetPremimumDeposits(string custid);
        Task<IEnumerable<PremimumAsset>> GetPremimumAssets(string custid);
        Task<PremimumAsset> GetPremimumTotAssetBal(string custid);
        Task<PremimumAsset> Get6monthavgTotal(string custid);
        Task<PremimumAsset> Get1yearavgTotal(string custid);
        Task<IEnumerable<PremimumAvgBal>> GetDepMove12MonBySchm(string custid);
        Task<PremimumAc> GetPremimumCustId(string custid);
    }
}
