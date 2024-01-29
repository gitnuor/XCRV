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
    public class LcinformationController : BaseController
    {
        private readonly ILogger<LcinformationController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public LcinformationController(ILogger<LcinformationController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchLCinformation(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            IList<LCinformation> data = new List<LCinformation>();

            string message = "Sorry!!! No Data Found!!!";
            data = (await _unitOfWork.DebitCardRepo.GetLCinformation(seachString)).ToList();

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpGet]
        public async Task<IActionResult> getLcDetails(string seachString, string searchLcNo)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

             LCinformation data = new LCinformation();

            string message = "Sorry!!! No Data Found!!!";
            data = await _unitOfWork.DebitCardRepo.GetLcDetails(seachString, searchLcNo);

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }
    }
}
