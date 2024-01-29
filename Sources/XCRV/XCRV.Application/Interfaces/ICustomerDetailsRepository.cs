using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICustomerDetailsRepository : IGenericRepository<CustomerDetails>
    {
        Task<CustomerDetails> GetCustomerDetailsByCif(string stringCif);
        Task<IEnumerable<Introducer>> GetInitiatorByCif(string stringCif);
        Task<IEnumerable<Guarantor>> GetGuarantorByCif(string stringCif);
        Task<IEnumerable<StandingInstruction>> GetStandingInstructionByCif(string stringCif);
    }
}
