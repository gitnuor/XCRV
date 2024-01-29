using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICustomerLimitRepository:IGenericRepository<CustomerLimit>
    {
        Task<IList<CustomerLimit>> getCustomerLimit(string isStatementTrue, string custid);
        Task<CustomerLimit> GetCustomerLimitByCustid(string stringCif);

    }
}
