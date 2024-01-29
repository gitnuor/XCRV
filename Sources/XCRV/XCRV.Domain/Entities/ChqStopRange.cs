using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class ChqStopRange
    {
        public string AccountNo { get; set; }
        public string ChqNo { get; set; }
        public string EndchqNo { get; set; }
        public string Status { get; set; }
        public string Struserid { get; set; }
        public string Rerarks { get; set; }
    }

    public class ChqStopRangeVerify
    {
        public string foracid { get; set; }
        public string acid { get; set; }
        public string frmChqno { get; set; }
        public string toChqno { get; set; }
        public string particular { get; set; }
        public string isverify { get; set; }
        public string User_Id { get; set; }
        public string Remarks { get; set; }
    }
}
