using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        Task<IReadOnlyList<Users>> GetUserInfoByUserName(string userName);
        Task<IReadOnlyList<Users>> GetXcrvUserName();
        Task<string> GetProjectName(string projectId);
        Task<IReadOnlyList<Users>> GetProbasiByUser(string userId);
        //Task<string> GetProbasiByUser(string userId);
    }
}
