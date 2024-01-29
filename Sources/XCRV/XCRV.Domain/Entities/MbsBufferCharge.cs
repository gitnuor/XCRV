using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class MbsBufferCharge
    {
        public string McbAccountNo { get; set; }
        public string ChargeBufferedAmount { get; set; }
    }

    public class MbsBufferInterest
    {
        public string IpbAccountNo { get; set; }
        public string IpbDueInterestOs { get; set; }
        public string IpbPenalAmountOs { get; set; }
        public string IpbDueIntProvision { get; set; }
    }

    public class FinacleMbsMapping
    {
        public string FINAccno { get; set; }
        public string MBSAccno { get; set; }
    }

    

    
}
