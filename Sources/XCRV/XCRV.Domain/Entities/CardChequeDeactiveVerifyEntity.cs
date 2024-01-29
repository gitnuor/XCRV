using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class CardChequeDeactiveVerifyEntity
    {
        public string nric_number { get; set; }

        public string strCusName { get; set; }

        public string strCardNo { get; set; }

        public string strChqBookName { get; set; }

        public string numChqName { get; set; }

        public string ChequeStatus { get; set; }

        public string ChequeCancelStatus { get; set; }

        public string intChqStatus { get; set; }

        public string dtChqStsChangeDate { get; set; }

        public string intCancelBy { get; set; }

        public string dtCancelDate { get; set; }

        public string UserName { get; set; }

        public string numChqBookSL { get; set; }

        public string numChqNo { get; set; }

        public string strCancelRemarks { get; set; }

        public string intUserID { get; set; }
        public string UserID { get; set; }

    }
}
