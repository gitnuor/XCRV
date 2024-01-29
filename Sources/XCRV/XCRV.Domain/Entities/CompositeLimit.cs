using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class CompositeLimit
    {
        public string product { get; set; }
        public string limit { get; set; }
        public decimal amount_disbursed { get; set; }
        public string format_amount_disbursed { get { return string.Format("{0:N2}", amount_disbursed); } }
        public decimal amount_adjusted { get; set; }
        public string format_amount_adjusted { get { return string.Format("{0:N2}", amount_adjusted); } }
        public decimal amount_outstanding { get; set; }
        public string format_amount_outstanding { get { return string.Format("{0:N2}", amount_outstanding); } }
        public decimal amount_overdue { get; set; }
        public string format_amount_overdue { get { return string.Format("{0:N2}", amount_overdue); } }
        public string no_of_disbursed { get; set; }
        public string no_adjusted { get; set; }
        public string no_of_outstanding { get; set; }
        public string contract_went_dpd { get; set; }
        public string no_in_overdue { get; set; }
        public string no_30_plus_dpd { get; set; }
        public string max_dpd { get; set; }
    }
}
