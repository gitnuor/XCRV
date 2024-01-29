using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class OutwardCheque
    {
        public string CHEQU_NO { get; set; }
        public string ZON_CODE_DATE { get; set; }
        public string BANK_BRANCH_NAME { get; set; }
        public string INSTRMNT_AMT { get; set; }
        public string STATUS_FLG { get; set; }
        public string REJ_TYPE { get; set; }
    }

    public class InwardCheque
    {
        public string TRAN_DATE { get; set; }
        public string CHQ_NO { get; set; }
        public string PAYEE_NAME { get; set; }
        public string VALUE_DATE { get; set; }
        public string SOL_ID { get; set; }
        public string TRAN_AMT { get; set; }

    }
}
