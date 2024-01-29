using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class MbsController : BaseController
    {
        private readonly ILogger<BalanceDetailsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public MbsController(ILogger<BalanceDetailsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> AccountBuffer(string accountNo)
        {
            MbsAccountBufferViewModel viewModel = new MbsAccountBufferViewModel();

            if (string.IsNullOrEmpty(accountNo))
            {
                return View(viewModel);
            }

            ViewBag.AccountNo = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);

            string schemCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(accountNo);

            var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();

            if (schemCode == "SRSTF" && IsStatementTrue == "N")
            {
                TempData["ErrorMessage"] = "You are not authorized to view this information";
                return View(viewModel);
            }

            viewModel.MbsBufferCharges = await _unitOfWork.MbsRepo.GetBufferChargeAmount(accountNo);
            viewModel.MbsBufferInterests = await _unitOfWork.MbsRepo.GetBufferInterestRelated(accountNo);

            if (viewModel.MbsBufferCharges.Count() == 0 && viewModel.MbsBufferInterests.Count() == 0)
            {
                TempData["ErrorMessage"] = "No Data Found!!!!";
            }

            return View(viewModel);
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Transaction()
        {
            return View();
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Statement()
        {
            return View();
        }

        public async Task<IActionResult> ShowMbsAccountDetails(string accountNo)
        {
            MbsAccountInfo AccountInfo = new MbsAccountInfo();
            ViewBag.ACNO = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);

            try
            {

                if (string.IsNullOrEmpty(accountNo))
                {
                    TempData["ErrorMessage"] = "Account No can not be empty.";
                    return PartialView("_ShowMbsAccountDetails", AccountInfo);
                }

                if (accountNo.Trim().Substring(0, 4) == "0000")
                {
                    AccountInfo = (await _unitOfWork.MbsRepo.GetMbsHoStatementAccountName(accountNo)).FirstOrDefault();
                    if (AccountInfo != null)
                    {
                        AccountInfo.csaAccountName = AccountInfo.coaAccName;
                    }
                }
                else
                {
                    AccountInfo = (await _unitOfWork.MbsRepo.GetMbsStatementAccountName(accountNo)).FirstOrDefault();

                }


                if (AccountInfo == null)
                {
                    AccountInfo = new MbsAccountInfo();
                    ViewBag.ACNO = string.Empty;
                    TempData["ErrorMessage"] = "Account Not Found";
                }
            }
            catch(Exception ex)
            {
                AccountInfo = new MbsAccountInfo();
                ViewBag.ACNO = string.Empty;
                TempData["ErrorMessage"] = ex.Message;
            }

            return PartialView("_ShowMbsAccountDetails", AccountInfo);

        }

        [HttpGet]
        public async Task<IActionResult> GetMbsTransaction(string accountNo, string fromDate, string toDate)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();


            accountNo = HttpUtility.HtmlEncode(accountNo);
            fromDate = HttpUtility.HtmlEncode(fromDate);
            toDate = HttpUtility.HtmlEncode(toDate);

            string message = "Sorry!!! No Data Found!!!";

            if (string.IsNullOrEmpty(accountNo))
            {
                message = "Account No can not be empty.";
            }

            else
            {
                if (string.IsNullOrEmpty(fromDate))
                {
                    message = "From date can not be empty.";
                }

                else if (!string.IsNullOrEmpty(fromDate))
                {
                    if (!DateTime.TryParseExact(fromDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out fDate))
                    {
                        message = "From date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                    }
                }

                if (string.IsNullOrEmpty(toDate))
                {
                    message = "To date can not be empty.";
                }
                else if (!string.IsNullOrEmpty(toDate))
                {
                    if (!DateTime.TryParseExact(toDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out tDate))
                    {
                        message = "To date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                    }
                }
            }
            IList<MbsTransaction> transactions = new List<MbsTransaction>();

            try
            {

                if (fDate > new DateTime(2000, 1, 1) && tDate > new DateTime(2000, 1, 1))
                {
                    if (!string.IsNullOrEmpty(accountNo))
                    {
                        if (accountNo.Trim().Substring(0, 4) == "0000")
                        {
                            transactions = (await _unitOfWork.MbsRepo.GetMbsHoTransaction(accountNo, fDate, tDate)).ToList();
                        }
                        else
                        {
                            transactions = (await _unitOfWork.MbsRepo.GetMbsTransaction(accountNo, fDate, tDate)).ToList();
                        }

                        DateTime dt = new DateTime();
                        foreach (var d in transactions)
                        {
                            if (!string.IsNullOrEmpty(d.atotransdate))
                            {
                                if (DateTime.TryParse(d.atotransdate, out dt))
                                {
                                    d.atotransdate = dt.ToString("dd-MMM-yyyy");
                                }
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                message = "No Data Found!!! (Error 500)";
            }
            

            return Json(new { data = transactions, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }

        public async Task<IActionResult> ShowMbsTransactionPrintable(string accountNo, string fromDate, string toDate)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();

            ViewBag.AccountNo = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);
            fromDate = HttpUtility.HtmlEncode(fromDate);
            toDate = HttpUtility.HtmlEncode(toDate);

            if (string.IsNullOrEmpty(accountNo))
            {
                return Content("<h4>Account No No can not be empty.</h4>");
            }

            else
            {
                if (string.IsNullOrEmpty(fromDate))
                {
                    return Content("<h4>From date can not be empty.</h4>");
                }

                else if (!string.IsNullOrEmpty(fromDate))
                {
                    if (!DateTime.TryParseExact(fromDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out fDate))
                    {
                        return Content("<h4>From date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)</h4>");
                    }
                }

                if (string.IsNullOrEmpty(toDate))
                {
                    return Content("<h4>From date can not be empty.</h4>");

                }
                else if (!string.IsNullOrEmpty(toDate))
                {
                    if (!DateTime.TryParseExact(toDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out tDate))
                    {
                        return Content("<h4>To date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)</h4>");
                    }
                }
            }
            IList<MbsTransaction> transactions = new List<MbsTransaction>();
            MbsAccountInfo MbsAccountInfo = new MbsAccountInfo();

            if (!string.IsNullOrEmpty(accountNo))
            {
                if (accountNo.Trim().Substring(0, 4) == "0000")
                {
                    var mbsTransactionsCall = _unitOfWork.MbsRepo.GetMbsHoTransaction(accountNo, fDate, tDate);
                    var mbsAccountInfoCall = _unitOfWork.MbsRepo.GetMbsHoStatementAccountName(accountNo);

                    MbsAccountInfo = (await mbsAccountInfoCall).FirstOrDefault();
                    transactions = (await mbsTransactionsCall).ToList();

                    if (MbsAccountInfo != null)
                    {
                        MbsAccountInfo.csaAccountName = MbsAccountInfo.coaAccName;
                    }
                }
                else
                {
                    var mbsTransactionsCall = _unitOfWork.MbsRepo.GetMbsTransaction(accountNo, fDate, tDate);
                    var mbsAccountInfoCall = _unitOfWork.MbsRepo.GetMbsStatementAccountName(accountNo);

                    MbsAccountInfo = (await mbsAccountInfoCall).FirstOrDefault();
                    transactions = (await mbsTransactionsCall).ToList();
                }

                DateTime dt = new DateTime();
                foreach (var d in transactions)
                {
                    if (!string.IsNullOrEmpty(d.atotransdate))
                    {
                        if (DateTime.TryParse(d.atotransdate, out dt))
                        {
                            d.atotransdate = dt.ToString("dd MMM yy");
                        }
                    }
                }
            }

            MbsTransactionViewModel viewModel = new MbsTransactionViewModel();
            viewModel.AccountInfo = MbsAccountInfo;
            viewModel.MbsTransactions = transactions;


            ViewBag.ReportName = "TRANSACTION OF ACCOUNT FOR THE PERIOD OF " + fromDate + " To " + toDate;

            return PartialView("_PrintableMbsTransaction", viewModel);

        }

        public async Task<IActionResult> ShowMbsStatementPrintable(string accountNo, string fromDate, string toDate)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();

            ViewBag.AccountNo = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);
            fromDate = HttpUtility.HtmlEncode(fromDate);
            toDate = HttpUtility.HtmlEncode(toDate);


            if (string.IsNullOrEmpty(accountNo))
            {
                return Content("<h4>Account No No can not be empty.</h4>");
            }

            else
            {
                if (string.IsNullOrEmpty(fromDate))
                {
                    return Content("<h4>From date can not be empty.</h4>");
                }

                else if (!string.IsNullOrEmpty(fromDate))
                {
                    if (!DateTime.TryParseExact(fromDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out fDate))
                    {
                        return Content("<h4>From date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)</h4>");
                    }
                }

                if (string.IsNullOrEmpty(toDate))
                {
                    return Content("<h4>From date can not be empty.</h4>");

                }
                else if (!string.IsNullOrEmpty(toDate))
                {
                    if (!DateTime.TryParseExact(toDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out tDate))
                    {
                        return Content("<h4>To date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)</h4>");
                    }
                }
            }
            IList<MbsTransaction> transactions = new List<MbsTransaction>();
            MbsAccountInfo MbsAccountInfo = new MbsAccountInfo();

            if (!string.IsNullOrEmpty(accountNo))
            {
                if (accountNo.Trim().Substring(0, 4) == "0000")
                {
                    var mbsTransactionsCall = _unitOfWork.MbsRepo.GetMbsHoTransaction(accountNo, fDate, tDate);
                    var mbsAccountInfoCall = _unitOfWork.MbsRepo.GetMbsHoStatementAccountName(accountNo);

                    MbsAccountInfo = (await mbsAccountInfoCall).FirstOrDefault();
                    transactions = (await mbsTransactionsCall).ToList();

                    if (MbsAccountInfo != null)
                    {
                        MbsAccountInfo.csaAccountName = MbsAccountInfo.coaAccName;
                    }
                }
                else
                {
                    var mbsTransactionsCall = _unitOfWork.MbsRepo.GetMbsTransaction(accountNo, fDate, tDate);
                    var mbsAccountInfoCall = _unitOfWork.MbsRepo.GetMbsStatementAccountName(accountNo);

                    MbsAccountInfo = (await mbsAccountInfoCall).FirstOrDefault();
                    transactions = (await mbsTransactionsCall).ToList();
                }

                DateTime dt = new DateTime();
                foreach (var d in transactions)
                {
                    if (!string.IsNullOrEmpty(d.atotransdate))
                    {
                        if (DateTime.TryParse(d.atotransdate, out dt))
                        {
                            d.atotransdate = dt.ToString("dd MMM yy");
                        }
                    }
                }
            }

            MbsTransactionViewModel viewModel = new MbsTransactionViewModel();
            viewModel.AccountInfo = MbsAccountInfo;
            viewModel.MbsTransactions = transactions;

            ViewBag.ReportName = "STATEMENT OF ACCOUNT FOR THE PERIOD OF " + fromDate + " To " + toDate;

            return PartialView("_PrintableMbsStatement", viewModel);

        }


        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> FinacleMbsAccNo(string accountNo)
        {
            FinacleMbsMapping viewModel = new FinacleMbsMapping();

            if (string.IsNullOrEmpty(accountNo))
            {
                return View(viewModel);
            }

            ViewBag.AccountNo = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);

            string schemCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(accountNo);
            var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
            if (schemCode == "SRSTF" && IsStatementTrue == "N")
            {
                TempData["ErrorMessage"] = "You are not authorized to view this information";
                return View(viewModel);
            }

            viewModel = await _unitOfWork.MbsRepo.GetMbsAccountMapping(accountNo);


            if (viewModel == null)
            {
                viewModel = new FinacleMbsMapping();
                TempData["ErrorMessage"] = "No Data Found!!!!";
            }

            return View(viewModel);
        }



    }
}
