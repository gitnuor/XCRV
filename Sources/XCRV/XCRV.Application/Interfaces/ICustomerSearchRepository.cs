using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.CommonEnums;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICustomerSearchRepository : IGenericRepository<CustomerSearch>
    {
        Task<IList<CustomerSearch>> SearchCustomerBySearchCriteria(CustomerSearchType searchType, string searchString, string isStatementTrue, string extensiveType,string _pbsFlag);
        Task<IList<CustomerSearch>> getAsthaStatus(string custid, string isStatementTrue);

        Task<IList<CustomerGurantorStaticData>> SearchByCustomerGurantorStatic(string searchString);
        Task<IList<FinStatementDetails>> SearchByTranId(string TranId, DateTime valueDate);
        Task<IList<DepositSummary>> SearchCustomerDeposit(string searchString);
        Task<IList<CurrentAccSummary>> SearchCustomerCurrent(string searchString);
        Task<IList<TermLoanDisbursment>> SearchTermLoanDisbursment(string searchString);
        Task<IList<TermLoanPerformance>> SearchTermLoanPerformance(string searchString);
        Task<IList<Customer360Summary>> SearchCust360Summary(string searchString);
        Task<IList<FundedLoan>> SearchFundedLoanSummary(string searchString);
        Task<IList<NonFunded>> SearchNonFundedLoanSummary(string searchString);
        Task<IList<CompositeLimit>> SearchCompositeLoanSummary(string searchString);
        Task<IList<LiabilitiesSummary>> SearchLiabilitiesSummary(string searchString);
        Task<IList<CardIssuence>> SearchcardIssuenceData(string searchString);
        Task<IList<CreditPerformance>> SearchcardPerformanceData(string searchString);
        Task<IList<GraphData>> getGraphData(string searchString, string param);
        Task<IList<GraphData2>> getGraphData2(string searchString, string param);
        Task<IList<GraphData3>> getGraphData3(string searchString, string param);
        Task<IList<GraphData4>> getGraphData4(string searchString, string param);
        Task<string> GetProbasiFlagByUser(string custId);
    }
}
