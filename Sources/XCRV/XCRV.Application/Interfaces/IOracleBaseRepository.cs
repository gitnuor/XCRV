using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IOracleBaseRepository
    {
        Task<DateTime> GetLastUpdatedDate();
        Task<bool> IsAccountAccessableByUser(string accno, string username);

        Task<string> GetAccountSchemCodeByAccountNumber(string accountNo);
        Task<string> GetSchemCodeByATMCardNumber(string accountNo);

        Task<IList<string>> GetDebitCardByCustomerNumber(string custNo);


    }
}
