using System.Collections.Generic;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class ClientBorrowerViewModel
    {
        public List<CustomerGurantorStaticData> customerSearch { get; set; }
        public List<FinStatementDetails> tranDetails { get; set; }
        public List<DepositSummary> depostSummary { get; set; }
        public List<CurrentAccSummary> currentAccSummary { get; set; }
        public List<TermLoanDisbursment> termLoanDisbursment { get; set; }
        public List<TermLoanPerformance> termLoanPerformance { get; set; }
        public List<CreditCardIssuence> creditCardIssuence { get; set; }
        public List<Customer360Summary> customer360Summary { get; set; }
        public List<FundedLoan> fundedLoan { get; set; }
        public List<NonFunded> nonfundedLoan { get; set; }
        public List<CompositeLimit> compositeLimit { get; set; }
        public List<LiabilitiesSummary> liabilitiesSummary { get; set; }
        public List<CardIssuence> cardIssuence { get; set; }
        public List<CreditPerformance> creditPerformance { get; set; }
        public List<GraphData> graphDatasCard1 { get; set; }
        public List<GraphData2> graphDatasCard2 { get; set; }
        public List<GraphData3> graphDatasCard3 { get; set; }
        public List<GraphData4> graphDatasCard4 { get; set; }

    }
}
