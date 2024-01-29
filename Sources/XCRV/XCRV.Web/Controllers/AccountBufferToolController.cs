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

namespace XCRV.Web.Controllers
{
    public class AccountBufferToolController : BaseController
    {
        private readonly ILogger<AccountBufferToolController> _logger;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        private string[] cardBinList;

        public AccountBufferToolController(ILogger<AccountBufferToolController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            cardBinList = _configuration.GetSection("AppSettings").GetSection("CARD_BIN_LIST").Value.Split(','); 
        }

        [Filters.AuthorizeActionFilter]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchAccountBufferTool(string accountNo)
        {

            accountNo = HttpUtility.HtmlEncode(accountNo);

            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            IList<AccountBufferTool> data = new List<AccountBufferTool>();

            string message = "Sorry!!! No Data Found!!!";

            
            if (AccountNumberValidationHelper.IsAccountNoValid(accountNo))
            {
                bool isAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
                if (!isAccessable)
                {
                    message = "Sorry!!! You are not authorized to view this information!!!";
                }
                else if (AccountNumberValidationHelper.IsAccountNoValid(accountNo))
                {
                    data = (await _unitOfWork.AccountBufferToolRepo.GetAccountBuffer(accountNo)).ToList();

                    decimal TotalChargeAmt = 0;
                    decimal TotalCollAmt = 0;
                    decimal TotalWaiveAmt = 0;
                    decimal TotalDueAmt = 0;

                    foreach (var row in data)
                    {
                        object cellData = row.Particulars;
                        row.Particulars = cellData.ToString().Length > 10 ? new Helpers.MaskCardNumber().Mask(cellData.ToString(), cardBinList) : cellData.ToString();

                        string CharAmt = row.Chrg_Amt.ToString();
                        TotalChargeAmt = TotalChargeAmt + decimal.Parse(CharAmt);
                        string CollAmt = row.Chrg_Adj_Amt.ToString();
                        TotalCollAmt = TotalCollAmt + decimal.Parse(CollAmt);
                        string WaiveAmt = row.Chrg_Waive_Amt.ToString();
                        TotalWaiveAmt = TotalWaiveAmt + decimal.Parse(WaiveAmt);
                        string DueAmt = row.Due_Amt.ToString(); ;
                        TotalDueAmt = TotalDueAmt + decimal.Parse(DueAmt);
                    }
                    ViewBag.TotalChargeAmt = TotalChargeAmt;

                    //data.Add(new AccountBufferTool()
                    //{
                    //    Chrg_Amt = TotalChargeAmt.ToString(),
                    //    Chrg_Adj_Amt = TotalCollAmt.ToString(),
                    //    Chrg_Waive_Amt = TotalWaiveAmt.ToString(),
                    //    Due_Amt = TotalDueAmt.ToString()
                    //});
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

           
           

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }
    }
}
