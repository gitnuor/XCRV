using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.Web.Controllers
{
    public class CreditCardController : BaseController
    {
        private readonly ILogger<CreditCardController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CreditCardController(ILogger<CreditCardController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        [NonAction]
        private string GetFormattedDate(string pstrDate)
        {
            DateTime dtimeTemp;
            System.Globalization.CultureInfo enUS = new System.Globalization.CultureInfo("en-US");
            if (DateTime.TryParseExact(pstrDate, "yyyyMMdd", enUS, System.Globalization.DateTimeStyles.None, out dtimeTemp))
            {
                return dtimeTemp.ToString("dd MMM yyyy");
            }
            else
            {
                if (!string.IsNullOrEmpty(pstrDate))
                    return pstrDate + " (YYYYMMDD)";
                else
                    return "";
            }
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> CardCustomerDetails(string customerId, string cardNo, string mobileNo, string type)
        {
            IList<CreditCardInfo> creditCardInfos = new List<CreditCardInfo>();


            ViewBag.CustomerId = customerId;
            ViewBag.CardNo = cardNo;
            ViewBag.MobileNo = mobileNo;

            customerId = HttpUtility.HtmlEncode(customerId);
            cardNo = HttpUtility.HtmlEncode(cardNo);
            mobileNo = HttpUtility.HtmlEncode(mobileNo);
            type = HttpUtility.HtmlEncode(type);

            if (string.IsNullOrEmpty(type))
            {
                return View(creditCardInfos);
            }

            if (string.IsNullOrEmpty(customerId) && string.IsNullOrEmpty(cardNo) && string.IsNullOrEmpty(mobileNo) && !string.IsNullOrEmpty(type) )
            {
                TempData["ErrorMessage"] = "Customer ID/Card No/Mobile No can not be empty.";
                return View(creditCardInfos);                
            }


            if (string.IsNullOrEmpty(customerId))
            {
                customerId = "-99";
            }
            if (string.IsNullOrEmpty(cardNo))
            {
                cardNo = "-99";
            }
            if (string.IsNullOrEmpty(mobileNo))
            {
                mobileNo = "-99";
            }

            creditCardInfos = await _unitOfWork.CreditCardRepo.GetCardCustInfoList(customerId.Trim(), cardNo.Trim(), mobileNo.Trim());

            if(creditCardInfos.Count ==0 && !string.IsNullOrEmpty(type))
            {
                TempData["ErrorMessage"] = "No Data Found!!!";
            }

            return View(creditCardInfos);
        }

        

        public async Task<IActionResult> CardCustomerDetailsInfo(string customerId, string cardNo)
        {
            CreditCardInfo creditCardInfo = new CreditCardInfo();

            if (string.IsNullOrEmpty(customerId) && string.IsNullOrEmpty(cardNo))
            {
                TempData["ErrorMessage"] = "Customer ID/Card No can not be empty.";

                return Content("<h3>Sorry!!! Customer ID/Card No can not be empty!!!</h3>");

            }

            ViewBag.CustomerId = customerId;
            ViewBag.CardNo = cardNo;

            customerId = HttpUtility.HtmlEncode(customerId);
            cardNo = HttpUtility.HtmlEncode(cardNo);


            if (string.IsNullOrEmpty(customerId))
            {
                customerId = "-99";
            }
            if (string.IsNullOrEmpty(cardNo))
            {
                cardNo = "-99";
            }

            creditCardInfo = (await _unitOfWork.CreditCardRepo.GetCardDetailsInfoList(customerId.Trim(), cardNo.Trim())).FirstOrDefault();

            if (creditCardInfo == null)
            {
                creditCardInfo = new CreditCardInfo();
            }

            return PartialView("_CardCustomerDetailsInfo", creditCardInfo);
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> CardRelatedInformation(string customerId, string cardNo, string mobileNo, string type)
        {
            IList<CreditCardInfo> creditCardInfos = new List<CreditCardInfo>();


            if (string.IsNullOrEmpty(type))
            {
                return View(creditCardInfos);
            }

            if (string.IsNullOrEmpty(customerId) && string.IsNullOrEmpty(cardNo) && string.IsNullOrEmpty(mobileNo) && !string.IsNullOrEmpty(type))
            {
                TempData["ErrorMessage"] = "Customer ID/Card No/Mobile No can not be empty.";
                return View(creditCardInfos);
            }

            ViewBag.CustomerId = customerId;
            ViewBag.CardNo = cardNo;
            ViewBag.MobileNo = mobileNo;

            customerId = HttpUtility.HtmlEncode(customerId);
            cardNo = HttpUtility.HtmlEncode(cardNo);
            mobileNo = HttpUtility.HtmlEncode(mobileNo);
            type = HttpUtility.HtmlEncode(type);

            if (string.IsNullOrEmpty(customerId))
            {
                customerId = "-99";
            }
            if (string.IsNullOrEmpty(cardNo))
            {
                cardNo = "-99";
            }
            if (string.IsNullOrEmpty(mobileNo))
            {
                mobileNo = "-99";
            }

            creditCardInfos = await _unitOfWork.CreditCardRepo.GetCardCustInfoList(customerId.Trim(), cardNo.Trim(), mobileNo.Trim());

            if (creditCardInfos.Count == 0 && !string.IsNullOrEmpty(type))
            {
                TempData["ErrorMessage"] = "No Data Found!!!";
            }

            return View(creditCardInfos);
        }

        public async Task<IActionResult> CardInfoDetails(string customerId, string cardNo)
        {
            CreditCardInfo creditCardInfo = new CreditCardInfo();

            if (string.IsNullOrEmpty(customerId) && string.IsNullOrEmpty(cardNo))
            {
                TempData["ErrorMessage"] = "Customer ID/Card No can not be empty.";

                return Content("<h3>Sorry!!! Customer ID/Card No can not be empty!!!</h3>");

            }

            ViewBag.CustomerId = customerId;
            ViewBag.CardNo = cardNo;

            customerId = HttpUtility.HtmlEncode(customerId);
            cardNo = HttpUtility.HtmlEncode(cardNo);
           

            if (string.IsNullOrEmpty(customerId))
            {
                customerId = "-99";
            }
            if (string.IsNullOrEmpty(cardNo))
            {
                cardNo = "-99";
            }

            creditCardInfo = (await _unitOfWork.CreditCardRepo.GetCardDetailsInfoList(customerId.Trim(), cardNo.Trim())).FirstOrDefault();

            if (creditCardInfo == null)
            {
                creditCardInfo = new CreditCardInfo();
            }
            else
            {
                creditCardInfo.First_Use_Date = GetFormattedDate(creditCardInfo.First_Use_Date);
                creditCardInfo.Production_Date = GetFormattedDate(creditCardInfo.Production_Date);
                creditCardInfo.Activation_Date = GetFormattedDate(creditCardInfo.Activation_Date);
            }

            return PartialView("_CardDetailsInfo", creditCardInfo);
        }
    }
}
