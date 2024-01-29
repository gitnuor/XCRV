using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class ChqStopReport
    {
        public string Account_Number { get; set; }
        public string Account_Title { get; set; }
        public string Cheque_No { get; set; }
        public string Cheque_Status { get; set; }
        public string Date_Time { get; set; }
        public string Agent_ID { get; set; }
        public string Agent_Remarks { get; set; }
        public string Supervisor_Remarks { get; set; }
        public string Supervisor_ID { get; set; }
        public string Check_Date { get; set; }

        
    }

    public class ChqStopSummaryReport
    {
        public string M_Date { get; set; }
        public string Cheque_No { get; set; }

    }
}
