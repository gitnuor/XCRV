using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class FundedLoan
    {
        public string ac_no { get; set; }
        public string scheme { get; set; }
        public decimal disbursement_amount { get; set; }
        public string format_disbursement_amount { get { return string.Format("{0:N2}", disbursement_amount); } }
        public decimal outstanding_amount { get; set; }
        public string format_outstanding_amount { get { return string.Format("{0:N2}", outstanding_amount); } }
        public DateTime disbursement_date { get; set; }
        public string disbursement_date_Formatted { get { return disbursement_date.ToString("dd-MMM-yyyy"); } }
        public DateTime maturity_date { get; set; }
        public string maturity_date_Formatted { get { return maturity_date.ToString("dd-MMM-yyyy"); } }
        public DateTime adjustment_date { get; set; }
        public string adjustment_date_Formatted { get { return adjustment_date.ToString("dd-MMM-yyyy"); } }
        public string tenor { get; set; }
        public string dpd { get; set; }
        public decimal interest_amount { get; set; }
        public string format_interest_amount { get { return string.Format("{0:N2}", interest_amount); } }
        public string cl_status { get; set; }
    }
}
