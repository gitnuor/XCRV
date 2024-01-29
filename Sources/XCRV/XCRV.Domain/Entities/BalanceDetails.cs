using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class BalanceDetails
    {
        public string foracid { get; set; }
        public string acct_crncy_code { get; set; }
        public string sol_id { get; set; }
        public string acct_name { get; set; }
        public string clr_bal_amt { get; set; }
        public string sanct_lim { get; set; }
        public string drwng_power { get; set; }
        public string utilised_amt { get; set; }
        public string dacc_lim { get; set; }
        public string lien_amt { get; set; }
        public string OverDueLiability { get; set; }
        public string system_reserved_amt { get; set; }
        public string un_clr_bal_amt { get; set; }
        public string EffAvailAmt { get; set; }
        public string adhoc_lim { get; set; }
        public string single_tran_lim { get; set; }
        public string clean_adhoc_lim { get; set; }
        public string clean_single_tran_lim { get; set; }
        public string CustStatus { get; set; }
    }
}
