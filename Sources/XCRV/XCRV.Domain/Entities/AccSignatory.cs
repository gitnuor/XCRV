using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class AccSignatory
    {
        public string AcId { get; set; }
        public string Acct_Name { get; set; }
        public string Mode_Of_Oper_Code { get; set; }
        public string ApplicantsCIF { get; set; }
        public string IsPrimaryApplicant { get; set; }
    }
}
