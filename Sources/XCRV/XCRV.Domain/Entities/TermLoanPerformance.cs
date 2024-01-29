using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class TermLoanPerformance
    {
        public string dpd { get; set; }
        public string average_delays_in_payment { get; set; }
        public string highest_dpd { get; set; }
        public string broken_emi { get; set; }
        public string broken_emi_percentage { get; set; }
        public string count_30_dpd { get; set; }
        public string no_of_emi_due { get; set; }
        public string emi_paid { get; set; }
        public decimal interest_amount { get; set; }
        public string format_interest_amount { get { return string.Format("{0:N2}", interest_amount); } }
        public string penal_amount { get; set; }
        public string cl_status { get; set; }
       
    }
}
