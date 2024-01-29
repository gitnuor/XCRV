using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class MbsAccountInfo
    {
        public string csaAccountName { get; set; }
        public string csaStatementAdd { get; set; }
        public string csaCustomerId { get; set; }
        public string acsAccStatusNotes { get; set; }

        public string coaAccName { get; set; }

        public MbsAccountInfo()
        {
            csaAccountName = string.Empty;
            csaStatementAdd = string.Empty;
            csaCustomerId = string.Empty;
            acsAccStatusNotes = string.Empty;
            coaAccName = string.Empty;
        }
    }
}
