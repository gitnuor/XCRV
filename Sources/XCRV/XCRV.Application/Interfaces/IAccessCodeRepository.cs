using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IAccessCodeRepository
    {
        Task<IList<FinacleUser>> GetFinacleUserList();

        Task<IList<FinacleUser>> GetAccessInfoList();

        Task<int> SaveAccessInfo(FinacleUser accessCode, string userName);

        Task<FinacleUser> GetIsExistAccessCode(FinacleUser accessCode);

        Task<int> UpdateAccessCode(FinacleUser accessCode, string userName);

        Task<int> DeleteAccessCode(FinacleUser accessCode, string userName);
    }
}
