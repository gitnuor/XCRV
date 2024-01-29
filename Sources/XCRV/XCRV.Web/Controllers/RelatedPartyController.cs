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
    public class RelatedPartyController : BaseController
    {
        private readonly ILogger<RelatedPartyController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RelatedPartyController(ILogger<RelatedPartyController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accno)
        {
            // InterestDetails interestBreakup = null;
            RelatedPartyViewModel relaytedParty = new RelatedPartyViewModel();

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

                            relaytedParty.RelatedPartyInfo = (await _unitOfWork.TransactionDetailsRepo.GetAccRelatedParty(accno.Trim())).FirstOrDefault();

                            relaytedParty.RelatedPartyDetails = await _unitOfWork.TransactionDetailsRepo.GetAccRelatedParty(accno.Trim());
                            if (relaytedParty.RelatedPartyInfo == null || relaytedParty.RelatedPartyDetails == null)
                            {
                                TempData["ErrorMessage"] = "No Data Found";
                            }
                            else {
                                relaytedParty.AccountNumber = relaytedParty.RelatedPartyInfo.FORACID;
                                relaytedParty.RelatedPartyInfo.FORACID = relaytedParty.RelatedPartyInfo.FORACID + " " + relaytedParty.RelatedPartyInfo.acct_crncy_code + " / " + relaytedParty.RelatedPartyInfo.sol_id;
                            }
                            

                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }
                }

            }
            //else { 
            //  return NotFound();
            //}
            if (relaytedParty.RelatedPartyDetails== null)
            {
                relaytedParty.RelatedPartyDetails = new List<InterestDetails>();
            }
            if (relaytedParty.RelatedPartyInfo == null)
            {
                relaytedParty.RelatedPartyInfo = new InterestDetails();
            }
           
            return View(relaytedParty);
        }
    }
}
