using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface IUnclearedChqRepository : IGenericRepository<UnclearedChqDr>
    {
        Task<IEnumerable<UnclearedChqDr>> GetUnclearedChqAmtDr(string acno);
        Task<IEnumerable<UnclearedChqCr>> GetUnclearedChqAmtCr(string acno);
    }
}
