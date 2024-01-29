using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class UnclearedChqDr
    {
        public string Acct_Number { get; set; }
        public string System_Reserved_Amt { get; set; }
        public string Chq_Number { get; set; }
        public string Chq_Amt { get; set; }
        public string Schm_Code { get; set; }
    }
}
