using System.Collections.Generic;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class IbViewModel
    {
        public string AccountNumber { get; set; }
        public string ErrorMessage { get; set; }
        public IbCustomerInfo CustInfo { get; set; }
        public List<IbTransaction> IbTransactions { get; set; }
    }
}
