using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class ChqReqLog
    {
        public string Foracid { get; set; }
        public string Acct_Name { get; set; }
        public string FrmChqno { get; set; }
        public string ToChqno { get; set; }
        public string ActionCount { get; set; }
        public string Particular { get; set; }
        public string v_Datetime { get; set; }
        public string User_Id { get; set; }
        public string Remarks { get; set; }
        public string IsVerify { get; set; }
        public string Acid { get; set; }

    }

    
}
