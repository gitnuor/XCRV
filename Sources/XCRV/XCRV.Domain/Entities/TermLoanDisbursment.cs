using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class TermLoanDisbursment
    {
        public string foracid { get; set; }
        public string acct_name { get; set; }
        public string schm_desc { get; set; }
        public decimal sanction_limit { get; set; }
        public string format_sanction_limit { get { return string.Format("{0:N2}", sanction_limit); } }
        public string account_status { get; set; }

        public DateTime disbursement_date { get; set; }
        public string disbursement_dateFormatted { get { return disbursement_date.ToString("dd-MMM-yyyy"); } }
        public decimal disbursement_amount { get; set; }
        public string format_disbursement_amount { get { return string.Format("{0:N2}", disbursement_amount); } }
        public string tenor { get; set; }
        public decimal emi_amount { get; set; }
        public string format_emi_amount { get { return string.Format("{0:N2}", emi_amount); } }
        public decimal outstanding_amount { get; set; }
        public string format_outstanding_amount { get { return string.Format("{0:N2}", outstanding_amount); } }
        public decimal overdue_amount { get; set; }
        public string format_overdue_amount { get { return string.Format("{0:N2}", overdue_amount); } }
        public string number_phase_disbursement { get; set; }
        public DateTime closing_date { get; set; }
        public string closing_dateFormatted { get { return closing_date.ToString("dd-MMM-yyyy"); } }

    }
}
