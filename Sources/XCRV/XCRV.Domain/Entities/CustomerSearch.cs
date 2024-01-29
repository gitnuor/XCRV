using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class CustomerSearch
    {
        public string sol_id { get; set; }
        public string foracid { get; set; }
        public string cust_id { get; set; }
        public string cust_name { get; set; }
        public string acct_name { get; set; }
        public string acct_short_name { get; set; }
        public string schm_code { get; set; }
        public string schm_type { get; set; }
        public string schm_desc { get; set; }
        public string clr_bal_amt { get; set; }
        public string mobile_no { get; set; }
        public string nid { get; set; }
        public string snid { get; set; }
        public string tin{get; set; }
        public string astha_status { get; set; }
    }
}
