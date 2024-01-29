using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class MbsTransaction
    {
        public string actTransNo { get; set; }
        public string atotransdate { get; set; }
        public string part { get; set; }
        public string dr { get; set; }
        public string cr { get; set; }
        public string balance { get; set; }
    }
}
