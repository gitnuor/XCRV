using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class ChqBookInfo
    {
        public string strCardNo { get; set; }
        public string numChqBookSL { get; set; }
        public string strChqBookName { get; set; }
        public string intNumberOfLeaf { get; set; }
        public string strChqPrefix { get; set; }
        public string numChqStartNo { get; set; }
        public string numChqEndNo { get; set; }
        public string issueDate { get; set; }
        public string bookStatus { get; set; }
        public string activitydate { get; set; }
        public string intActivatedBy { get; set; }
        public string intApprStatus { get; set; }
        public string chequeUsed { get; set; }
        public string chequeAvailable { get; set; }
        public string dtActivateDate { get; set; }
        public string strRemarks { get; set; }
        
    }


    public class CardChqEntity
    {
        public string numChqBookSL { get; set; }
        public string strChqBookName { get; set; }
        public string intNumberOfLeaf { get; set; }
        public string issueDate { get; set; }
        public string bookStatus { get; set; }

        public string numChqNo { get; set; }
        public string chequeStatus { get; set; }
        public string intPaymentStatus { get; set; }
        public string chequeCancelStatus { get; set; }
        public string intChqStatus { get; set; }
        public string strUserName { get; set; }
        public string dtChqStsChangeDate { get; set; }
        public string paymentDate { get; set; }
        public string strRemarks { get; set; }
        public string intChqStatusChangeBy { get; set; }
    }

    public class CardDeactiveEntity
    {
        public string numChqBookSL { get; set; }
        
        public string intChqStatus { get; set; }
        
        public string intUserID { get; set; }
        
        public string strCancelRemarks { get; set; }
        
        public string intQueryType { get; set; }

        public string paymentStatus { get; set; }
        public string chqCancelStatus { get; set; }
        public bool forFullBook { get; set; }
        public string txtChqStatus { get; set; }
    }
}
