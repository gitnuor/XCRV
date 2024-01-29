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
using XCRV.Web.Helpers;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class DebitCardController : BaseController
    {
        private readonly ILogger<DebitCardController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        private string[] cardBinList;

        public DebitCardController(ILogger<DebitCardController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            cardBinList = _configuration.GetSection("AppSettings").GetSection("CARD_BIN_LIST").Value.Split(',');
        }

        [Filters.AuthorizeActionFilter]
        public IActionResult Details()
        {
            return View();
        }

        public async Task<IActionResult> ShowDebitCardDetails(string seachType, string searchString, string maskedCardNo)
        {
            DebitCardInfoViewModel debitCardViewModel = new DebitCardInfoViewModel();

            seachType = HttpUtility.HtmlEncode(seachType);
            searchString = HttpUtility.HtmlEncode(searchString);

            if (string.IsNullOrEmpty(searchString))
            {
                return PartialView("_DebitCardDetails", debitCardViewModel);
            }
            if (string.IsNullOrEmpty(seachType) )
            {
                TempData["ErrorMessage"] = "Select Search Type!!!";
                return PartialView("_DebitCardDetails", debitCardViewModel);
            }

            var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(searchString.Trim(), userName);

            if (seachType.Equals("DebitCardNo"))
            {

                
                debitCardViewModel.DebitCardDetail = (await _unitOfWork.DebitCardRepo.GetDebitCardDetailInfo(searchString.Trim())).FirstOrDefault();

                string schemCode = await _unitOfWork.OracleBaseRepo.GetSchemCodeByATMCardNumber(searchString);

                if(debitCardViewModel.DebitCardDetail != null)
                {
                    if (schemCode.Equals("SRSTF") && IsStatementTrue.Equals("N"))
                    {
                        debitCardViewModel.DebitCardDetail.Daily_Card_Limit = "****";
                        debitCardViewModel.DebitCardDetail.Pos_Purchase_Limit = "****";
                    }
                    else if(!IsAccessable)
                    {
                        debitCardViewModel.DebitCardDetail.Daily_Card_Limit = "****";
                        debitCardViewModel.DebitCardDetail.Pos_Purchase_Limit = "****";
                    }
                    
                    List<DebitCardTransaction> debitCardTransactions = (await _unitOfWork.DebitCardRepo.GetDebitCardTransaction(searchString.Trim(), IsStatementTrue)).ToList();

                    debitCardTransactions.ForEach(p => p.Card_No = new MaskCardNumber().Mask(p.Card_No, cardBinList));
                    debitCardTransactions.ForEach(p => p.Tran_Particular = new MaskCardNumber().Mask(p.Tran_Particular, cardBinList));
                    debitCardViewModel.DebitCardTransactions = debitCardTransactions;

                    return PartialView("_DebitCardDetails", debitCardViewModel);
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! No Data Found.";
                    debitCardViewModel.DebitCardDetail = new DebitCardDetail();
                    return PartialView("_DebitCardDetails", debitCardViewModel);
                }

            }
            else if(seachType.Equals("AccountNo"))
            {
                if (!IsAccessable)
                {
                    TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information.";
                    return PartialView("_AccountDebitCardList", debitCardViewModel);
                }

                IEnumerable<DebitCardDetail> debitCardDetails = await _unitOfWork.DebitCardRepo.GetDebitCardNo(searchString.Trim());

                foreach(var d in debitCardDetails)
                {
                   d.Masked_Card_Number = d.Card_Number.ToString().Length > 10 ? new MaskCardNumber().Mask(d.Card_Number, cardBinList) : d.Card_Number.ToString();
                }

                if(debitCardDetails.Count() ==0)
                {
                    TempData["ErrorMessage"] = "Sorry!!! Data not found.";
                }

                return PartialView("_AccountDebitCardList", debitCardDetails);
            }

            return PartialView("_DebitCardDetails", debitCardViewModel);

        }
    }
}
