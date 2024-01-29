using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class SmsBankingController : BaseController
    {
        private readonly ILogger<SmsBankingController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SmsBankingController(ILogger<SmsBankingController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [NonAction]
        private string FullyQualifiedMob(string mobileNo)
        {
            if (mobileNo.IndexOf("+88") >= 0)
            {
                return mobileNo;
            }
            else
            {
                return "+88" + mobileNo;
            }
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Details()
        {
            return View();
        }

        public async Task<IActionResult> ShowPushPullDetails(string mobileNo)
        {
            SmsBankingDetailsViewModel viewModel = new SmsBankingDetailsViewModel();

            ViewBag.MobileNo = mobileNo;

            mobileNo = HttpUtility.HtmlEncode(mobileNo);

            if (string.IsNullOrEmpty(mobileNo))
            {
                return PartialView("_PushPullInfo", viewModel);
            }

            viewModel.MobileVsAccounts = (await _unitOfWork.MobileVsAccountRepo.GetMobileVsAccount(string.Empty, mobileNo)).ToList();

            if (viewModel.MobileVsAccounts.Count > 0)
            {
                string isStaff = "N";
                if (viewModel.MobileVsAccounts.Where(p => p.Schm_Code.Equals("SRSTF")).Count() > 0)
                {
                    isStaff = "Y";
                }
                mobileNo = FullyQualifiedMob(mobileNo);
                viewModel.SmsPulls = (await _unitOfWork.SmsRepo.GetTopTenPullSMS(mobileNo, isStaff)).ToList();
                viewModel.SmsPushes = (await _unitOfWork.SmsRepo.GetTopTenPushSMS(mobileNo, isStaff)).ToList();
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry!!! No Data Found";
            }

            return PartialView("_PushPullInfo",viewModel);
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Log()
        {
            return View();
        }

        public async Task<IActionResult> ShowSmsBankingLog(string accountNo, string mobileNo)
        {
            SmsBankingLogViewModel viewModel = new SmsBankingLogViewModel();

            ViewBag.MobileNo = mobileNo;
            ViewBag.accountNo = accountNo;

            mobileNo = HttpUtility.HtmlEncode(mobileNo);
            accountNo = HttpUtility.HtmlEncode(accountNo);

            if (string.IsNullOrEmpty(mobileNo))
            {
                return PartialView("_SmsLog", viewModel);
            }


            if (string.IsNullOrEmpty(accountNo) && string.IsNullOrEmpty(mobileNo))
            {
                TempData["ErrorMessage"] = "Sorry!!! Both A/c no and mobile no can not be empty.";
                return PartialView("_SmsLog", viewModel);
            }
            else if (!string.IsNullOrEmpty(mobileNo) && mobileNo.Trim().Length < 6)
            {
                TempData["ErrorMessage"] = "Sorry!!! Provided mobile no is not valid.";
                return PartialView("_SmsLog", viewModel);
            }

            string schemaCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(accountNo.Trim());
            var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            if (schemaCode.Equals("SRSTF") && IsStatementTrue.Equals("N"))
            {
                TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information.";
                return PartialView("_SmsLog", viewModel);
            }
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information.";
                return PartialView("_SmsLog", viewModel);
            }

            string queryType="3";
            if (string.IsNullOrEmpty(accountNo))
            {
                queryType = "1";
            }
            else if(string.IsNullOrEmpty(mobileNo))
            {
                queryType = "2";    
            }

            viewModel.AcMobiles = await _unitOfWork.AccountSchemRepo.GetAcMobile(accountNo, mobileNo, queryType);

            IEnumerable<MobileVsAccount> mobileVsAccounts  = (await _unitOfWork.MobileVsAccountRepo.GetMobileVsAccount(string.Empty, mobileNo)).ToList();
            if (mobileVsAccounts.Count() > 0)
            {
                string isStaff = "N";
                if (mobileVsAccounts.Where(p => p.Schm_Code.Equals("SRSTF")).Count() > 0)
                {
                    isStaff = "Y";
                }
                mobileNo = FullyQualifiedMob(mobileNo);
                viewModel.SmsLog = await _unitOfWork.SmsRepo.GetSMSLog(mobileNo, isStaff); 
            }
            else
            {
                TempData["ErrorMessage"] = "Sorry!!! No Data Found";
            }

            return PartialView("_SmsLog", viewModel);
        }
    }
}
