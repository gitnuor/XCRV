using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class CustomerLimit
    {
        public string cust_id { get; set; }
        public string name { get; set; }
        public string tot_lim { get; set; }
        public string tot_ostd { get; set; }
        public string tot_fun_lia { get; set; }
        public string acno { get; set; }
        public string lmt { get; set; }
        public string clr_bal_amt { get; set; }
        public string snctn_date { get; set; }
        public string lim_exp_date { get; set; }
        public string schm_type { get; set; }
        public string crncy { get; set; }


    }
}
