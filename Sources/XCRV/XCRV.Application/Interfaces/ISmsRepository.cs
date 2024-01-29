using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ISmsRepository
    {
        Task<IEnumerable<SmsPush>> GetTopTenPushSMS(string mobileNo, string schmCode);
        Task<IEnumerable<SmsPull>> GetTopTenPullSMS(string mobileNo, string schmCode);
        Task<IEnumerable<SmsPull>> GetSMSLog(string mobileNo, string schmCode);
    }
}
