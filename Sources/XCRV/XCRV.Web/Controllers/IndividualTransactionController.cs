using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class IndividualTransactionController : BaseController
    {
        private readonly ILogger<IndividualTransactionController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public IndividualTransactionController(ILogger<IndividualTransactionController> logger, IUnitOfWork unitOfWork,IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration= configuration;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accountNo, string tranID, DateTime date, string flag)
        {           
            IndividualTransaction _indTran = new IndividualTransaction();
            string tdate = date.ToString();

            ViewBag.AccountNo = accountNo;
            ViewBag.TranID = tranID;

            accountNo = HttpUtility.HtmlEncode(accountNo);
            tranID = HttpUtility.HtmlEncode(tranID);
            flag = HttpUtility.HtmlEncode(flag);

            if (!string.IsNullOrEmpty(flag))
            {
                if (accountNo == null)
                {
                    TempData["ErrorMessage"] = "Account NO can not be empty.";
                }
                else if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo.Trim()))
                {
                    TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                }
                else if (tranID == null)
                {
                    TempData["ErrorMessage"] = "Transaction ID can not be empty.";
                }
                else if (tdate == "1/1/0001 12:00:00 AM")
                {
                    TempData["ErrorMessage"] = "Transaction date can not be empty.";
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
                        else
                        {
                            _indTran = (await _unitOfWork.TransactionDetailsRepo.GetIndividualTranDetails(accountNo.Trim(), tranID, date));
                            if (_indTran == null)
                            {
                                TempData["ErrorMessage"] = "No data found!";
                            }
                            else
                            {
                                string patCollection = _configuration.GetSection("AppSettings").GetSection("CARD_BIN_LIST").Value;//Configuration.GetValue<string>("MySettings:DbConnection");
                                var splitcollection = patCollection.Split(',');
                                string strRemarks = _indTran.TRAN_RMKS;
                                if (strRemarks != null)
                                {
                                    _indTran.TRAN_RMKS = _indTran.TRAN_RMKS.ToString().Length > 10 ? new Helpers.MaskCardNumber().Mask(_indTran.TRAN_RMKS.ToString(), splitcollection) : _indTran.TRAN_RMKS.ToString();
                                }
                                

                            }
                                  
                        }

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }

                }
            }

            if (_indTran == null)
            {
                _indTran = new IndividualTransaction();
            }
            
            if (tdate == "1/1/0001 12:00:00 AM")
            {
                ViewBag.tranDate = "";
            }
            else
            {
                ViewBag.tranDate = date.ToString("dd-MMM-yyyy");
            }
            return View(_indTran);
        }
    }
}
