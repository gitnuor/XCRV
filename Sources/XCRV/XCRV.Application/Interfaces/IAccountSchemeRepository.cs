using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IAccountSchemeRepository : IGenericRepository<TermDepositScheme>
    {
        Task<IEnumerable<TermDepositScheme>> GetTermDepositSchemByAcno(string acno);
        Task<IEnumerable<CaSaAccountInfo>> GetCaSaAccountInfoByAcno(string acno);
        Task<EffectiveBal> GetCustomerBal(string accno, DateTime pdtimeFromDate, DateTime pdtimeToDate);
        Task<EffectiveBal> GetAccBal(string accno);
        Task<IEnumerable<AcMobile>> GetAcMobile(string pstrACNO, string pstrMobNo, string pstrQeryType);
        Task<IEnumerable<AccSignatory>> GetACCSignatoryInfo(string pstrACNO);
    }
}
