using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class CardChqViewModel
    {
        public CardCustomerInformation CardCustomerInformation { get; set; }
        public IList<ChqBookInfo> ChqBookInfos { get; set; }
    }

    public class CardChqBookActiveRequst
    {
        public string chqNo { get; set; }
        public string remarks { get; set; }
        public string opType { get; set; }
    }

    public class CardChqBookVerifyReqeust
    {
        public string refNo { get; set; }
        public string chqNo { get; set; }
        public string cardNo { get; set; }
        public string remarks { get; set; }
        public string userID { get; set; }
        public string frmDate { get; set; }
        public string todate { get; set; }

        public string opType { get; set; }

        public string submited { get; set; }

        
    }
        
}
