using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.CommonEnums;
using XCRV.Domain.Entities;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class ClientGuarantorSummaryController : BaseController
    {
        private readonly ILogger<ClientGuarantorSummaryController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public ClientGuarantorSummaryController(ILogger<ClientGuarantorSummaryController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            ClientBorrowerViewModel data = new ClientBorrowerViewModel();
            if (data.customerSearch == null)
            {
                data.customerSearch = new List<CustomerGurantorStaticData>();
            }
            return View(data);
        }

        public async Task<IActionResult> CustomerLevelStaticData(string customerId)
        {
            string seachType = "CustomerID";
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            // IList<CustomerSearch> data = new List<CustomerSearch>();
            ClientBorrowerViewModel data = new ClientBorrowerViewModel();
            string message = "Sorry!!! No Data Found!!!";

            data.customerSearch = (await _unitOfWork.CustomerSearchRepo.SearchByCustomerGurantorStatic(customerId)).ToList();

            if (data.customerSearch == null)
            {
                data.customerSearch = new List<CustomerGurantorStaticData>();
            }
           // return PartialView(data);
            return Json(new { data = data.customerSearch, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }
        public async Task<IActionResult> CIFDetails(string cif_id , string flag)
        {
            string seachType = "CustomerID";
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

           // IList<CustomerSearch> data = new List<CustomerSearch>();
            ClientBorrowerViewModel data = new ClientBorrowerViewModel();
            string message = "Sorry!!! No Data Found!!!";

            data.customer360Summary = (await _unitOfWork.CustomerSearchRepo.SearchCust360Summary(cif_id)).ToList();
            data.liabilitiesSummary = (await _unitOfWork.CustomerSearchRepo.SearchLiabilitiesSummary(cif_id)).ToList();
            data.depostSummary = (await _unitOfWork.CustomerSearchRepo.SearchCustomerDeposit(cif_id)).ToList();
            data.currentAccSummary= (await _unitOfWork.CustomerSearchRepo.SearchCustomerCurrent(cif_id)).ToList();
            data.termLoanDisbursment = (await _unitOfWork.CustomerSearchRepo.SearchTermLoanDisbursment(cif_id)).ToList();
            data.termLoanPerformance = (await _unitOfWork.CustomerSearchRepo.SearchTermLoanPerformance(cif_id)).ToList();
            data.fundedLoan = (await _unitOfWork.CustomerSearchRepo.SearchFundedLoanSummary(cif_id)).ToList();
            data.nonfundedLoan = (await _unitOfWork.CustomerSearchRepo.SearchNonFundedLoanSummary(cif_id)).ToList();
            data.compositeLimit = (await _unitOfWork.CustomerSearchRepo.SearchCompositeLoanSummary(cif_id)).ToList();         
            data.cardIssuence = (await _unitOfWork.CustomerSearchRepo.SearchcardIssuenceData(cif_id)).ToList();
            data.creditPerformance = (await _unitOfWork.CustomerSearchRepo.SearchcardPerformanceData(cif_id)).ToList();


            if (data.depostSummary.Count==0 || data.currentAccSummary.Count == 0 || data.termLoanDisbursment.Count == 0 || data.termLoanPerformance.Count == 0 || data.customer360Summary.Count == 0 || data.fundedLoan.Count == 0 || data.nonfundedLoan.Count == 0 || data.compositeLimit.Count == 0 || data.liabilitiesSummary.Count == 0)
            {
                data.depostSummary = new List<DepositSummary>();
                data.currentAccSummary = new List<CurrentAccSummary>();
                data.termLoanDisbursment = new List<TermLoanDisbursment>();
                data.termLoanPerformance = new List<TermLoanPerformance>();
                data.customer360Summary = new List<Customer360Summary>();
                data.fundedLoan = new List<FundedLoan>();
                data.nonfundedLoan = new List<NonFunded>();
                data.compositeLimit = new List<CompositeLimit>();
                data.liabilitiesSummary = new List<LiabilitiesSummary>();
               
            }
            if (data.cardIssuence.Count == 0 || data.creditPerformance.Count == 0)
            {
                data.cardIssuence = new List<CardIssuence>();
                data.creditPerformance = new List<CreditPerformance>();
            }
            if (flag != null)
            {
                return PartialView("CIFDetailsTab", data);
            }
            else {
                return PartialView(data);
            }
            
           // return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }

        //[HttpGet]
        public async Task<IActionResult> GetSummaryDataForDashBoard(string cif_id)
        {
            ClientBorrowerViewModel data = new ClientBorrowerViewModel();
            string flag1 = "C1";
            string flag2 = "C2";
            string flag3 = "C3";
            string flag4 = "G1";
            data.graphDatasCard1 = (await _unitOfWork.CustomerSearchRepo.getGraphData(flag1, cif_id)).ToList();
            data.graphDatasCard2 = (await _unitOfWork.CustomerSearchRepo.getGraphData2(flag2, cif_id)).ToList();
            data.graphDatasCard3 = (await _unitOfWork.CustomerSearchRepo.getGraphData3(flag3, cif_id)).ToList();
            data.graphDatasCard4 = (await _unitOfWork.CustomerSearchRepo.getGraphData4(flag4, cif_id)).ToList();
            //data.DailyUserActivation = Convert.ToInt16("100");
            //data.WeeklyUserActivation = Convert.ToInt16("200");
            //data.totalCustomer = Convert.ToInt16("300");

            //try
            //{
            //    DashboardDataViewModel CountUserActivitionForDashBoard = _dashboardService.CountUserActivitionForDashBoard();

            //    return Json(new { data = CountUserActivitionForDashBoard, status = "success", result = CommonAjaxResponse(Constants.MessageType.success, "Success", Constants.ResponseCode.success) });

            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("[Stack Trace]  [ " + ex.StackTrace + " ]  [Message] " + ex.Message);
            //    return Json(new { result = CommonAjaxResponse(MessageType.error, ex.Message, ResponseCode.error) });
            //}


            return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            //return PartialView("_GraphView", data);
        }


    }
}
