using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XCRV.Application.Interfaces;
using XCRV.Domain.CommonEnums;
using XCRV.Domain.Entities;
using XCRV.Web.Common;

namespace XCRV.Web.Controllers
{
    public class CustomerSearchController : BaseController
    {
        private readonly ILogger<CustomerSearchController> _logger;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public CustomerSearchController(ILogger<CustomerSearchController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpGet]
        public async Task<IActionResult> SearchCustomerBySearchCriteria(string seachType, string seachString)
        {
            CustomerSearchType type;
            string extensiveType = string.Empty;
            string _pbsFlag = "1";

            seachType = HttpUtility.HtmlEncode(seachType);
            seachString = HttpUtility.HtmlEncode(seachString);

            switch (seachType)
            {
                case "AccountNo":
                    type = CustomerSearchType.SP_SEARCH_CUST_BY_ACNO_HR;
                    break;
                case "DebitCardNo":
                    type = CustomerSearchType.SP_SEARCH_CUST_BY_ATM_HR;
                    break;
                case "CustomerID":
                    type = CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR;
                    break;
                case "AccountName":
                    type = CustomerSearchType.SP_SEARCH_CUST_BY_NAME_HR;
                    break;
                default:
                    type = CustomerSearchType.SP_EXTENSIVE_SEARCH;
                    extensiveType = seachType;
                    break;
            }

            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            IList<CustomerSearch> data = new List<CustomerSearch>();

            string message = "Sorry!!! No Data Found!!!";

            if (type == CustomerSearchType.SP_SEARCH_CUST_BY_ACNO_HR)
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
                        data = await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(type, seachString, isStatementTrue, extensiveType, _pbsFlag);
                        foreach (var m in data)
                        {
                            m.astha_status = (await _unitOfWork.CustomerSearchRepo.getAsthaStatus(m.cust_id, isStatementTrue)).FirstOrDefault().astha_status;
                        }
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

            }
            else
            {
                data = await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(type, seachString, isStatementTrue, extensiveType, _pbsFlag);
                foreach (var d in data)
                {
                    d.astha_status = (await _unitOfWork.CustomerSearchRepo.getAsthaStatus(d.cust_id, isStatementTrue)).FirstOrDefault().astha_status;
                }
            }

            string groupName = claims.FirstOrDefault(p => p.Type.Equals("GroupName")).Value.ToString();
            if (data.Count > 0)
            {
                if (groupName == "Y")
                {
                    string msg = "<font color='red'><b>YOU ARE NOT AUTHORIZED TO VIEW DATA!!</b></font>";
                    string probasiFlag = await _unitOfWork.CustomerSearchRepo.GetProbasiFlagByUser(data[0].cust_id);
                    if (probasiFlag == "Y")
                    {
                        return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
                    }
                    else {
                        return Json(new { data = 0, status = "failed", message = msg, result = CommonAjaxResponse("failed", "failed", "200") });
                    }
                }
            }
            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }
    }
}
