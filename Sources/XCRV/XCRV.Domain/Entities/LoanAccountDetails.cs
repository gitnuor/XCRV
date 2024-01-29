using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class LoanAccountDetails
    {
        public string SchdlNo { get; set; }
        public string InterestRate { get; set; }
        public string InstallmentAmt { get; set; }
        public string TotalInstallment { get; set; }
        public string DemandRaised { get; set; }
        public string DemandToBeRaised { get; set; }
    }
}
