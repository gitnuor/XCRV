using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class FinStatementDetails
    {
        public string cust_id { get; set; }
        public string acct_name { get; set; }
        public string acct_crncy_code { get; set; }
        public string address { get; set; }
        public string foracid { get; set; }
        public string account_type { get; set; }
        public string value_date { get; set; }
        public string tran_particular { get; set; }
        public string instrmnt_num { get; set; }
        public string withdraw { get; set; }
        public string deposit { get; set; }
        public string balance { get; set; }
        public string tran_id { get; set; }
        public string instrmnt_type { get; set; }
        public string Withdrawl { get; set; }

        //SP_MINI_STATEMENT

    }
}
