using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class LiabilitiesSummary
    {
        public string type { get; set; }
       // public string total_face_value { get; set; }

        public Decimal total_face_value { get; set; }
        public string format_total_face_value { get { return string.Format("{0:N2}", total_face_value);} }
        public string max_interest_rate { get; set; }
        public Decimal total_maturity_value { get; set; }
        public string format_total_maturity_value { get { return string.Format("{0:N2}", total_maturity_value); } }
        public string no_of_lien { get; set; }
        public string no_of_non_lien { get; set; }
    }
}
