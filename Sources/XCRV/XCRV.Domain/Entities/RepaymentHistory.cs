using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class RepaymentHistory
    {
        public string Shdl_Num { get; set; }
        public string Due_Date { get; set; }
        public string Dmd_Amt { get; set; }
        public string Principle { get; set; }
        public string Interest { get; set; }
        public string Collections { get; set; }
        public string Last_Adj_Date { get; set; }
    }
}
