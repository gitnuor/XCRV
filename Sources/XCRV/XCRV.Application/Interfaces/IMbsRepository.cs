using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IMbsRepository
    {
        Task<IEnumerable<MbsBufferCharge>> GetBufferChargeAmount(string acno);
        Task<IEnumerable<MbsBufferInterest>> GetBufferInterestRelated(string acno);
        Task<IEnumerable<MbsAccountInfo>> GetMbsStatementAccountName(string acno);
        Task<IEnumerable<MbsAccountInfo>> GetMbsHoStatementAccountName(string acno);
        Task<IEnumerable<MbsTransaction>> GetMbsTransaction(string acno, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<MbsTransaction>> GetMbsHoTransaction(string acno, DateTime fromDate, DateTime toDate);
        Task<FinacleMbsMapping> GetMbsAccountMapping(string acno);

    }
}
