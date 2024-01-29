using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using XCRV.Application.Interfaces;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class CardProController : Controller
    {
        private readonly ILogger<CardProController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CardProController(ILogger<CardProController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #region PreviousStatementDetails Page

        public IActionResult PreviousStatementDetails()
        {

            ViewBag.Years = new SelectList(GetYearList(), DateTime.Now.Year.ToString());
            ViewBag.Months = new SelectList(GetMonthList(),"Key","Value", DateTime.Now.Month.ToString());
            return View();
        }

        public IActionResult PreviousStatementDetailsInfo(PreviousStatementDetailsRequest request)
        {
            var previousSummary = _unitOfWork.CardProRepository.GetPreviousStatementSummary(request.CardNo, request.Month, request.Year, request.AccountType);
            var previousTransaction = _unitOfWork.CardProRepository.GetPreviousStatementDetails(request.CardNo, request.Month, request.AccountType);

            return View();
        }

        #endregion

        #region CreditHistorySummary

        public IActionResult CreditHistorySummary()
        {
            return View();
        }

        public IActionResult GetCreditHistorySummary(CreditHistoryRequest request)
        {
            var transaction = _unitOfWork.CardProRepository.GetCreditHistorySummary(request.CardNo, request.AccountType);

            return View();
        }

        #endregion




        #region Private Methods

        private IList<string> GetYearList()
        {
            int year = DateTime.Now.Year - 20;

            IList<string> years = new List<string>();

            for (int Y = year; Y <= DateTime.Now.Year; Y++)
            {
                years.Add(Y.ToString());
            }
            return years;
        }

        public Dictionary<int, string> GetMonthList()
        {
            System.Globalization.DateTimeFormatInfo info = System.Globalization.DateTimeFormatInfo.GetInstance(null);

            Dictionary<int, string> months = new Dictionary<int, string>();

            for (int i = 1; i < 13; i++)
            {
                months.Add(i, info.GetMonthName(i));                
            }

            return months;
        }

        #endregion
    }
}
