using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class CardCustomerInformation
    {
        public string CB_ID_TYPE { get; set; }

        public string CB_CUSTOMER_IDNO { get; set; }

        public string CB_CARDHOLDER_NO { get; set; }

        public string NAME { get; set; }

        public string CB_DOB { get; set; }

        public string CB_SEX { get; set; }

        public string CB_MOTHER_NAME { get; set; }

        public string CB_MARITAL_STATUS { get; set; }

        public string CB_MOBILE_NO { get; set; }

        public string CB_EMAIL { get; set; }

        public string CB_CUST_LIMIT { get; set; }

        public string CB_PLASTIC_CD { get; set; }

        public string CB_CENTRE_CD { get; set; }

        public string CB_BILL_ADDR_CD { get; set; }

        public string HomeAddress { get; set; }

        public string CompanyAddress { get; set; }

        public string PR_DESC { get; set; }

        public string CB_CARD_PRDCT_GROUP { get; set; }

        public string CB_COMPANY_NAME { get; set; }

        public string CB_EXPIRY_CCYYMM { get; set; }

        public string STATUS { get; set; }

        public string PhotoUrl { get; set; }
        public string SignatureUrl { get; set; }
    }
}
