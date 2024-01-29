using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class CreditPerformance
    {
        public string cust_id { get; set; }
        public string card_name { get; set; }
        public string avg_utilization { get; set; }
        public string eol_times { get; set; }
        public string count_of_30dpd { get; set; }
        public string count_of_60dpd { get; set; }
        public string count_of_90dpd { get; set; }
        public string count_of_90_plus_dpd { get; set; }
        public string cl_status { get; set; }
    }
}
