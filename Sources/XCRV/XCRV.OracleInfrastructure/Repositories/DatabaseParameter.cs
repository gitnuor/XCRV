using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.OracleInfrastructure.Repositories
{
    public static class DatabaseConnection
    {
        public static readonly string XCRVFinConnection = "XCRVFinConnection";
        public static readonly string FinacleReportDB = "FinacleReportDB";
        public static readonly string SqlCRMConnection = "SqlCRMConnection";
        public static readonly string SqlCSPMConnection = "SqlCSPMConnection";
        public static readonly string OraCardProStageConnection = "OraCardProStageConnection";
        public static readonly string SqlSMSConnection = "SqlSMSConnection"; 
        public static readonly string RewardPointConnection = "RewardPointConnection";
        public static readonly string SqlMBSConnectionBranch = "SqlMBSConnectionBranch";
        public static readonly string SqlMBSConnectionHO = "SqlMBSConnectionHO";
        public static readonly string DbConnectionStringCardCHQ = "DbConnectionStringCardCHQ";
        public static readonly string DbOrclConnectionStringCHQ = "DbOrclConnectionStringCHQ";
        public static readonly string DbConnectionStringOracleCardpro = "DbConnectionStringOracleCardpro";
        public static readonly string CardProCustomer360Analyzer = "CardProCustomer360Analyzer";
        

        public static readonly string SqlMBSConnectionArchive = "SqlMBSConnectionArchive";
        public static readonly string InhouseIBDBConnection = "InhouseIBDBConnection";
        public static readonly string DbConnectionStringIRIS = "DbConnectionStringIRIS";
        public static readonly string DbConnectionStringCredit = "DbConnectionStringCredit";

        public static readonly string OraCardProArcvConnection = "OraCardProArcvConnection";
    }

    internal static class DatabasePackage
    {
        internal static readonly string FINACAL_PACKAGE_NAME = "XCRV360V2.";
        internal static readonly string CARDPRO_PACKAGE_NAME = "XCRV360V2_CARD.";
        internal static readonly string CARD_CHQ_PACKAGE_NAME = "crv_chq.CHQ.";
        internal static readonly string QMS_CARDPRO_PACKAGE_NAME = "cardpro.CARD_CHQ.";
        internal static readonly string CardPro_XCRV360_CUSTOM_ANALYZER = "REPORT.XCRV360_CUSTOM_ANALYZER.";

        internal static readonly string OraCardProArcvConnectionPackageName = "CARDPRO.XCRV360V3.";

    }

    public static class DatabaseProcedure
    {
        internal static class FinacalProcedure
        {
            internal static readonly string SP_GET_TDS_INFO = "SP_GET_TDS_INFO";
            internal static readonly string SP_GET_SBA_CCA_INFO = "SP_GET_SBA_CCA_INFO";
            internal static readonly string SP_TRANSACTION_CURRENT = "SP_TRANSACTION_CURRENT";

            internal static readonly string SP_CUST_LIMIT_TOTAL = "SP_CUST_LIMIT_TOTAL";
            internal static readonly string SP_CUST_COLLATERAL_S = "SP_CUST_COLLATERAL_S";


            internal static readonly string SP_CUST_GENERAL_DETAILS_SELECT = "SP_CUST_GENERAL_DETAILS_SELECT";
            internal static readonly string SP_INITIATOR_BY_CIF = "SP_INITIATOR_BY_CIF";
            internal static readonly string SP_GUARANTOR_BY_CIF = "SP_GUARANTOR_BY_CIF";
            internal static readonly string SP_SI_INFO = "SP_SI_INFO";
            internal static readonly string SP_MULT_LC = "SP_MULT_LC";
            internal static readonly string SP_AVG_BAL_RPT = "SP_AVG_BAL_RPT";
            internal static readonly string SP_CUST_PERS_INFO = "SP_CUST_PERS_INFO";
            internal static readonly string SP_TOT_INT_CUST = "SP_TOT_INT_CUST";
            internal static readonly string SP_DEPOSIT_TOT_BAL = "SP_DEPOSIT_TOT_BAL"; 
            internal static readonly string SP_TOT_LOAN_AMT_CUST_ID = "SP_TOT_LOAN_AMT_CUST_ID";
            internal static readonly string SP_GET_PREMIUM_AC = "SP_GET_PREMIUM_AC";
            internal static readonly string SP_AVG_BAL_6_MON_TOT = "SP_AVG_BAL_6_MON_TOT";
            internal static readonly string SP_AVG_BAL_12_MON_TOT = "SP_AVG_BAL_12_MON_TOT";
            internal static readonly string SP_AVG_BAL_12_MON_BY_SCHM = "SP_AVG_BAL_12_MON_BY_SCHM";
            internal static readonly string SP_DEPOSIT_BAL_BY_CUST_ID = "SP_DEPOSIT_BAL_BY_CUST_ID";
            internal static readonly string SP_DA_BO_LAA_BY_CUST_ID = "SP_DA_BO_LAA_BY_CUST_ID";
            internal static readonly string SP_LC_INFO = "SP_LC_INFO";
            internal static readonly string SP_CUST_LIM_DTLS_HR = "SP_CUST_LIM_DTLS_HR";
            internal static readonly string SP_CUST_AGE_CLASSIFICATION = "SP_CUST_AGE_CLASSIFICATION";
            internal static readonly string SP_TRAN_FREQ_LAST_MON_BY_CIF = "SP_TRAN_FREQ_LAST_MON_BY_CIF";
            internal static readonly string SP_CAA_BY_CUST_ID_SELECT = "SP_CAA_BY_CUST_ID_SELECT";
            internal static readonly string SP_TDA_SELECT = "SP_TDA_SELECT";
            internal static readonly string SP_LAA_BY_CUST_ID_SELECT = "SP_LAA_BY_CUST_ID_SELECT";
            internal static readonly string SP_SBA_BY_CUST_ID_SELECT = "SP_SBA_BY_CUST_ID_SELECT";
            internal static readonly string SP_SEARCH_CUST_BY_ACNO_HR = "SP_SEARCH_CUST_BY_ACNO_HR"; 
            internal static readonly string SP_SEARCH_CUST_BY_ACNO_PBS = "SP_SEARCH_CUST_BY_ACNO_PBS";
            internal static readonly string SP_SEARCH_CUST_BY_CUST_ID_HR = "SP_SEARCH_CUST_BY_CUST_ID_HR";
            internal static readonly string SP_SEARCH_CUST_BY_CUST_ID_PBS = "SP_SEARCH_CUST_BY_CUST_ID_PBS"; 
            internal static readonly string SP_SEARCH_CUST_BY_ATM_PBS = "SP_SEARCH_CUST_BY_ATM_PBS"; 
            internal static readonly string SP_SEARCH_CUST_BY_ATM_HR = "SP_SEARCH_CUST_BY_ATM_HR";
            internal static readonly string SP_SEARCH_CUST_BY_NAME_PBS = "SP_SEARCH_CUST_BY_NAME_PBS"; 
            internal static readonly string SP_SEARCH_CUST_BY_NAME_HR = "SP_SEARCH_CUST_BY_NAME_HR";
            internal static readonly string SP_CUST_GUARANT_DATA = "SP_CUST_GUARANT_DATA";
            internal static readonly string SP_CUST_DEPOSIT_SUMMARY = "SP_CUST_DEPOSIT_SUMMARY";
            internal static readonly string SP_CUST_CURRENT_SUMMARY = "SP_CUST_CURRENT_SUMMARY";
            internal static readonly string SP_CUST_TERM_LOAN_DISBURSMENT = "SP_CUST_TERM_LOAN_DISBURSMENT";
            internal static readonly string SP_CUST_TERM_LOAN_Performance = "SP_CUST_TERM_LOAN_Performance";
            internal static readonly string SP_CUST_360_SUMMARY = "SP_CUST_360_SUMMARY";
            internal static readonly string SP_FUNDED_Loan_SUMMARY = "SP_FUNDED_Loan_SUMMARY";
            internal static readonly string SP_NonFUNDED_Loan_SUMMARY = "SP_NonFUNDED_Loan_SUMMARY";
            internal static readonly string SP_Composite_Loan_SUMMARY = "SP_Composite_Loan_SUMMARY";
            internal static readonly string SP_Liabilities_SUMMARY = "SP_Liabilities_SUMMARY";
            internal static readonly string SP_CREDIT_CARD_ISSUANCE = "SP_CREDIT_CARD_ISSUANCE";
            internal static readonly string SP_CREDIT_CARD_PERFORMANCE = "SP_CREDIT_CARD_PERFORMANCE";
            internal static readonly string SP_GRAPH_DATA = "SP_GRAPH_DATA";
            internal static readonly string SP_TRANS_BY_TRANSID = "SP_TRANS_BY_TRANSID";
            
            internal static readonly string SP_EXTENSIVE_SEARCH = "SP_EXTENSIVE_SEARCH";
            internal static readonly string SP_EXTENSIVE_SEARCH_PBS = "REPORT.SP_EXTENSIVE_SEARCH_PBS_CORE";
            internal static readonly string SP_astha_status = "REPORT.SP_astha_status";
            internal static readonly string GET_LAA_AC_INFO_CORE = "GET_LAA_AC_INFO_CORE";
            internal static readonly string CRV_LOAN_DETAILS = "CRV_LOAN_DETAILS";
            internal static readonly string SP_CRV_LOAN_REPAYMENTHISTORY = "SP_CRV_LOAN_REPAYMENTHISTORY";
            internal static readonly string SP_LOAN_PAYOFF_Info = "SP_LOAN_PAYOFF_Info";
            internal static readonly string SP_LOAN_PAYOFF = "SP_LOAN_PAYOFF";
            internal static readonly string SP_LOAN_GURANTOR_CORE = "SP_LOAN_GURANTOR_CORE";
            internal static readonly string SP_LOAN_DOC = "SP_LOAN_DOC";
            internal static readonly string SP_AC_LIMIT_DETAILS = "SP_AC_LIMIT_DETAILS";
            internal static readonly string SP_SBACCANOM = "SP_SBACCANOM";
            internal static readonly string SP_GET_INFO_BY_ACCNO = "SP_GET_INFO_BY_ACCNO";
            internal static readonly string SP_getAccAccessibilityStatus = "SP_getAccAccessibilityStatus";
            internal static readonly string SP_BOD = "SP_BOD";
            internal static readonly string SP_CORP_ACCT_STMT_V2 = "SP_CORP_ACCT_STMT_V2";
            internal static readonly string SP_STATEMENT_CUST_DETAILS = "SP_STATEMENT_CUST_DETAILS";
            internal static readonly string SP_TRANSACTION = "SP_TRANSACTION";
            internal static readonly string SP_TRANSACTION_CDCI_CREDITCARD = "SP_TRANSACTION_CDCI_CREDITCARD";
            internal static readonly string SP_UNCLEARED_CHQ_AMT_CR = "SP_UNCLEARED_CHQ_AMT_CR";
            internal static readonly string SP_UNCLEARED_CHQ_AMT_DR = "SP_UNCLEARED_CHQ_AMT_DR";
            internal static readonly string SP_FinStatement = "SP_FinStatement";
            internal static readonly string SP_AC_INT_DTL_INQ = "SP_AC_INT_DTL_INQ";
            internal static readonly string SP_INT_BREAKUP = "SP_INT_BREAKUP";
            internal static readonly string SP_RELATED_PARTY_INQUERY = "SP_RELATED_PARTY_INQUERY";
            internal static readonly string SP_MINI_STATEMENT = "SP_MINI_STATEMENT";
            internal static readonly string SP_ATM_TRANSACTION = "SP_ATM_TRANSACTION";
            internal static readonly string SP_GET_INFO_BY_ATM = "SP_GET_INFO_BY_ATM";
            internal static readonly string sp_account_balance_details = "sp_account_balance_details";
            internal static readonly string SP_TRAN_DTL_INDI = "SP_TRAN_DTL_INDI";


            internal static readonly string SP_REPEAT_LOAN_CUSTOMER = "SP_REPEAT_LOAN_CUSTOMER";
            internal static readonly string SP_REPEAT_LOAN_PROPRIETOR = "SP_REPEAT_LOAN_PROPRIETOR";
            internal static readonly string SP_REPEAT_LOAN_INFO = "SP_REPEAT_LOAN_INFO";
            internal static readonly string SP_REPEAT_LOAN_GUARANTOR = "SP_REPEAT_LOAN_GUARANTOR";
            internal static readonly string BYFFER_TOOL = "BYFFER_TOOL";
            internal static readonly string SP_MOBILE_VS_ACCOUNT = "SP_MOBILE_VS_ACCOUNT";
            internal static readonly string SP_AC_MOBILE = "SP_AC_MOBILE";
            internal static readonly string SP_DEBIT_CARD_DETAILS = "SP_DEBIT_CARD_DETAILS";
            internal static readonly string SP_DEBIT_CARD_TRANSACTION = "SP_DEBIT_CARD_TRANSACTION";
            internal static readonly string SP_GET_DEBIT_CARD_NO = "SP_GET_DEBIT_CARD_NO";
          
            internal static readonly string SP_ACC_SIGNATORY_INFO = "SP_ACC_SIGNATORY_INFO";

            internal static readonly string SP_GET_FIN_USER_LIST = "SP_GET_FIN_USER_LIST";
            internal static readonly string SP_GET_ACCESSINFO_LIST = "SP_GET_ACCESSINFO_LIST";
            internal static readonly string SP_XCRV_ACCESSINFOINSERT = "SP_XCRV_ACCESSINFOINSERT";
            internal static readonly string SP_CHECK_ACCESSINFO_IF_EXIST = "SP_CHECK_ACCESSINFO_IF_EXIST";
            internal static readonly string SP_XCRV_ACCESSINFOUPDATE = "SP_XCRV_ACCESSINFOUPDATE";
            internal static readonly string SP_XCRV_ACCESSINFODELETE = "SP_XCRV_ACCESSINFODELETE";

            internal static readonly string SP_OUTWARD_CHECK_TOP20 = "SP_OUTWARD_CHECK_TOP20";
            internal static readonly string INWARD_CHECK_TOP20 = "INWARD_CHECK_TOP20";

            internal static readonly string SP_GET_DEBIT_CARD_NO_BY_CUSTID = "SP_GET_DEBIT_CARD_NO_BY_CUSTID";
        }

        public static class CspmProcedure
        {
            public static readonly string SP_UserInfo_forXCRV360V2 = "SP_UserInfo_forXCRV360V2";           
            public static readonly string SP_GetXCRV360V2Menu = "SP_GetXCRV360V2Menu";
            public static readonly string spGetMenuPrevilegeXCRV = "spGetMenuPrevilegeXCRV";
            public static readonly string XCRV360_V2_CustomerMemoInsert = "XCRV360_V2_CustomerMemoInsert";
            public static readonly string XCRV360_V2_CustomerMemoUpdate = "XCRV360_V2_CustomerMemoUpdate";
            public static readonly string XCRV_Log_Details = "XCRV_Log_Details";

            public static readonly string XCRV360_V2_CustomerMemoGetByCustomeID = "XCRV360_V2_CustomerMemoGetByCustomeID";
        }

        public static class TaraRewardPoint 
        {
            public static readonly string SP_TUESDAY_CARDREWARD_POINT = "SP_TUESDAY_CARDREWARD_POINT";
            public static readonly string SP_TUESDAY_CARDREWARD_POINT_ONDate = "SP_TUESDAY_CARDREWARD_POINT_ONDate";
            public static readonly string SP_Debit_Card_Info = "SP_Debit_Card_Info";
            public static readonly string VALIDATE_CREDIT_CARD = "VALIDATE_CREDIT_CARD";
            public static readonly string SP_TUESDAY_CCARDREWARD_POINT = "SP_TUESDAY_CCARDREWARD_POINT";
            public static readonly string SP_TUESDAY_CCARDREWARD_POINT_ONDate = "SP_TUESDAY_CCARDREWARD_POINT_ONDate";


        }

        internal static class CardProProcedure
        {
            internal static readonly string SP_GET_TDS_INFO = "SP_CUST_INFO_LIST";
            internal static readonly string SP_CUST_INFO_CUSTOMER_IDNO = "SP_CUST_INFO_CUSTOMER_IDNO";
        }


        public static class BblSmsProcedure
        {
            public static readonly string SP_XCRV360V2_GetTopTenPullSMS = "SP_XCRV360V2_GetTopTenPullSMS";
            public static readonly string SP_XCRV360V2_GetTopTenPushSMS = "SP_XCRV360V2_GetTopTenPushSMS";
            public static readonly string SP_XCRV360V2_GetUserRequestLogFinacle = "SP_XCRV360V2_GetUserRequestLogFinacle";
        }

        public static class MbsProcedure
        {
            public static readonly string SP_GetBufferChargeAmount = "SP_GetBufferChargeAmount";
            public static readonly string SP_GetBufferInterestRelated = "SP_GetBufferInterestRelated";
            public static readonly string SP_Statement_Voucher = "SP_Statement_Voucher";
            public static readonly string SP_Statement_AccountName_Voucher = "SP_Statement_AccountName_Voucher";
            public static readonly string SP_Statement_AccountName = "SP_Statement_AccountName";
        }

        public static class CardChqProcedure
        {
            public static readonly string SP_GET_CHQ_Leaf_Search_Limit = "SP_GET_CHQ_Leaf_Search_Limit";
            public static readonly string SP_CARDCHQBOOK_Get_update_ChqBookData = "SP_CARDCHQBOOK_Get_update_ChqBookData";
            public static readonly string SP_CARDCHQBOOK_ChqBookCancelLostEntry = "SP_CARDCHQBOOK_ChqBookCancelLostEntry";
            public static readonly string SP_CARDCHQBOOK_CancelEntry = "SP_CARDCHQBOOK_CancelEntry";
            public static readonly string SP_CARDCHQBOOK_CancelVerifyData = "SP_CARDCHQBOOK_CancelVerifyData";
            public static readonly string SP_CARDCHQBOOK_CancelVerifyCheck = "SP_CARDCHQBOOK_CancelVerifyCheck";
            public static readonly string SP_CARDCHQBOOK_CancelVerifyEntry = "SP_CARDCHQBOOK_CancelVerifyEntry";
            public static readonly string SP_CARDCHQBOOK_Report_Active_Deactive = "SP_CARDCHQBOOK_Report_Active_Deactive";
        }

        public static class CrvCardChqProcedure
        {
            public static readonly string Crv_ChqCustInfo = "Crv_ChqCustInfo";
            public static readonly string Crv_Chqstatus_range = "Crv_Chqstatus_range";
            public static readonly string Crv_Chqstatusupdate_range = "Crv_Chqstatusupdate_range";
            public static readonly string Crv_proctmp_chqlog = "Crv_proctmp_chqlog";
            public static readonly string CRV_CHQUSER_REQLOG_GET = "CRV_CHQUSER_REQLOG_GET";
            public static readonly string CRV_CHQUSER_REQLOG_VERIFY = "CRV_CHQUSER_REQLOG_VERIFY";
            public static readonly string CRV_CHECK_VERIFY = "CRV_CHECK_VERIFY";
            public static readonly string CRV_CHQ_STOP_REPORT = "CRV_CHQ_STOP_REPORT";
            public static readonly string CRV_CHQ_STOP_REPORT_SUM = "CRV_CHQ_STOP_REPORT_SUM";
        }

        public static class QmsCardProProcedure
        {
            public static readonly string Card_Chq_Cust_Info = "Card_Chq_Cust_Info";
        }

        public static class InhouseIBDBConnectionProcedure
        {
            public static readonly string SP_UserInfo_Get_By_AccountNo = "SP_UserInfo_Get_By_AccountNo";
            public static readonly string SP_Get_Transactions_By_AccNo = "SP_Get_Transactions_By_AccNo";
        }

        public static class OraCardProArcvConnectionProcedure
        {
            public static readonly string SP_CardPro_GET_PSS_BY_CRDNO = "SP_CardPro_GET_PSS_BY_CRDNO";
            public static readonly string SP_CardPro_GET_PSD_BY_CRDNO = "SP_CardPro_GET_PSD_BY_CRDNO";
            public static readonly string SP_CardPro_GET_CHS_BY_CRDNO = "SP_CardPro_GET_CHS_BY_CRDNO";
            public static readonly string SP_CardPro_GET_TRN_BY_CRDNO = "SP_CardPro_GET_TRN_BY_CRDNO";


        }
    }
}
