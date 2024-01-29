using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IChequeDetailsRepository
    {
        Task<IList<OutwardCheque>> GetTop20OutwardCheques(string accNo);
        Task<IList<InwardCheque>> GetTop20InwardCheques(string accNo);
    }
}
