using System.Collections.Generic;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class NomineeViewModel
    {
        public string AccountNumber { get; set; }
        public IList<Nominee> Nominees { get; set; }
    }
}
