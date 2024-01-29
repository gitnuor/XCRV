using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Common;
using XCRV.Web.Helpers;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class CustomerGeneralDetailsController : BaseController
    {
        private readonly ILogger<CustomerGeneralDetailsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerGeneralDetailsController(ILogger<CustomerGeneralDetailsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string customerId)
        {
            CustomerDetails customerDetails = null;
            ViewBag.CustomerId = customerId;
            var claims = User.Claims;

            customerId = HttpUtility.HtmlEncode(customerId);

            string groupName = claims.FirstOrDefault(p => p.Type.Equals("GroupName")).Value.ToString();

            if (!string.IsNullOrEmpty(customerId))
            {            
                if(IsNumberString(customerId))
                {
                    customerDetails = await _unitOfWork.CustomerDetailsRepo.GetCustomerDetailsByCif(customerId.Trim());
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (customerDetails != null)
            {
                if (groupName == "Y")
                {
                    string probasiFlag = await _unitOfWork.CustomerSearchRepo.GetProbasiFlagByUser(customerId);
                    if (probasiFlag == "Y")
                    {
                        return View(customerDetails);                     
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "YOU ARE NOT AUTHORIZED TO VIEW DATA!!";
                        return View(new CustomerDetails());
                    }
                }
            }

            if (customerDetails == null)
            {
                customerDetails = new CustomerDetails();
            }

            return View(customerDetails);
        }

        public async Task<IActionResult> GetGuarantorByCif(string cif)
        {
            cif = HttpUtility.HtmlEncode(cif);
            string message = "";
            var guarantors = await _unitOfWork.CustomerDetailsRepo.GetGuarantorByCif(cif);
            return Json(new { data = guarantors, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        public async Task<IActionResult> GetIntroducerByCif(string cif)
        {
            cif = HttpUtility.HtmlEncode(cif);
            string message = "";
            var introducers = await _unitOfWork.CustomerDetailsRepo.GetInitiatorByCif(cif);
            return Json(new { data = introducers, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        public async Task<IActionResult> GetStandingInstructionByCif(string cif)
        {
            cif = HttpUtility.HtmlEncode(cif);

            string message = "";
            var standingInstructions = await _unitOfWork.CustomerDetailsRepo.GetStandingInstructionByCif(cif);
            return Json(new { data = standingInstructions, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        public async Task<IActionResult> GetCustomerMemosByCif(string cif)
        {
            cif = HttpUtility.HtmlEncode(cif);

            string message = "Sorry!!! No Data Found!!!";
            var customerMemos = await _unitOfWork.CustomerMemoRepo.GetCustomerMemosByCif(cif);
            return Json(new { data = customerMemos, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        public async Task<IActionResult> SaveCustomerMemo(CustomerMemoViewModel memo)
        {            
            memo.userId = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                return Json(new { Errors = ModelState.Errors(), status = "error" });
            }
            else
            {
                int count = 0;
                if (memo.id > 0)
                {
                    count = await _unitOfWork.CustomerMemoRepo.UpdateCustomerMemo(memo.id, memo.memoText, memo.userId);
                }
                else
                {
                    count = await _unitOfWork.CustomerMemoRepo.InsertCustomerMemo(memo.memoText, memo.userId, memo.customerId);
                }
                if(count > 0)
                {
                    var customerMemos = await _unitOfWork.CustomerMemoRepo.GetCustomerMemosByCif(memo.customerId);
                    return Json(new { data = customerMemos, status = "success", message = "Customer Memo Saved Sucessfully!!!", result = CommonAjaxResponse("Success", "Success", "200") });
                }
                else
                {
                    return Json(new { status = "failed", message = "Customer Memo Save Failed!!!" });
                }
            }
        }
    }
}
