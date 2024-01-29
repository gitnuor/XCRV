using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class LoanPayoffInfo
    {
        public string Cur_Entity_Name { get; set; }
        public string Cur_Val { get; set; }
        public string Fu_Entity_Name { get; set; }
        public string Fu_Val { get; set; }
    }

    public class LoanPayOff
    {
        public string Foracid { get; set; }
        public string Acct_Name { get; set; }
        public string ostd { get; set; }
        public string Unapplied_Interest { get; set; }
        public string Date_Open { get; set; }
        public string Interest_Rate { get; set; }
    }
}
