using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class NonFunded
    {
        public string lc_bg_number { get; set; }
        public string product { get; set; }
        public decimal issue_amount { get; set; }
        public string format_issue_amount { get { return string.Format("{0:N2}", issue_amount); } }
        public decimal outstanding { get; set; }
        public string format_outstanding { get { return string.Format("{0:N2}", outstanding); } }
        public DateTime issue_date { get; set; }
        public string issue_date_Formatted { get { return issue_date.ToString("dd-MMM-yyyy"); } }
        public DateTime expiry_date { get; set; }
        public string expiry_date_Formatted { get { return expiry_date.ToString("dd-MMM-yyyy"); } }
        public DateTime adjustment_date { get; set; }
        public string adjustment_date_Formatted { get { return adjustment_date.ToString("dd-MMM-yyyy"); } }
        public string tenor { get; set; }
        public string margin { get; set; }
        public decimal commision_amount { get; set; }
        public string format_commision_amount { get { return string.Format("{0:N2}", commision_amount); } }
        public string beneficiary_name { get; set; }

    }
}
