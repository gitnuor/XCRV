using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.CommonEnums
{
    public enum CommonEnum
    {
    }

    public enum CustomerSearchType
    {
        SP_SEARCH_CUST_BY_CUST_ID_HR,
        SP_SEARCH_CUST_BY_ACNO_HR,
        SP_SEARCH_CUST_BY_ATM_HR,
        SP_EXTENSIVE_SEARCH,
        SP_SEARCH_CUST_BY_NAME_HR,
        SP_EXTENSIVE_SEARCH_PBS,
        DebitCard,
        CreditCard
    }
}
