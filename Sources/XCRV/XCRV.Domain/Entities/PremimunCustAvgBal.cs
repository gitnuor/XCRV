using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class PremimunCustAvgBal
    {
        public string cust_id { get; set; }
        public string foracid { get; set; }
        public string acct_name { get; set; }
        public string acct_label_desc  { get; set; }
        public string schm_type { get; set; }
        public string tdaavgbal   { get; set; }
        public string casaavgbal  { get; set; }
        
    }
}
