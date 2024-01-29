using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public  interface IInhouseIBRepository
    {
        Task<IList<IbCustomerInfo>> GetIbCusotmerInfo(string acno);
        Task<IList<IbTransaction>> GetTransactionByAccNo(string acno, string RecordToReturn);
    }
}
