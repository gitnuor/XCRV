using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class DebitCardTransaction
    { 
        public string Card_No { get; set; }
        public string Tran_Date { get; set; }
        public string Tran_Amt { get; set; }
        public string Tran_Particular { get; set; }
    }
}
