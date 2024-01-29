using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class MbsTransactionViewModel
    {
        public MbsAccountInfo AccountInfo { get; set; }

        public IList<MbsTransaction> MbsTransactions { get; set; }

        public MbsTransactionViewModel()
        {
            AccountInfo = new MbsAccountInfo();
            MbsTransactions = new List<MbsTransaction>();
        }
    }
}
