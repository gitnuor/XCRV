using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class RewardPoint
    {
        public string FORACID { get; set; }
        public string MonthlyEarned { get; set; }
        public string MonthlyRedeem { get; set; }
        public string MonthlyExpiry { get; set; }
        public string NextMonthExpiry { get; set; }
        public string AvailableBalance { get; set; }
        public string AvailableOn { get; set; }
        public string AvailableRewardPoint { get; set; }
        public string PreviousMonthBalance { get; set; }
        public string TotalEarned { get; set; }
        public string TotalRedeem { get; set; }
        public string TotalExpiry { get; set; }
        public string ResponseMessege { get; set; }
    }
}
