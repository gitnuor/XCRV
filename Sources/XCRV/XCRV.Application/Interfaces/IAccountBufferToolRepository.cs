using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IAccountBufferToolRepository
    {
        Task<IEnumerable<AccountBufferTool>> GetAccountBuffer(string acno);
    }
}
