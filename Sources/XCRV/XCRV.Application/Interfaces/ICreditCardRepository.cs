using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICreditCardRepository : IGenericRepository<CreditCardInfo>
    {
        Task<IList<CreditCardInfo>> GetCardCustInfoList(string CustID, string CardNo, string MobileNo);
        Task<IList<CreditCardInfo>> GetCardDetailsInfoList(string CustID, string CardNo);
    }
}
