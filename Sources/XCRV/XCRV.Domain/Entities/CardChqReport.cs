using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class CardChqReport
    {
        public string cardNo { get; set; }
        public string nric_number { get; set; }
        public string cusName { get; set; }
        public string chequeStatus { get; set; }
        public string numChqNo { get; set; }
        public string date_time { get; set; }
        public string agentID { get; set; }
        public string agentRemarks { get; set; }
        public string superVisorRemarks { get; set; }
        public string superVisorID { get; set; }
        public string checkedDate { get; set; }
    }
}
