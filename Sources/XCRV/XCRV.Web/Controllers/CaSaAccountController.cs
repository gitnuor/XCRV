using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Common;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class CaSaAccountController : BaseController
    {
        private readonly ILogger<CaSaAccountController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        
        public CaSaAccountController(ILogger<CaSaAccountController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accountNo)
        {
            ViewBag.Title = "Saving Account/Current Account Info";            

            CaSaViewModel accountInforation = new CaSaViewModel();
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
                    if(IsNumberString(accountNo))
                    {
                        int accVal;
                        if(accountNo.Trim().Length == 16)
                        {
                            accVal = Convert.ToInt32(accountNo.Substring(4, 1));
                        }
                        else
                        {
                            accVal = Convert.ToInt32(accountNo.Substring(0, 1));
                        }

                        if (accVal != 2 && accVal != 1)
                        {
                            TempData["ErrorMessage"] = "Sorry!!! This is not a SBA/CAA Account!!!";
                        }
                        else
                        {
                            accountInforation.CaSaAccount = (await _unitOfWork.AccountSchemRepo.GetCaSaAccountInfoByAcno(accountNo)).FirstOrDefault();

                            if (accountInforation.CaSaAccount == null || accountInforation.CaSaAccount.Cust_Id == null)
                            {
                                TempData["ErrorMessage"] = "Sorry!!! SBA/CAA Account not found!!!";
                            }
                            else
                            {
                                var unclearedChqAmtCrCall = _unitOfWork.UnclearedChqRepo.GetUnclearedChqAmtCr(accountNo);
                                var unclearedChqAmtDrCall = _unitOfWork.UnclearedChqRepo.GetUnclearedChqAmtDr(accountNo);
                                var rewardPointCall = _unitOfWork.RewardPointRepo.CurrentMonthStmtRPBal(accountNo);

                                var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

                                accountInforation.UnclearedChqCr = (await unclearedChqAmtCrCall).FirstOrDefault();
                                accountInforation.UnclearedChqDr = (await unclearedChqAmtDrCall).FirstOrDefault();

                                bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);

                                if (accountInforation.UnclearedChqCr != null)
                                {
                                    if ((accountInforation.UnclearedChqCr.Schm_Code.Equals("SRSTF") && IsStatementTrue.Equals("N")) || !IsAccessable)
                                    {
                                        accountInforation.UnclearedChqCr.Tran_Amt = "****";
                                    }
                                }
                                if (accountInforation.UnclearedChqDr != null)
                                {
                                    if ((accountInforation.UnclearedChqDr.Schm_Code.Equals("SRSTF") && IsStatementTrue.Equals("N")) || !IsAccessable)
                                    {
                                        accountInforation.UnclearedChqDr.System_Reserved_Amt = "****";
                                    }
                                }

                                accountInforation.CaSaAccount.Clr_Bal_Amt = accountInforation.CaSaAccount.Clr_Bal_Amt + " " + accountInforation.CaSaAccount.Acct_Crncy_Code + " / " + accountInforation.CaSaAccount.Sol_Id;

                                accountInforation.CaSaAccount.Acct_Name = accountInforation.CaSaAccount.Acct_Name + " / " + accountInforation.CaSaAccount.Acid;

                                if ((accountInforation.CaSaAccount.Schm_Code.Equals("SRSTF") && IsStatementTrue.Equals("N")) || !IsAccessable)
                                {
                                    accountInforation.CaSaAccount.Clr_Bal_Amt = "****";
                                    accountInforation.CaSaAccount.Intrate = "****";
                                    accountInforation.CaSaAccount.Un_Clr_Bal_Amt = "****";
                                    accountInforation.CaSaAccount.Cum_Dr_Amt = "****";
                                    accountInforation.CaSaAccount.Cum_Cr_Amt = "****";
                                    accountInforation.CaSaAccount.Lien_Amt = "****";
                                    accountInforation.CaSaAccount.Acrd_Cr_Amt = "****";
                                    accountInforation.CaSaAccount.Bal_On_Frez_Date = "****";
                                    accountInforation.CaSaAccount.Nrml_Accrued_Amount_Cr = "****";
                                    accountInforation.CaSaAccount.Nrml_Accrued_Amount_Dr = "****";
                                    accountInforation.CaSaAccount.Nrml_Booked_Amount_Cr = "****";
                                    accountInforation.CaSaAccount.Nrml_Booked_Amount_Dr = "****";
                                    accountInforation.CaSaAccount.Last6MonthsAvgbal = "****";
                                    accountInforation.CaSaAccount.Sanct_Lim = "****";
                                }
                                try
                                {
                                    accountInforation.RewardPoint = await rewardPointCall;

                                    if (!accountInforation.RewardPoint.ResponseMessege.Equals("Success"))
                                    {
                                        TempData["ErrorMessage"] = "Reward Point Data :: " + accountInforation.RewardPoint.ResponseMessege + ".";
                                    }
                                }
                                catch(Exception ex)
                                {
                                    TempData["ErrorMessage"] = "Reward Point Data :: " + ex.Message + ".";
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

            var claims = User.Claims;
            string groupName = claims.FirstOrDefault(p => p.Type.Equals("GroupName")).Value.ToString();
            if (accountInforation.CaSaAccount != null || accountInforation.UnclearedChqCr!=null || accountInforation.UnclearedChqDr != null || accountInforation.RewardPoint != null)
            {
                if (groupName == "Y")
                {
                    string probasiFlag = await _unitOfWork.CustomerSearchRepo.GetProbasiFlagByUser(accountInforation.CaSaAccount.Cust_Id);
                    if (probasiFlag == "Y")
                    {
                        if (accountInforation.CaSaAccount == null)
                        {
                            accountInforation.CaSaAccount = new CaSaAccountInfo();
                        }
                        if (accountInforation.UnclearedChqCr == null)
                        {
                            accountInforation.UnclearedChqCr = new UnclearedChqCr();
                        }
                        if (accountInforation.UnclearedChqDr == null)
                        {
                            accountInforation.UnclearedChqDr = new UnclearedChqDr();
                        }
                        if (accountInforation.RewardPoint == null)
                        {
                            accountInforation.RewardPoint = new RewardPoint();
                        }
                        return View(accountInforation);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "YOU ARE NOT AUTHORIZED TO VIEW DATA!!";
                        if (probasiFlag != "Y")
                        {
                            accountInforation.CaSaAccount = new CaSaAccountInfo();
                        }
                        if (probasiFlag != "Y")
                        {
                            accountInforation.UnclearedChqCr = new UnclearedChqCr();
                        }
                        if (probasiFlag != "Y")
                        {
                            accountInforation.UnclearedChqDr = new UnclearedChqDr();
                        }
                        if (probasiFlag != "Y")
                        {
                            accountInforation.RewardPoint = new RewardPoint();
                        }
                        return View(accountInforation);
                    }
                }
            }

            if (accountInforation.CaSaAccount == null)
            {
                accountInforation.CaSaAccount = new CaSaAccountInfo();
            }
            if (accountInforation.UnclearedChqCr == null)
            {
                accountInforation.UnclearedChqCr = new UnclearedChqCr();
            }
            if (accountInforation.UnclearedChqDr == null)
            {
                accountInforation.UnclearedChqDr = new UnclearedChqDr();
            }
            if (accountInforation.RewardPoint == null)
            {
                accountInforation.RewardPoint = new RewardPoint();
            }

            
            return View(accountInforation);
        }

        public async Task<ActionResult> ShowNominees(string accountNo)
        {
            accountNo = HttpUtility.HtmlEncode(accountNo);

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);

            if(!IsAccessable)
            {
                return Content("<h3>Sorry!!! You are not authorized to view this information!!!</h3>");
            }
            {
                var model = await _unitOfWork.NomineeRepo.GetCaSaNominees(accountNo);
                return PartialView("_Nominee", model);
            }
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> BasicInfo(string accountNo)
        {
            ViewBag.Title = "Account Information";
            ViewBag.AccountNo = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);

            IList<CustomerDetails> customerDetails = new List<CustomerDetails>();
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
                        IEnumerable<AccSignatory> accSignatories = await _unitOfWork.AccountSchemRepo.GetACCSignatoryInfo(accountNo);

                        if (accSignatories.Count() > 0)
                        {
                            ViewBag.AccountName = accSignatories.First().Acct_Name;

                            CustomerDetails customerDetail = null;
                            

                            foreach(var acc in accSignatories)
                            {
                                customerDetail = await _unitOfWork.CustomerDetailsRepo.GetCustomerDetailsByCif(acc.ApplicantsCIF);

                                if (customerDetail != null)
                                {
                                    customerDetail.cust_section_title = acc.IsPrimaryApplicant;
                                    customerDetails.Add(customerDetail);
                                }
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Sorry!!! Account Number Not Found!!!!";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }

                }
            }
            return View(customerDetails);
        }
    }
}
