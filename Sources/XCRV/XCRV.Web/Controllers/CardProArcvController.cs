using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class CardProArcvController : BaseController
    {
        private readonly ILogger<CardProArcvController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CardProArcvController(ILogger<CardProArcvController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult CardHolderMemo()
        {
            return View();
        }

        public IActionResult TransactionSummary()
        {
            return View();
        }

        public async Task<IActionResult> getTransactionDetails(string cardno,DateTime FromDate, DateTime ToDate)
        {
            var data =await _unitOfWork.CardProRepository.GetTransactionSummary(cardno, FromDate,ToDate);
            //TransactionSummary data = new TransactionSummary();
            return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
        }
    }
}
