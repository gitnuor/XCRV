using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class CardChqActiveVerify
    {        
        public string NRIC_Number { get; set; }

        public string strcusname { get; set; }

        public string strCardNo { get; set; }

        public string cb_mobile_no { get; set; }

        public string strChqBookName { get; set; }

        public string intNumberOfLeaf { get; set; }

        public string numChqStartNo { get; set; }

        public string numChqEndNo { get; set; }

        public string IssueDate { get; set; }

        public string ChequeUsed { get; set; }

        public string ChequeAvailable { get; set; }

        public string strRemarks { get; set; }

        public string numChqBookSL { get; set; }

        public string BookStatus { get; set; }

        public string intActivatedBy { get; set; }

        public string activitydate { get; set; }

        public string UserName { get; set; }
        
    }
}
