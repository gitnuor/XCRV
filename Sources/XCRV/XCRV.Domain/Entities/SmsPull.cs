using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class SmsPull
    {
        public string MobileNo { get; set; }
        public string RequestString { get; set; }
        public string ResponseString { get; set; }
        public string RequestTime { get; set; }
    }
}
