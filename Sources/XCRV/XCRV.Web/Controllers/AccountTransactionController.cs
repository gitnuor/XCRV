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
    public class AccountTransactionController : BaseController
    {
        private readonly ILogger<CustomerLimitController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public AccountTransactionController(ILogger<CustomerLimitController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accno)
        {
            ViewBag.Accno = accno;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getCustomerBal(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string fdate = FromDate.ToString();
                string tdate = ToDate.ToString();
                EffectiveBal data=new EffectiveBal();
                if (!string.IsNullOrEmpty(accno))
                {
                    accno = HttpUtility.HtmlEncode(accno.Trim());
                }
                if (fdate != "1/1/0001 12:00:00 AM" && tdate != "1/1/0001 12:00:00 AM")
                {
                    data = await _unitOfWork.AccountSchemRepo.GetCustomerBal(accno, FromDate, ToDate);
                }

               // var data = await _unitOfWork.AccountSchemRepo.GetCustomerBal(accno, FromDate, ToDate);//.CustomerLimitRepo.GetCustomerLimitByCustid(Custid.Trim());

                return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> getCustomerName(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string fdate = FromDate.ToString();
                string tdate = ToDate.ToString();
                FinStatementDetails data=new FinStatementDetails();
                if (!string.IsNullOrEmpty(accno))
                {
                    accno = HttpUtility.HtmlEncode(accno.Trim());
                }
                if (fdate != "1/1/0001 12:00:00 AM" && tdate!= "1/1/0001 12:00:00 AM")
                {
                    data = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
                }
               // var data = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);//AccountSchemRepo.GetCustomerBal(accno, FromDate, ToDate);//.CustomerLimitRepo.GetCustomerLimitByCustid(Custid.Trim());

                return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> getAccountTransactionDetails(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string msg = string.Empty;
                string fdate = FromDate.ToString();
                string tdate = ToDate.ToString();
                var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

                IList<AccountADCTransaction> data = new List<AccountADCTransaction>();
                if (accno == null)
                {
                    msg = "<font color='red'><b>Account no can not be empty.</b></font>"; //"ATM No can not be empty.";
                }
                else if (fdate == "1/1/0001 12:00:00 AM")
                {
                    msg = "<font color='red'><b>From Date can not be empty.</b></font>";
                }
                else if (tdate == "1/1/0001 12:00:00 AM")
                {
                    msg = "<font color='red'><b>To Date can not be empty.</b></font>";
                }
                else
                {
                    string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(accno.Trim());
                    if (((schemeCode == "SRSTF" && IsStatementTrue == "N")
                    || (!await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accno.Trim(), userName))))
                    {
                        msg = "You are not authorized to view this information!!!";
                    }
                    else
                    {
                        data = await _unitOfWork.TransactionDetailsRepo.GetAccountTransactionDetails(accno, FromDate, ToDate);//.GetTransactionDetails(seachString, FromDate, ToDate);//CustomerLimitRepo.getCustomerLimit(seachString, isStatementTrue);
                        if (data.Count == 0 || data == null)
                        {
                            msg = "<font color='red'><b>No Data Found!</b></font>";
                        }
                        else if (data.Count == 1 && string.IsNullOrEmpty(data[0].tran_id))
                        {
                            msg = "<font color='red'><b>No Data Found!</b></font>";
                            data = new List<AccountADCTransaction>();
                        }
                        else
                        {
                            string patCollection = _configuration.GetSection("AppSettings").GetSection("CARD_BIN_LIST").Value;//Configuration.GetValue<string>("MySettings:DbConnection");

                            var splitcollection = patCollection.Split(',');
                            string maskResult = "";
                            foreach (var mask in data.ToList())
                            {
                                maskResult = mask.tran_part.ToString().Length > 10 ? new Helpers.MaskCardNumber().Mask(mask.tran_part.ToString(), splitcollection) : mask.tran_part.ToString();
                                mask.tran_id = mask.tran_id;
                                mask.tran_date = mask.tran_date;
                                mask.tran_part = maskResult;
                                mask.deposit = mask.deposit;
                                mask.withdraw = mask.withdraw;
                            }
                        }
                    }
                }
                return Json(new { data = data, status = "success", message = msg, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }

        }

    }
}
