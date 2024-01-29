using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class ATMTransaction
    {       
        public string tran_date { get; set; }
        public string tran_amt { get; set; }
        public string tran_particular { get; set; }
        public string tran_rmks { get; set; }
    }
}
