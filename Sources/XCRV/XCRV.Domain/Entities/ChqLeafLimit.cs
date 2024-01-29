using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class ChqLeafLimit
    {
        public int Leaf_Limit { get; set; }
        public string CHQ_Type { get; set; }
    }

    public class ChqStatus
    {
        public string Chqno { get; set; }
        public string Leaf_Status { get; set; }
    }

    public class ChqLog
    {
        public string ChqNo { get; set; }
        public string Action { get; set; }
        public string Particular { get; set; }
    }
}
