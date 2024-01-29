using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XCRV.Web.Helpers
{
    public class MenuMapping
    {
        public string Url { get; set; }
        public string RewriteUrl { get; set; }

        public MenuMapping(string url, string rewriteUrl)
        {
            this.Url = url;
            this.RewriteUrl = rewriteUrl;
        }
    }

    public class MenuMappingList
    {
        public IList<MenuMapping> MenuMappings { get; set; }

        public MenuMappingList()
        {
            MenuMappings = new List<MenuMapping>();

            MenuMappings.Add(new MenuMapping("CustomerSearch.aspx", "/CustomerSearch/Index"));
            MenuMappings.Add(new MenuMapping("CustomerSearchPBS.aspx", "/CustomerSearchPBS/Index"));
            MenuMappings.Add(new MenuMapping("CustomerGeneralDetails.aspx", "/CustomerGeneralDetails/Index"));
            MenuMappings.Add(new MenuMapping("CustomerMatrix.aspx", "/CustomerMatrix/Index"));
            MenuMappings.Add(new MenuMapping("CustomerLimit.aspx", "/CustomerLimit/Index"));
            MenuMappings.Add(new MenuMapping("CustomerCollateralDetails.aspx", "/CustomerCollateralDetails/Index"));
            MenuMappings.Add(new MenuMapping("AccountTransaction.aspx", "/AccountTransaction/Index"));
            MenuMappings.Add(new MenuMapping("TransactionDetails.aspx", "/TransactionDetails/Index"));
            MenuMappings.Add(new MenuMapping("Account_ADC_Transaction.aspx", "/Account_ADC_Transaction/Index"));
            MenuMappings.Add(new MenuMapping("FinStatement.aspx", "/FinStatement/Index"));
            MenuMappings.Add(new MenuMapping("FinMiniStatement.aspx", "/FinMiniStatement/Index"));
            MenuMappings.Add(new MenuMapping("IndividualTransaction.aspx", "/IndividualTransaction/Index"));
            MenuMappings.Add(new MenuMapping("AccountBasicInfo.aspx", "/CaSaAccount/BasicInfo"));
            MenuMappings.Add(new MenuMapping("SBACCAInformation.aspx", "/CasaAccount/Index"));
            MenuMappings.Add(new MenuMapping("DPSFDRInformation.aspx", "/TermDepositScheme/Index"));
            MenuMappings.Add(new MenuMapping("ATMTransaction.aspx", "/ATMTransaction/Index"));
            MenuMappings.Add(new MenuMapping("BalanceDetails.aspx", "/BalanceDetails/Index"));
            MenuMappings.Add(new MenuMapping("RelatedParty.aspx", "/RelatedParty/Index"));
            MenuMappings.Add(new MenuMapping("AccountInterestDetails.aspx", "/AccountInterestDetails/Index"));
            MenuMappings.Add(new MenuMapping("InterestBreakup.aspx", "/InterestBreakup/Index "));
            MenuMappings.Add(new MenuMapping("LAAInformations.aspx", "/LoanAccount/Index"));
            MenuMappings.Add(new MenuMapping("LoanPayOff.aspx", "/LoanAccount/LoanPayoff"));
            MenuMappings.Add(new MenuMapping("CollectedLoanDoc.aspx", "/LoanAccount/LoanDocument"));
            MenuMappings.Add(new MenuMapping("LoanGuarantorInfo.aspx", "/LoanAccount/LoanGuarantor"));
            MenuMappings.Add(new MenuMapping("LoanACLimitDetails.aspx", "/LoanAccount/LoanAccountLimit"));
            MenuMappings.Add(new MenuMapping("DigitalLoanApplication.aspx", "/LoanAccount/Application"));
            MenuMappings.Add(new MenuMapping("MBSAccountBuffer.aspx", "/Mbs/AccountBuffer"));
            MenuMappings.Add(new MenuMapping("MBSTransaction.aspx", "/Mbs/Transaction"));
            MenuMappings.Add(new MenuMapping("MBSStatement.aspx", "/Mbs/Statement"));
            MenuMappings.Add(new MenuMapping("FinacleMBSAccountNumber.aspx", "/Mbs/FinacleMbsAccNo"));
            MenuMappings.Add(new MenuMapping("CardCustomarGeneralDetail.aspx", "/CreditCard/CardCustomerDetails"));
            MenuMappings.Add(new MenuMapping("CardDetailInfo.aspx", "/CreditCard/CardRelatedInformation"));
            MenuMappings.Add(new MenuMapping("LiveLAAInformations.aspx", "/LoanAccount/LiveInfo"));
            MenuMappings.Add(new MenuMapping("AccountBufferTool.aspx", "/AccountBufferTool/Index"));
            MenuMappings.Add(new MenuMapping("SMSBankingDetails.aspx", "/SmsBanking/Details"));
            MenuMappings.Add(new MenuMapping("SMSBankingLog.aspx", "/SmsBanking/Log"));
            MenuMappings.Add(new MenuMapping("DebitCardDetails.aspx", "/DebitCard/Details"));
            MenuMappings.Add(new MenuMapping("AccessCodeEntry.aspx", "/AccessCode/Entry"));
            MenuMappings.Add(new MenuMapping("FinStatement_HR.aspx", "/FinStatementHR/Index"));
            MenuMappings.Add(new MenuMapping("SIinformation.aspx", "/SIinformation/Index"));
            MenuMappings.Add(new MenuMapping("Lcinformation.aspx", "/Lcinformation/Index"));
            MenuMappings.Add(new MenuMapping("PremiumCustAvgBalRpt.aspx", "/PremiumCustAvgBalRpt/Index"));
            MenuMappings.Add(new MenuMapping("CustomerDashboard.aspx", "/PremiumCustomerDashboard/Index"));

            MenuMappings.Add(new MenuMapping("/TaraTuesdayRewardPoints/Index", "/TaraTuesdayRewardPoints/Index"));
            MenuMappings.Add(new MenuMapping("ChequeStop.aspx", "/ChequeStop/Maker"));
            MenuMappings.Add(new MenuMapping("ChqVerifyBySupervisor.aspx", "/ChequeStop/Verify"));
            MenuMappings.Add(new MenuMapping("ChequeStopReport.aspx", "/ChequeStop/Report"));
            MenuMappings.Add(new MenuMapping("CrdChqActvMaker.aspx", "/CardChq/Active"));
            MenuMappings.Add(new MenuMapping("CrdChqActvChecker.aspx", "/CardChq/ActiveVerify"));
            MenuMappings.Add(new MenuMapping("CardChequeDeactive.aspx", "/CardChq/Deactive"));
            MenuMappings.Add(new MenuMapping("CardChequeDeactiveVerify.aspx", "/Cardchq/DeactiveVerify"));
            MenuMappings.Add(new MenuMapping("CardChequeReport.aspx", "/CardChq/Report"));
            
            MenuMappings.Add(new MenuMapping("FinStatementExcel.aspx", "/FinStatementExcel/Index"));
            MenuMappings.Add(new MenuMapping("singlecustomerviewdashboard.aspx", "/CustomerDashboard/Index"));
            MenuMappings.Add(new MenuMapping("TravelQuota.aspx", "/TravelQuota/Index"));
            MenuMappings.Add(new MenuMapping("ClientGuarantorSummary.aspx", "/ClientGuarantorSummary/Index"));

            //MenuMappings.Add(new MenuMapping("PreviousStatementDetails.aspx", "/CardPro/PreviousStatementDetails"));
            //MenuMappings.Add(new MenuMapping("CreditHistorySummary.aspx", "/CardPro/CreditHistorySummary"));
            //MenuMappings.Add(new MenuMapping("CardholderMemos.aspx", "/CardProArcv/CardHolderMemo"));
            //MenuMappings.Add(new MenuMapping("TransactionSummary.aspx", "/CardProArcv/TransactionSummary"));


        }
    }
}
