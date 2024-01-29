using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IMenuPrevilegeRepository : IGenericRepository<MenuPrevilege>
    {
        Task<IReadOnlyList<MenuPrevilege>> GetByUserIdAndGroupIdAndProjectId(int userId, int groupId, int projectId);
    }
}
