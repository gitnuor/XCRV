using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class LoanApplicationViewModel
    {
        public CorporateCustomer CustomerBasicInfo { get; set; }

        public IList<Proprietor> LoanProprietors { get; set; }

        //public LoanAccountInfo LoanAccountInfo { get; set; }

        public IList<LoanAccountInfo> LoanAccountInfoList { get; set; }

        public IList<Guarantor> Guarantors { get; set; }
    }
}
