using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class LoanAccountViewModel
    {
        public LoanAccountInfo LoanAccountInfo { get; set; }
        public IEnumerable<LoanAccountDetails> LoanAccountDetails { get; set; }
    }
}
