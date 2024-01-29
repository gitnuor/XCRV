using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICardProRepository
    {
        Task<CradProStatementSummary> GetPreviousStatementSummary(string pstrCustID, string pstrMonthYear, string pstrPrevMonthYear, string pACCT_TYPE);
        Task<IList<CardProStatementDetails>> GetPreviousStatementDetails(string pstrCARDNO, string P_MON_YEAR, string pACCT_TYPE);
        Task<CardProCreditHistory> GetCreditHistorySummary(string pstrCardNo, string acct_type);
        Task<TransactionSummary> GetTransactionSummary(string CardNo, DateTime fromDate, DateTime toDate);
    }
}
