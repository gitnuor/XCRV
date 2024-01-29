using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public  class DepositSummary
    {
        public string foracid { get; set; }
        public string type_of_ac { get; set; }
        public Decimal face_value { get; set; }
        public string format_face_value { get { return string.Format("{0:N2}", face_value); } }
        public string interest_rate { get; set; }
        public Decimal present_value { get; set; }
        public string format_present_value { get { return string.Format("{0:N2}", present_value); } }
        public Decimal maturity_value { get; set; }
        public string format_maturity_value { get { return string.Format("{0:N2}", maturity_value); } }
        public DateTime open_date { get; set; }
        public string open_date_Formatted { get { return open_date.ToString("dd-MMM-yyyy"); } }
        public DateTime maturity_date { get; set; }
        public string maturity_date_Formatted { get { return maturity_date.ToString("dd-MMM-yyyy"); } }
        public string lien_status { get; set; }
    }
}
