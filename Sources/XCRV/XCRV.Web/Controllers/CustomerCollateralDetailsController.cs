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
    public class CustomerCollateralDetailsController : BaseController
    {
        private readonly ILogger<CustomerCollateralDetailsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerCollateralDetailsController(ILogger<CustomerCollateralDetailsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index()
        {           
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getCustomerLimit(string seachString)
        {
            try
            {
                string msg = string.Empty;
                var claims = User.Claims;
                string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
                IList<CustomerCollateral> data = new List<CustomerCollateral>();
                if (!string.IsNullOrEmpty(seachString))
                {
                    seachString = HttpUtility.HtmlEncode(seachString);
                }
                if (seachString == null)
                {
                    msg = "<font color='red'><b>Customer Id can not be empty.</b></font>"; //"ATM No can not be empty.";
                }
                else 
                {
                    data = await _unitOfWork.CustomerCollateralRepo.getCustomerCollateral(seachString);
                    if (data == null || data.Count==0)
                    {
                        msg = "<font color='red'><b>No Data Found!</b></font>";
     
                    }
                }
                return Json(new { data = data, status = "success", message = msg, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }


        [HttpGet]
        public async Task<IActionResult> getCustomerLimitInfo(string Custid)
        {
            try
            {
                if (!string.IsNullOrEmpty(Custid))
                {
                    Custid = HttpUtility.HtmlEncode(Custid);
                }
                CustomerCollateral data=new CustomerCollateral();
                if (Custid != null)
                {
                    data = await _unitOfWork.CustomerCollateralRepo.GetCustomerCollateralByCustid(Custid.Trim());
                    
                }
                return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });

            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }
    }
}
