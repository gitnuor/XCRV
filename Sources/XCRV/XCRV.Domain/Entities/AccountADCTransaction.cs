using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class AccountADCTransaction
    {
        public string tran_id { get; set; }
        public string tran_date { get; set; }
        public string tran_part { get; set; }
        public string deposit { get; set; }
        public string withdraw { get; set; }

    }
}
