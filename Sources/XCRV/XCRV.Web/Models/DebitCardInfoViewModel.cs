using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class DebitCardInfoViewModel
    {
        public DebitCardDetail DebitCardDetail { get; set; }
        public IList<DebitCardTransaction> DebitCardTransactions { get; set; }
        public DebitCardInfoViewModel()
        {
            DebitCardDetail = new DebitCardDetail();
            DebitCardTransactions = new List<DebitCardTransaction>();
        }

    }
}
