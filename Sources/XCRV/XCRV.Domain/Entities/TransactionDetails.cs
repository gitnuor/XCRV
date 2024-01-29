using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class TransactionDetails
    {
        public string tran_date { get; set; }
        public string tran_id { get; set; }
        public string tran_particular { get; set; }
        public string init_sol_id { get; set; }
        public string cheque_num { get; set; }
        public string debit_amount { get; set; }
        public string credit_amount { get; set; }
        public string entry_user { get; set; }
        public string posted_user { get; set; }
        public string verify_user { get; set; }
        public string seachString { get; set; }
        public DateTime FromDate { get; set; }
      
        public DateTime ToDate { get; set; }
    }
}
