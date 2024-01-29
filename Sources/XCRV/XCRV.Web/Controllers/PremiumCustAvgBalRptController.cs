using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.Web.Controllers
{
    public class PremiumCustAvgBalRptController : BaseController
    {

        private readonly ILogger<PremiumCustAvgBalRptController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PremiumCustAvgBalRptController(ILogger<PremiumCustAvgBalRptController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getPremimumAvgBal(string seachType, string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            //int period = Convert.ToInt32(seachType.SelectedValue);
            decimal balance = Convert.ToDecimal(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            IList<PremimunCustAvgBal> data = new List<PremimunCustAvgBal>();

            string message = "Sorry!!! No Data Found!!!";
            data = (await _unitOfWork.DebitCardRepo.GetAvgPremiumBal(seachType, balance.ToString())).ToList();

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

    }
}
