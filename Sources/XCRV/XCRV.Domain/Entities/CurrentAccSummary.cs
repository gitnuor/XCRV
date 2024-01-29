using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class CurrentAccSummary
    {
        public string foracid { get; set; }
        public decimal credit_sum { get; set; }
        public string format_credit_sum { get { return string.Format("{0:N2}", credit_sum); } }
        public decimal average_balance { get; set; }
        public string format_average_balance { get { return string.Format("{0:N2}", average_balance); } }
        public string account_status { get; set; }
    }
}
