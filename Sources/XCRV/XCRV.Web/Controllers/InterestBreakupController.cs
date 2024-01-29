﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.Web.Controllers
{
    public class InterestBreakupController : BaseController
    {
        private readonly ILogger<InterestBreakupController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public InterestBreakupController(ILogger<InterestBreakupController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

       // [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accno)
        {
            InterestDetails interestBreakup = null;
            ViewBag.AccountNo = accno;
            accno = HttpUtility.HtmlEncode(accno);

            if (!string.IsNullOrEmpty(accno))
            {
                if (!AccountNumberValidationHelper.IsAccountNoValid(accno.Trim()))
                {
                    TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                }
                else
                {
                    if (IsNumberString(accno))
                    {
                        var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                        string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                        string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(accno.Trim());
                        bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accno, userName); //_unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
                        if (!IsAccessable)
                        {
                            TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";
                        }
                        else if (schemeCode == "SRSTF" && IsStatementTrue == "N")
                        {
                            TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";
                        }
                        else
                        {

                            interestBreakup = (await _unitOfWork.TransactionDetailsRepo.GetAccInterestDetails(accno.Trim())).FirstOrDefault();
                            if (interestBreakup == null)
                            {
                                TempData["ErrorMessage"] = "No Data Found.";
                            }

                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }
                }

            }
            if (interestBreakup == null)
            {
                interestBreakup = new InterestDetails();
            }
            
            return View(interestBreakup);
        }
    }
}
