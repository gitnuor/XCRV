using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class BalanceDetailsController : BaseController
    {
        private readonly ILogger<BalanceDetailsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public BalanceDetailsController(ILogger<BalanceDetailsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accountNo)
        {
            BalanceDetails _balDetail = null;
            ViewBag.AccountNo = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);

            if (!string.IsNullOrEmpty(accountNo))
            {
                if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo.Trim()))
                {
                    TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                }
                else
                {
                    if (IsNumberString(accountNo))
                    {


                        var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                        string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

                        string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(accountNo.Trim());
                        if ((schemeCode == "SRSTF" && IsStatementTrue == "N")
                            || (!await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo.Trim(), userName)))
                        {
                            TempData["ErrorMessage"] = "You are not authorized to view this information!!!";
                        }
                        else {
                            _balDetail = (await _unitOfWork.TransactionDetailsRepo.GetBalanceDetails(accountNo));
                            
                            if (_balDetail == null)
                            {
                                TempData["ErrorMessage"] = "No data found!";
                            }
                        }
                        
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }

                }

            }
            if (_balDetail == null)
            {
                _balDetail= new BalanceDetails();
            }

           
            return View(_balDetail);
        }
    }
}
