using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class AccountBufferTool
    {
        public string Acct_Num { get; set; }
        public string Chrg_Adj_Acct { get; set; }
        public string Tran_Date { get; set; }
        public string Adj_Seq { get; set; }
        public string Chrg_Amt { get; set; }
        public string Chrg_Adj_Amt { get; set; }
        public string Chrg_Waive_Amt { get; set; }
        public string Last_Adj_Dt { get; set; }
        public string Particulars { get; set; }
        public string Charg_Coll_Flg { get; set; }
        public string Due_Amt { get; set; }
    }
}
