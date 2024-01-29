using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Helpers;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class ChequeStopController : BaseController
    {

        private readonly ILogger<ChequeStopController> _logger;
        private readonly IUnitOfWork _unitOfWork;       

        public ChequeStopController(ILogger<ChequeStopController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        private async Task<int> GetLeafLimitValue()
        {
            var leafLimit = await _unitOfWork.CardChqRepo.GetCardChqLeafSearchLimit("CHQ");

            if(leafLimit != null)
            {
                return leafLimit.Leaf_Limit;
            }
            return 0;
        }

        [Filters.AuthorizeActionFilter]
        public IActionResult Maker()
        {
            return View();
        }

        public async Task<ActionResult> ShowCustomerInfo(string accountNo, string leafFrom, string leafTo, string submited)
        {
            try
            {
                accountNo = HttpUtility.HtmlEncode(accountNo);
                leafFrom = HttpUtility.HtmlEncode(leafFrom);
                leafTo = HttpUtility.HtmlEncode(leafTo);
                submited = HttpUtility.HtmlEncode(submited);

                ChqCustomer chqCustomer = null;

                if (string.IsNullOrEmpty(submited))
                {
                    if (chqCustomer == null)
                    {
                        chqCustomer = new ChqCustomer();
                        chqCustomer.ChqStatusList = new List<ChqStatus>();
                    }
                    return PartialView("_CardCustomerInfo", chqCustomer);
                }

                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

                int leafLimit = await GetLeafLimitValue();


                if (string.IsNullOrEmpty(accountNo))
                {
                    ViewBag.ErrorMessage = "Account No. can not be empty.";
                }
                else if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo))
                {
                    ViewBag.ErrorMessage = "Account Number must be 13 or 16 digits & Can't contain Special/Normal characters.";
                }
                else if ( string.IsNullOrEmpty(leafFrom))
                {
                    ViewBag.ErrorMessage = "From Leaf No. can not be empty.";
                }
                else if (!this.IsNumberString(leafFrom))
                {
                    ViewBag.ErrorMessage = "From Leaf No Can't contain Special/Normal characters.";
                }
                else if (string.IsNullOrEmpty(leafTo))
                {
                    ViewBag.ErrorMessage = "To Leaf No. can not be empty.";
                }
                else if (!this.IsNumberString(leafTo))
                {
                    ViewBag.ErrorMessage = "To Leaf No Number can't contain Special/Normal characters.";
                }
                else if (Math.Abs((int.Parse(leafTo) - int.Parse(leafFrom))) >= leafLimit)
                {
                    ViewBag.ErrorMessage = "Leaf range can not be more than " + leafLimit + ".";
                }
                else
                {
                    try
                    {
                        chqCustomer = await _unitOfWork.CrvCardChqRepo.GetChqCustInfoList(accountNo);

                        if(chqCustomer!= null)
                        {
                            chqCustomer.ChqStatusList = await _unitOfWork.CrvCardChqRepo.GetChqstatusByRange(accountNo, leafFrom, leafTo, userName);
                        }
                    }
                    catch (Exception ex) 
                    {
                        ViewBag.ErrorMessage = ex.Message; 
                    }
                }

                if(chqCustomer == null)
                {
                    chqCustomer = new ChqCustomer();
                    chqCustomer.ChqStatusList = new List<ChqStatus>();
                }
                return PartialView("_CardCustomerInfo", chqCustomer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> ChangeCheckStatus(ChqStopRange chqStopRange)
        {
            string message = "";
            int leafLimit = await GetLeafLimitValue();

            bool saved = false;

            if (string.IsNullOrEmpty(chqStopRange.AccountNo.Trim()))
            {
                message = "Account No. can not be empty." ;
            }
            else if (!AccountNumberValidationHelper.IsAccountNoValid(chqStopRange.AccountNo.Trim()))
            {
                message="Account Number must be 13 or 16 digits & Can't contain Special/Normal characters.";
            }
            else if (string.IsNullOrEmpty(chqStopRange.ChqNo.Trim()))
            {
                message = "From Leaf No. can not be empty.";
                
            }
            else if (!this.IsNumberString(chqStopRange.ChqNo))
            {
                message = "From Leaf No Can't contain Special/Normal characters.";
            }
            else if (string.IsNullOrEmpty(chqStopRange.EndchqNo))
            {
                message = "To Leaf No. can not be empty.";
            }
            else if (!this.IsNumberString(chqStopRange.EndchqNo))
            {
                message = "To Leaf No Number can't contain Special/Normal characters.";
                
            }
            else if ((int.Parse(chqStopRange.EndchqNo.Trim()) - int.Parse(chqStopRange.ChqNo.Trim())) >= leafLimit)
            {
                message = "Leaf range can not be more than " + leafLimit + ".";
                
            }
            else if (string.IsNullOrEmpty(chqStopRange.Rerarks))
            {
                message = "Remarks can not be empty.";
            }
            else
            {
                //chqStopRange.Struserid = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                chqStopRange.Struserid = HttpContext.User.Identity.Name;
                message = await _unitOfWork.CrvCardChqRepo.ChangeChqstatusByRange(chqStopRange);
                saved = true;
            }


            return Json(new { status = saved, message = message });
        }


        public async Task<ActionResult> ShowChqLog(string accountNo)
        {
            try
            {
                accountNo = HttpUtility.HtmlEncode(accountNo);
                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                var data = await _unitOfWork.CrvCardChqRepo.GetChqLog(accountNo, userName);
                return PartialView("_ChqLog", data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Filters.AuthorizeActionFilter]
        public IActionResult Verify()
        {
            return View();
        }

        public async Task<IActionResult> ShowChqReqLog(ChqReqLogRequest chqReqLogRequest)
        {
            string message = "";

            IList<ChqReqLog> list = new List<ChqReqLog>();

            CultureInfo enUS = new CultureInfo("en-US");
            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();


            if ( !string.IsNullOrEmpty(chqReqLogRequest.accountNo) && !string.IsNullOrEmpty(chqReqLogRequest.submited) )
            {
                if( !AccountNumberValidationHelper.IsAccountNoValid(chqReqLogRequest.accountNo.Trim()))
                {
                    message = "Account Number must be 13 or 16 digits & Can't contain Special/Normal characters.";
                }
            }
            if (string.IsNullOrEmpty(chqReqLogRequest.frmDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                message = "From date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(chqReqLogRequest.frmDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                if (!DateTime.TryParseExact(chqReqLogRequest.frmDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out fDate))
                {
                    message = "From date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            if (string.IsNullOrEmpty(chqReqLogRequest.toDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                message = "To date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(chqReqLogRequest.toDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                if (!DateTime.TryParseExact(chqReqLogRequest.toDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out tDate))
                {
                    message = "To date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }
            
            chqReqLogRequest.checkerUserID = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                           
            chqReqLogRequest.makerUserID = System.Web.HttpUtility.HtmlEncode(string.IsNullOrEmpty(chqReqLogRequest.makerUserID) ? "-99" : chqReqLogRequest.makerUserID.Trim());
            list = await _unitOfWork.CrvCardChqRepo.GetChqReqLog(chqReqLogRequest.accountNo, fDate.ToString("dd-MMM-yyyy"), tDate.ToString("dd-MMM-yyyy"), chqReqLogRequest.makerUserID, chqReqLogRequest.checkerUserID );
            

            ViewBag.ErrorMessage = message;
            return PartialView("_PendingChqList", list);
        }

        public async Task<IActionResult> VerifyChqStatus(IList<ChqStopRangeVerify> chqStopRangeVerifies)
        {
            string message = "";
            bool saved = false;

            if (chqStopRangeVerifies.Count ==0)
            {
                message = "No Chq Selected!!!";
            }
            else
            {
                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                message = await _unitOfWork.CrvCardChqRepo.VerifyCheque(chqStopRangeVerifies, userName);
                saved = true;
            }


            return Json(new { status = saved, message = message });
        }

        [Filters.AuthorizeActionFilter]
        public IActionResult Report()
        {
            return View();
        }

        public async Task<JsonResult> GetMakerUserId()
        {
            return Json(await _unitOfWork.CrvCardChqRepo.GetMakerUserList());
        }

        public async Task<IActionResult> ShowChqStopLogReport(ChqStopReportRequest chqReqLogRequest)
        {
            string message = "";

            IList<ChqStopReport> list = new List<ChqStopReport>();

            CultureInfo enUS = new CultureInfo("en-US");
            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();


            if (!string.IsNullOrEmpty(chqReqLogRequest.accountNo) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                if (!AccountNumberValidationHelper.IsAccountNoValid(chqReqLogRequest.accountNo.Trim()))
                {
                    message = "Account Number must be 13 or 16 digits & Can't contain Special/Normal characters.";
                }
            }
            if (string.IsNullOrEmpty(chqReqLogRequest.frmDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                message = "From date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(chqReqLogRequest.frmDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                if (!DateTime.TryParseExact(chqReqLogRequest.frmDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out fDate))
                {
                    message = "From date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            if (string.IsNullOrEmpty(chqReqLogRequest.toDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                message = "To date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(chqReqLogRequest.toDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                if (!DateTime.TryParseExact(chqReqLogRequest.toDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out tDate))
                {
                    message = "To date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            if(chqReqLogRequest.reportType == "Details")
            {
                list = await _unitOfWork.CrvCardChqRepo.GetChqStopReport(chqReqLogRequest.accountNo, chqReqLogRequest.chqNo, fDate.ToString("dd-MMM-yyyy"), tDate.ToString("dd-MMM-yyyy"), chqReqLogRequest.userId);
                ViewBag.ErrorMessage = message;
                return PartialView("_DetailsReport", list);
            }
            else
            {
                IList<ChqStopSummaryReport> summaryReports = new List<ChqStopSummaryReport>();
                summaryReports = await _unitOfWork.CrvCardChqRepo.GetChqStopSummaryReport(fDate.ToString("dd-MMM-yyyy"), tDate.ToString("dd-MMM-yyyy"));
                ViewBag.ErrorMessage = message;
                return PartialView("_SummaryReport", summaryReports);
            }
        }


        public async Task<IActionResult> GetChqStopLogReportInExcel(ChqStopReportRequest chqReqLogRequest)
        {
            string message = "";

            IList<ChqStopReport> list = new List<ChqStopReport>();

            CultureInfo enUS = new CultureInfo("en-US");
            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();


            if (!string.IsNullOrEmpty(chqReqLogRequest.accountNo) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                if (!AccountNumberValidationHelper.IsAccountNoValid(chqReqLogRequest.accountNo.Trim()))
                {
                    message = "Account Number must be 13 or 16 digits & Can't contain Special/Normal characters.";
                }
            }
            if (string.IsNullOrEmpty(chqReqLogRequest.frmDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                message = "From date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(chqReqLogRequest.frmDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                if (!DateTime.TryParseExact(chqReqLogRequest.frmDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out fDate))
                {
                    message = "From date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            if (string.IsNullOrEmpty(chqReqLogRequest.toDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                message = "To date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(chqReqLogRequest.toDate) && !string.IsNullOrEmpty(chqReqLogRequest.submited))
            {
                if (!DateTime.TryParseExact(chqReqLogRequest.toDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out tDate))
                {
                    message = "To date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            string sb = string.Empty;
            if (chqReqLogRequest.reportType == "Details")
            {
                list = await _unitOfWork.CrvCardChqRepo.GetChqStopReport(chqReqLogRequest.accountNo, chqReqLogRequest.chqNo, fDate.ToString("dd-MMM-yyyy"), tDate.ToString("dd-MMM-yyyy"), chqReqLogRequest.userId);
                TempData["ErrorMessage"] = message;
                sb = ReportHelper<ChqStopReport>.ConvertExcel(list);
            }
            else
            {
                IList<ChqStopSummaryReport> summaryReports = new List<ChqStopSummaryReport>();
                summaryReports = await _unitOfWork.CrvCardChqRepo.GetChqStopSummaryReport(fDate.ToString("dd-MMM-yyyy"), tDate.ToString("dd-MMM-yyyy"));
                TempData["ErrorMessage"] = message;
                sb = ReportHelper<ChqStopSummaryReport>.ConvertExcel(summaryReports);
            }

            if (sb.Length == 0)
            {
                TempData["ErrorMessage"] = "Sorry!!! No Data found.!!!";
                return RedirectToAction("Report");
            }
            string fileName = "ChqStopReport_" + DateTime.Now.ToString("MMddyyHHmmss").Trim();
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/vnd.ms-excel", fileName + ".xls");
        }
    }
}
