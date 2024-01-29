using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICustomerCollateralRepository:IGenericRepository<CustomerCollateral>
    {   
        Task<IList<CustomerCollateral>> getCustomerCollateral(string custid);
        Task<CustomerCollateral> GetCustomerCollateralByCustid(string stringCif);
    }
}
