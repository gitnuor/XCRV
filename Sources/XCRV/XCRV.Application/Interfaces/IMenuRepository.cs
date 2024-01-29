using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        Task<IReadOnlyList<Menu>> GetAllAsync();
    }
}
