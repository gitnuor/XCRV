using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ITransactionDetailsRepository : IGenericRepository<TransactionDetails>
    {
        Task<IList<TransactionDetails>> GetTransactionDetails(string custid,DateTime pdtimeFromDate, DateTime pdtimeToDate);
        Task<IList<AccountADCTransaction>> GetADCTransactionDetails(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate);
        Task<IList<AccountADCTransaction>> GetAccountTransactionDetails(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate);
        Task<FinStatementDetails> GetCustInfo(string accno);
        Task<IList<FinStatementDetails>> GetFinTransactionDetails(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate);
        Task<IEnumerable<InterestDetails>> GetAccInterestDetails(string accno);
        Task<IEnumerable<InterestDetails>> GetAccInterestBreakup(string accno);
        Task<IEnumerable<InterestDetails>> GetAccRelatedParty(string accno);
        Task<IList<FinMiniStatement>> GetFinMiniStatementDetails(string acno);
        Task<IList<ATMTransaction>> GetATMTransactionDetails(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate);
        //Task<IEnumerable<BalanceDetails>> GetBalanceDetails(string accno);
        Task<BalanceDetails> GetBalanceDetails(string accno);
        Task<IndividualTransaction> GetIndividualTranDetails(string accountNo, string tranID, DateTime date);
    }
}
