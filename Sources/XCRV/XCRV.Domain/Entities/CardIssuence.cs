using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class CardIssuence
    {
        public string cust_id { get; set; }
        public string card_name { get; set; }
        public string product { get; set; }
        public decimal limit { get; set; }
        public string format_limit { get { return string.Format("{0:N2}", limit); } }
        public decimal outstanding { get; set; }
        public string format_outstanding { get { return string.Format("{0:N2}", outstanding); } }
        public string interest_rate { get; set; }
        public string billing_date { get; set; }
        //public string billing_date_Formatted { get { return billing_date.ToString("dd-MMM-yyyy"); } }
        public decimal overdue_amount { get; set; }
        public string format_overdue_amount { get { return string.Format("{0:N2}", overdue_amount); } }
        public string sanction_date { get; set; }
        //public string sanction_date_Formatted { get { return sanction_date.ToString("dd-MMM-yyyy"); } }
        public string expiry_date { get; set; }
        //public string expiry_date_Formatted { get { return expiry_date.ToString("MMM-yy"); } }
        public decimal min_due { get; set; }
        public string format_min_due { get { return string.Format("{0:N2}", min_due); } }
        public string card_type { get; set; }
        public string security { get; set; }
        public string card_status { get; set; }
        public string first_utilzation_date { get; set; }
        //public string first_utilzation_date_Formatted { get { return first_utilzation_date.ToString("dd-MMM-yyyy"); } }
    }
}
