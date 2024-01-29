using System.Collections.Generic;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class ChequeDetailsViewModel
    {
        public string AccountNumber { get; set; }
        public string ErrorMessage { get; set; }
        public IList<InwardCheque> InwardChequeList { get; set; }
        public IList<OutwardCheque> OutwardChequeList { get; set; }
    }
}
