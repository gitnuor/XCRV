using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class LoanAccountLimit
    {
        public string Foracid { get; set; }
        public string Acct_Crncy_Code { get; set; }
        public string Sol_Id { get; set; }
        public string Acct_Name { get; set; }
        public string Balance { get; set; }
        public string Sanct_Lim { get; set; }
        public string RecCreatedDate { get; set; }
        public string ApplicableDate { get; set; }
        public string ExpiryDate { get; set; }
        public string LoanDocDate { get; set; }
        public string ReviewDate { get; set; }
        public string Status { get; set; }
    }
}
