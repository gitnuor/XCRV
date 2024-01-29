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
    public class ATMTransactionController : BaseController
    {
        private readonly ILogger<ATMTransactionController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ATMTransactionController(ILogger<ATMTransactionController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> getATMTransactionDetails(string cardno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                IList<ATMTransaction> data = new List<ATMTransaction>();
                string fdate = FromDate.ToString();
                string tdate = ToDate.ToString();
                string msg =string.Empty;
                var claims = User.Claims;
                string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();

                cardno = HttpUtility.HtmlEncode(cardno);

                if (cardno == null)
                {
                    msg = "<font color='red'><b>ATM No can not be empty.</b></font>"; //"ATM No can not be empty.";
                }
                else if (fdate == "1/1/0001 12:00:00 AM")
                {
                    msg = "<font color='red'><b>From Date can not be empty.</b></font>";
                }
                else if (tdate == "1/1/0001 12:00:00 AM")
                {
                    msg = "<font color='red'><b>To Date can not be empty.</b></font>";
                }
                else
                {
                    string schemeCode = await _unitOfWork.OracleBaseRepo.GetSchemCodeByATMCardNumber(cardno.Trim());
                    if (schemeCode == "SRSTF" && isStatementTrue == "N")
                    {
                        msg = "You are not authorized to view this information.";
                    }
                    else
                    {
                        data = await _unitOfWork.TransactionDetailsRepo.GetATMTransactionDetails(cardno, FromDate, ToDate);
                        if (data.Count==0) {
                            msg = "<font color='red'><b>No Data Found!</b></font>"; 
                        }
                    }
                }              
                return Json(new { data = data, status = "success",message=msg, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }
       
    }
}

