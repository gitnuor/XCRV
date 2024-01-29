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
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class FinMiniStatementController : BaseController
    {
        private readonly ILogger<FinMiniStatementController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public FinMiniStatementController(ILogger<FinMiniStatementController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [Filters.AuthorizeActionFilter]
        public async Task< IActionResult> Index(string accno)
        {
            FinMiniStatementViewModel finMinistate = new FinMiniStatementViewModel();

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

                            finMinistate.miniStatementDetails = (await _unitOfWork.TransactionDetailsRepo.GetFinMiniStatementDetails(accno.Trim()));
                            finMinistate.accBal= await _unitOfWork.AccountSchemRepo.GetAccBal(accno);
                            if (finMinistate.miniStatementDetails.Count == 0 || finMinistate.miniStatementDetails == null)
                            {
                                TempData["ErrorMessage"] = "No Data Found";
                            }
                            else {
                                var rewardPointCall = _unitOfWork.RewardPointRepo.CurrentMonthStmtRPBal(accno);
                                try
                                {
                                    finMinistate.rewardPointinfo = await rewardPointCall;

                                    if (!finMinistate.rewardPointinfo.ResponseMessege.Equals("Success"))
                                    {
                                        TempData["ErrorMessage"] = "Reward Point Data :: " + finMinistate.rewardPointinfo.ResponseMessege + ".";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    TempData["ErrorMessage"] = "Reward Point Data :: " + ex.Message + ".";
                                }
                            }
                           
                            // finMinistate.rewardPointinfo= await _unitOfWork.RewardPointRepo.CurrentMonthStmtRPBal(accno);
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }
                }

            }
            if (finMinistate.miniStatementDetails == null)
            {
                finMinistate.miniStatementDetails = new List<FinMiniStatement>();
            }
            if (finMinistate.rewardPointinfo == null)
            {
                finMinistate.rewardPointinfo = new RewardPoint();
            }
            if (finMinistate.accBal== null)
            {
                finMinistate.accBal = new EffectiveBal();
            }
            
            return View(finMinistate);
        }

    }
}
