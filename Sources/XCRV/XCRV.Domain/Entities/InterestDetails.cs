using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class InterestDetails
    {
        //
        public string FORACID { get; set; }
        public string acct_crncy_code { get; set; }
        public string sol_id { get; set; }
        public string ACCT_NAME { get; set; }
        public string nrml_interest_amount_cr { get; set; }
        public string interest_calc_upto_date_cr { get; set; }
        public string last_interest_run_date_cr { get; set; }
        public string nrml_accrued_amount_cr { get; set; }
        public string nrml_booked_amount_cr { get; set; }
        public string accrued_upto_date_cr { get; set; }
        public string last_accrual_run_date_cr { get; set; }
        public string booked_upto_date_cr { get; set; }
        public string last_book_run_date_cr { get; set; }
        public string nrml_interest_amount_dr { get; set; }
        public string interest_calc_upto_date_dr { get; set; }
        public string last_interest_run_date_dr { get; set; }
        public string nrml_accrued_amount_dr { get; set; }
        public string nrml_booked_amount_dr { get; set; }
        public string accrued_upto_date_dr { get; set; }
        public string last_accrual_run_date_dr { get; set; }
        public string booked_upto_date_dr { get; set; }
        public string last_book_run_date_dr { get; set; }

        //SP_INT_BREAKUP 
        public string ADDNL_INTEREST_AMOUNT_DR { get; set; }
        public string NRML_AMORTIZED_AMOUNT_CR { get; set; }
        public string NRML_INT_SUSPENSE_AMT_DR { get; set; }
        public string PENAL_INT_SUSPENSE_AMT_DR { get; set; }
        public string PENAL_ACCRUED_AMOUNT_DR { get; set; }
        public string PENAL_BOOKED_AMOUNT_DR { get; set; }
        public string PENAL_INTEREST_AMOUNT_DR { get; set; }
        public string ADDNL_ACCRUED_AMOUNT_DR { get; set; }
        public string ADDNL_BOOKED_AMOUNT_DR { get; set; }

        //SP_RELATED_PARTY_INQUERY      
        public string CLR_BAL_AMT { get; set; }
        public string ACCT_POA_AS_SRL_NUM { get; set; }
        public string CUST_ID { get; set; }
        public string ACCT_POA_AS_NAME { get; set; }
        public string ACCT_POA_AS_REC_TYPE { get; set; }
        public string CUST_RELTN_CODE { get; set; }
        public string ACCT_POA_AS_AMT_ALWD { get; set; }


    }
}
