using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.CommonEnums;
using XCRV.Domain.Entities;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class TaraTuesdayRewardPointsController : BaseController
    {
        private readonly ILogger<TaraTuesdayRewardPointsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public TaraTuesdayRewardPointsController(ILogger<TaraTuesdayRewardPointsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            // var cot = TempData["mobileno"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getTaraTuesdayRewardPoint(string seachType, string seachString, string fdate, string tdate)
        {
            CustomerSearchType type;
            string extensiveType = string.Empty;

            switch (seachType)
            {
                case "DebitCard":
                    type = CustomerSearchType.DebitCard;
                    break;
                case "CreditCard":
                    type = CustomerSearchType.CreditCard;
                    break;
                default:
                    type = CustomerSearchType.DebitCard;
                    // extensiveType = seachType;
                    break;
            }

            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            // IList<TuesdayRewardPoint> data = new List<TuesdayRewardPoint>();
            TuesdayRewardPoint pointDetail = new TuesdayRewardPoint();
            TuesdayRewardPoint contactNo = new TuesdayRewardPoint();
            TuesdayRewardViewModel _tuesdayRewardDetail = new TuesdayRewardViewModel();
            // var data = string.Empty;
            string message = "Sorry!!! No Data Found!!!";
            try
            {

                if (type == CustomerSearchType.CreditCard)
                {
                    if (AccountNumberValidationHelper.IsAccountNoValid(seachString))
                    {
                        bool isAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(seachString, userName);
                        if (!isAccessable)
                        {
                            message = "Sorry!!! You are not authorized to view this information!!!";
                        }
                        else if (AccountNumberValidationHelper.IsAccountNoValid(seachString))
                        {
                            //data = await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(type, seachString, isStatementTrue, extensiveType);
                        }
                        else
                        {
                            message = "Sorry!!! No Data Found!!!";
                        }
                    }
                    else
                    {
                        message = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                    }

                    _tuesdayRewardDetail.tuesdayDetail = await _unitOfWork.RewardPointRepo.GetRewardPointByCreditCard(seachString, fdate,tdate);
                    if (_tuesdayRewardDetail.tuesdayDetail == null)
                    {
                        _tuesdayRewardDetail.tuesdayDetail = new TuesdayRewardPoint();
                        //return Content(TempData["ErrorMessage"].ToString());
                    }
                    _tuesdayRewardDetail.contactDetailDebit = (await _unitOfWork.RewardPointRepo.GetCardValidate(seachString));
                    if (_tuesdayRewardDetail.contactDetailDebit == null)
                    {
                        _tuesdayRewardDetail.contactDetailDebit = new TuesdayRewardContact();
                    }
                }
                else
                {

                    _tuesdayRewardDetail.tuesdayDetail = await _unitOfWork.RewardPointRepo.GetRewardPointByDebitCard(seachString, fdate, tdate);//.CustomerSearchRepo.SearchCustomerBySearchCriteria(type, seachString, isStatementTrue, extensiveType);
                    _tuesdayRewardDetail.contactDetailDebit = (await _unitOfWork.RewardPointRepo.GetCardHolderContactNo(seachString)).FirstOrDefault();
                }
                return Json(new { data = _tuesdayRewardDetail, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> getTaraTuesdayRewardPointOnDate(string seachType, string seachString, string OnDate)
        {
            CustomerSearchType type;
            string extensiveType = string.Empty;

            switch (seachType)
            {
                case "DebitCard":
                    type = CustomerSearchType.DebitCard;
                    break;
                case "CreditCard":
                    type = CustomerSearchType.CreditCard;
                    break;
                default:
                    type = CustomerSearchType.DebitCard;
                    // extensiveType = seachType;
                    break;
            }

            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            // IList<TuesdayRewardPoint> data = new List<TuesdayRewardPoint>();
            TuesdayRewardPoint pointDetail = new TuesdayRewardPoint();
            TuesdayRewardPoint contactNo = new TuesdayRewardPoint();
          TuesdayRewardViewModel _tuesdayRewardDetail =new TuesdayRewardViewModel();
            // var data = string.Empty;
            string message = "Sorry!!! No Data Found!!!";
            try
            {

                if (type == CustomerSearchType.CreditCard)
                {
                    if (AccountNumberValidationHelper.IsAccountNoValid(seachString))
                    {
                        bool isAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(seachString, userName);
                        if (!isAccessable)
                        {
                            message = "Sorry!!! You are not authorized to view this information!!!";
                        }
                        else if (AccountNumberValidationHelper.IsAccountNoValid(seachString))
                        {
                            //data = await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(type, seachString, isStatementTrue, extensiveType);
                        }
                        else
                        {
                            message = "Sorry!!! No Data Found!!!";
                        }
                    }
                    else
                    {
                        message = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                    }
               
                   _tuesdayRewardDetail.tuesdayDetail= await _unitOfWork.RewardPointRepo.GetRewardPointByCreditCardOnDate(seachString, OnDate);
                    if (_tuesdayRewardDetail.tuesdayDetail == null)
                    {
                        _tuesdayRewardDetail.tuesdayDetail = new TuesdayRewardPoint();
                         
                    }
                    _tuesdayRewardDetail.contactDetailDebit = (await _unitOfWork.RewardPointRepo.GetCardValidate(seachString));
                    if (_tuesdayRewardDetail.contactDetailDebit == null)
                    {
                        _tuesdayRewardDetail.contactDetailDebit = new TuesdayRewardContact();
                    }

                }
                else
                {

                    _tuesdayRewardDetail.tuesdayDetail = (await _unitOfWork.RewardPointRepo.GetRewardPointByDebitCardOnDate(seachString, OnDate)).FirstOrDefault();//.CustomerSearchRepo.SearchCustomerBySearchCriteria(type, seachString, isStatementTrue, extensiveType);
                    _tuesdayRewardDetail.contactDetailDebit = (await _unitOfWork.RewardPointRepo.GetCardHolderContactNo(seachString)).FirstOrDefault();

                }
                return Json(new { data = _tuesdayRewardDetail, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> getCardHolderContactNo(string seachString)
        {
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            // IList<TuesdayRewardPoint> data = new List<TuesdayRewardPoint>();
            TuesdayRewardPoint contactNo = new TuesdayRewardPoint();
            string message = "Sorry!!! No Data Found!!!";
            try
            {
                //contactNo = (await _unitOfWork.RewardPointRepo.GetCardHolderContactNo(seachString)).FirstOrDefault();//.CustomerSearchRepo.SearchCustomerBySearchCriteria(type, seachString, isStatementTrue, extensiveType);
                return Json(new { data = contactNo, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<IActionResult> getCardHolderContactNoCredit(string seachString)
        {
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            // IList<TuesdayRewardPoint> data = new List<TuesdayRewardPoint>();
            TuesdayRewardPoint contactNoCredt = new TuesdayRewardPoint();
            string message = "Sorry!!! No Data Found!!!";
            try
            {
               // contactNoCredt = await _unitOfWork.RewardPointRepo.GetCardValidate(seachString);//.CustomerSearchRepo.SearchCustomerBySearchCriteria(type, seachString, isStatementTrue, extensiveType);
                return Json(new { data = contactNoCredt, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}


