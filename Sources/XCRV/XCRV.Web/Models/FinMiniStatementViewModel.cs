using System.Collections.Generic;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class FinMiniStatementViewModel
    {
        public RewardPoint rewardPointinfo { get; set; }
        public EffectiveBal accBal { get; set; }
        public IList<FinMiniStatement> miniStatementDetails { get; set; }
    }
}
