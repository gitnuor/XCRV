using System.Collections.Generic;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class LoanDocumentDashboardViewModel
    {
        public string AccountNumber { get; set; }
        public string ErrorMessage { get; set; }
        public IList<LoanDocument> DocList { get; set; }
    }
}
