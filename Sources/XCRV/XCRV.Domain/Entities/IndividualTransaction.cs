using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class IndividualTransaction
    {
        public string tran_id { get; set; }
        public string tran_date { get; set; }
        public string value_date { get; set; }
        public string tran_type { get; set; }
        public string foracid { get; set; }
        public string tran_amt { get; set; }
        public string TRAN_PARTICULAR { get; set; }
        public string TRAN_RMKS { get; set; }
        public string sol_id { get; set; }
        public string part_tran_type { get; set; }
        public string entry_user_id { get; set; }
        public string pstd_user_id { get; set; }
        public string vfd_user_id { get; set; }      

    }
}
