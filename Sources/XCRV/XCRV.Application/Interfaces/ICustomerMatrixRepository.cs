using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICustomerMatrixRepository :IGenericRepository<CustomerMatrix>
    {
        Task<IEnumerable<CustomerMatrix>> GetTDA(string pstrCustID);
        Task<IEnumerable<CustomerMatrix>> GetLAA(string pstrCustID);
        Task<IEnumerable<CustomerMatrix>> GetSBA(string pstrCustID);
        Task<IEnumerable<CustomerMatrix>> GetCAA(string pstrCustID);
        Task<IEnumerable<CustomerTransactionFrequency>> GetTransactionFrequency(string pstrCustID);
        Task<IEnumerable<CustomerAgeClassification>> GetCustomerAgeClassification(string pstrCustID);
    }
}
