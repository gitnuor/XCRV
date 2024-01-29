using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XCRV.Web.Models
{
    public class ChqReqLogRequest
    {
        public string accountNo { get; set; }
        public string frmDate { get; set; }
        public string toDate { get; set; }
        public string makerUserID { get; set; }
        public string checkerUserID { get; set; }
        public string submited { get; set; }
    }

    public class ChqStopReportRequest
    {
        public string accountNo { get; set; }
        public string chqNo { get; set; }
        public string frmDate { get; set; }
        public string toDate { get; set; }
        public string userId { get; set; }
        public string reportType { get; set; }
        public string submited { get; set; }
    }
}
