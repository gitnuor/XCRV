using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class Customer360Summary
    {
        public string account_status { get; set; }
        public Decimal total_sanction_limit { get; set; }
        public string format_total_sanction_limit { get { return string.Format("{0:N2}", total_sanction_limit); } }
        public Decimal total_disbursement_amount { get; set; }
        public string format_total_disbursement_amount { get { return string.Format("{0:N2}", total_disbursement_amount); } }
        public DateTime last_disbursement_date { get; set; }
        public string last_disbursement_date_Formatted { get { return last_disbursement_date.ToString("dd-MMM-yyyy"); } }
        public Decimal total_interest_amount { get; set; }
        public string format_total_interest_amount { get { return string.Format("{0:N2}", total_interest_amount); } }
        public string total_penal_amount { get; set; }
        public string max_dpd { get; set; }
        public string max_average_delays { get; set; }
        public string max_highest_dpd { get; set; }
        public string max_broken_emi { get; set; }
        public string max_broken_emi_percentage { get; set; }
    }
}
