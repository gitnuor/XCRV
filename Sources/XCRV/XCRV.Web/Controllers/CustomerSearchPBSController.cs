using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.CommonEnums;
using XCRV.Domain.Entities;

namespace XCRV.Web.Controllers
{
    public class CustomerSearchPBSController : BaseController
    {
        private readonly ILogger<CustomerSearchPBSController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public CustomerSearchPBSController(ILogger<CustomerSearchPBSController> logger, IUnitOfWork unitOfWork)
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
        public async Task<IActionResult> SearchCustomerBySearchCriteria(string seachType, string seachString)
        {
            CustomerSearchType type;
            string extensiveType = string.Empty;
            string _pbsFlag = "2";
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
                case "Mobile":
                    type = CustomerSearchType.SP_EXTENSIVE_SEARCH_PBS;
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
                        foreach (var d in data)
                        { 
                          d.astha_status=(await _unitOfWork.CustomerSearchRepo.getAsthaStatus(d.cust_id,isStatementTrue)).FirstOrDefault().astha_status;
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
            
                foreach(var d in data)
                {
                    d.astha_status = (await _unitOfWork.CustomerSearchRepo.getAsthaStatus(d.cust_id, isStatementTrue)).FirstOrDefault().astha_status;
                    
                }
            }
            
            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }
    }
}
