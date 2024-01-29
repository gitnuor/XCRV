using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class CaSaViewModel
    {
        public CaSaAccountInfo CaSaAccount { get; set; }
        public UnclearedChqDr UnclearedChqDr { get; set; }
        public UnclearedChqCr UnclearedChqCr { get; set; }
        public RewardPoint RewardPoint { get; set; }
        public IList<BalanceDetails> BalanceDetails { get; set; }
    }
}
