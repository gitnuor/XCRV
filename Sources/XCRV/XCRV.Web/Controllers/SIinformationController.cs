using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.Web.Controllers
{
    public class SIinformationController : BaseController
    {
        private readonly ILogger<SIinformationController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SIinformationController(ILogger<SIinformationController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchSIinformation(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            IList<SIinformation> data = new List<SIinformation>();

            string message = "Sorry!!! No Data Found!!!";
            data = (await _unitOfWork.DebitCardRepo.GetSIinformation(seachString)).ToList();

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }
    }
}
