using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class PassportEndorsementDebit
    {
        public string customerid { get; set; }
        public string ParamValue { get; set; }
        public string limit_type { get; set; }
        public string qlimit_assigned { get; set; }
        public string qusagepercentage { get; set; }
        public string qusagepercentageamount { get; set; }
        public string qlimit_amount { get; set; }

        public string qlimit_assigned_credit { get; set; }
        public string qusagepercentageamount_credit { get; set; }
        public string qlimit_amount_credit { get; set; }

        public string PassportNo { get; set; }
        public string cb_passport_no { get; set; }
        public string cb_idno { get; set; }
        public string limit_type_cr { get; set; }
        // public string qlimit_amount_credit { get; set; }
        // public string qlimit_amount_credit { get; set; }
    }
}
