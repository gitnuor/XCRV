using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface INomineeRepository : IGenericRepository<Nominee>
    {
        Task<IEnumerable<Nominee>> GetCaSaNominees(string acno);
    }
}
