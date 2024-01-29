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

namespace XCRV.Web.Controllers
{
    public class TermDepositSchemeController : BaseController
    {

        private readonly ILogger<TermDepositSchemeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TermDepositSchemeController(ILogger<TermDepositSchemeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accountNo)
        {
            TermDepositScheme termDepositSchem = null;

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


                        if (accVal != 3 )
                        {
                            TempData["ErrorMessage"] = "Sorry!!! This is not a DPS/FDR Account!!!";
                        }
                        else
                        {
                            termDepositSchem = (await _unitOfWork.AccountSchemRepo.GetTermDepositSchemByAcno(accountNo.Trim())).FirstOrDefault();
                            if (termDepositSchem == null || termDepositSchem.Cust_Id == null)
                            {
                                TempData["ErrorMessage"] = "Sorry!!! DPS/FDR Account not found!!!";
                            }
                            else
                            {
                                termDepositSchem.Clr_Bal_Amt = termDepositSchem.Clr_Bal_Amt + " " + termDepositSchem.Acct_Crncy_Code + " / " + termDepositSchem.Sol_Id;
                                termDepositSchem.Acct_Name = termDepositSchem.Acct_Name + " / " + termDepositSchem.Foracid;

                                var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

                                string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(accountNo.Trim());
                                if ((schemeCode == "SRSTF" && IsStatementTrue == "N")
                                    || (!await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo.Trim(), userName)))
                                {
                                    termDepositSchem.Intrate = "****";
                                    termDepositSchem.Un_Clr_Bal_Amt = "****";
                                    termDepositSchem.Cum_Dr_Amt = "****";
                                    termDepositSchem.Cum_Cr_Amt = "****";
                                    termDepositSchem.Acrd_Cr_Amt = "****";
                                    termDepositSchem.Lien_Amt = "****";
                                    termDepositSchem.Nrml_Accrued_Amount_Cr = "****";
                                    termDepositSchem.Nrml_Accrued_Amount_Dr = "****";
                                    termDepositSchem.Nrml_Booked_Amount_Cr = "****";
                                    termDepositSchem.Nrml_Booked_Amount_Dr = "****";
                                    termDepositSchem.Maturity_Amount = "****";
                                    termDepositSchem.Cumulative_Principal = "****";
                                    termDepositSchem.Cumulative_Int_Credited = "****";
                                    termDepositSchem.Clr_Bal_Amt = "****";
                                    termDepositSchem.Cumulative_Int_Paid = "****";
                                }

                                termDepositSchem.Schm_Code = termDepositSchem.Schm_Code + "/" + termDepositSchem.Schm_Desc;


                            }
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }

                }
                
            }
            if (termDepositSchem == null)
            {
                termDepositSchem = new TermDepositScheme();
            }

            
            return View(termDepositSchem);
        }
    }
}
