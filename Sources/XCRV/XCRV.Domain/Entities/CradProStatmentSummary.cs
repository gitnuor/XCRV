using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class CradProStatementSummary
    {
        public string BasicCardNo { get; set; }
        public string Name { get; set; }
        public string CreditLimit { get; set; }
        public string OpneingBalance { get; set; }
        public string StatementDate { get; set; }
        public string MinimumPayment { get; set; }  
        public string CurrentBalance { get; set; }      
        public string DueDate { get; set; }
    }

    public class CardProStatementDetails
    {
        public string Card_Holder_Number { get; set; }
        public string TRXN_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string TRXN_CODE { get; set; }
        public string MCC { get; set; }
        public string DESCRIPTION { get; set; }
        public string AMOUNT { get; set; }
    }
}
