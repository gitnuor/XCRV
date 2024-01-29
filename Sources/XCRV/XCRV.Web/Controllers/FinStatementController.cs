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
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class FinStatementController : BaseController
    {
        private readonly ILogger<FinStatementController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public FinStatementController(ILogger<FinStatementController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accno, DateTime FromDate, DateTime ToDate)
        {
            ViewBag.Accno = accno;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getCustomerBal(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(accno))
                {
                    accno = HttpUtility.HtmlEncode(accno);
                }
                var claims = User.Claims;
                var data = await _unitOfWork.AccountSchemRepo.GetCustomerBal(accno, FromDate, ToDate);

                string groupName = claims.FirstOrDefault(p => p.Type.Equals("GroupName")).Value.ToString();
                if (data!= null)
                {
                    if (groupName == "Y")
                    {
                        string msg1 = "<font color='red'><b>YOU ARE NOT AUTHORIZED TO VIEW DATA!!</b></font>";
                       var custid= await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
                        string probasiFlag = await _unitOfWork.CustomerSearchRepo.GetProbasiFlagByUser(custid.cust_id);
                        if (probasiFlag == "Y")
                        {
                            return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
                        }
                        else
                        {
                            return Json(new { data = 0, status = "failed", message = msg1, result = CommonAjaxResponse("failed", "failed", "200") });
                        }
                    }
                }

                return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }


        [HttpGet]
        public async Task<IActionResult> getCustomerGeneralInfo(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                FinStatementDetails data = new FinStatementDetails();
                var claims = User.Claims;
                if (!string.IsNullOrEmpty(accno))
                {
                    accno = HttpUtility.HtmlEncode(accno);
                }
                string msg = string.Empty;
                string fdate= FromDate.ToString();
                string tdate = ToDate.ToString();
                //if (fdate == null)
                //{ 
                //   fdate=
                //}
                // string fdate = TempData["FromDate"].ToString();
                //string tdate = TempData["ToDate"].ToString();
                if (accno == null)
                {
                    return NotFound();
                }
                else if (fdate == "1/1/0001 12:00:00 AM")
                {
                    return NotFound();
                }
                else if (tdate == "1/1/0001 12:00:00 AM")
                {
                    return NotFound();
                }
                else {
                    data = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
                }

                string groupName = claims.FirstOrDefault(p => p.Type.Equals("GroupName")).Value.ToString();
                if (data!=null)
                {
                    if (groupName == "Y")
                    {
                        string msg1 = "<font color='red'><b>YOU ARE NOT AUTHORIZED TO VIEW DATA!!</b></font>";
                        string probasiFlag = await _unitOfWork.CustomerSearchRepo.GetProbasiFlagByUser(data.cust_id);
                        if (probasiFlag == "Y")
                        {
                            return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
                        }
                        else
                        {
                            return Json(new { data =0, status = "failed", message = msg, result = CommonAjaxResponse("failed", "failed", "200") });
                        }
                    }
                }

                return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> getFnTransactionDetails(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string extensiveType = string.Empty;
                ViewBag.Accno = accno;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;

                TempData["FromDate"] = FromDate;
                TempData["ToDate"] = ToDate;


                if (!string.IsNullOrEmpty(accno))
                {
                    accno = HttpUtility.HtmlEncode(accno);
                }
                string msg = string.Empty;
                string fdate = FromDate.ToString();
                string tdate = ToDate.ToString();
                var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

                IList<FinStatementDetails> data1 = new List<FinStatementDetails>();
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
                        data1 = await _unitOfWork.TransactionDetailsRepo.GetFinTransactionDetails(accno, FromDate, ToDate);
                        if (data1.Count == 0 || data1 == null)
                        {
                            msg = "<font color='red'><b>No Data Found!</b></font>";
                        }
                        else if (data1.Count == 1 && string.IsNullOrEmpty(data1[0].tran_id))
                        {
                            msg = "<font color='red'><b>No Data Found!</b></font>";
                            data1 = new List<FinStatementDetails>();
                        }
                        else {
                            string patCollection = _configuration.GetSection("AppSettings").GetSection("CARD_BIN_LIST").Value;//Configuration.GetValue<string>("MySettings:DbConnection");

                            var splitcollection = patCollection.Split(',');
                            string maskResult = "";

                            decimal withdraw = 0;
                            decimal deposit = 0;
                            decimal balance = 0;

                            foreach (var mask in data1.ToList())
                            {
                                maskResult = string.IsNullOrEmpty(mask.tran_particular) ? "" : mask.tran_particular.ToString().Length > 10 ? new Helpers.MaskCardNumber().Mask(mask.tran_particular.ToString(), splitcollection) : mask.tran_particular.ToString();
                                mask.tran_id = mask.tran_id;
                                mask.value_date = mask.value_date;
                                mask.tran_particular = maskResult;
                                mask.instrmnt_num = mask.instrmnt_num;
                                mask.withdraw = mask.withdraw;
                                mask.deposit = mask.deposit;
                                mask.balance = mask.balance;
                                //data.Append(mask);
                                withdraw += (string.IsNullOrEmpty(mask.withdraw) ? 0 : Convert.ToDecimal(mask.withdraw));
                                deposit += (string.IsNullOrEmpty(mask.deposit) ? 0 : Convert.ToDecimal(mask.deposit));
                                balance += (string.IsNullOrEmpty(mask.balance) ? 0 : Convert.ToDecimal(mask.balance));
                            }

                            var summary = new FinStatementDetails();
                            summary.balance = balance.ToString();
                            summary.withdraw = withdraw.ToString();
                            summary.deposit = deposit.ToString();

                            data1.Add(summary);
                        }
                       
                    }
                }

                var claims = User.Claims;
                string groupName = claims.FirstOrDefault(p => p.Type.Equals("GroupName")).Value.ToString();
                if (data1.Count>0)
                {
                    if (groupName == "Y")
                    {
                        string msg1 = "YOU ARE NOT AUTHORIZED TO VIEW DATA!!";
                       var custid= await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
                        string probasiFlag = await _unitOfWork.CustomerSearchRepo.GetProbasiFlagByUser(custid.cust_id);
                        if (probasiFlag == "Y")
                        {
                            return Json(new { data = data1, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
                        }
                        else
                        {
                            return Json(new { data = 0, status = "failed", message = msg1, result = CommonAjaxResponse("failed", "failed", "200") });
                        }
                    }
                }

                return Json(new { data = data1, status = "success", message = msg, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> getCustomerRewardPoint(string accno)
        {
            try
            {
               // RewardPoint rewardPointCall = new RewardPoint();
                //if (!string.IsNullOrEmpty(accno))
                //{
                //    accno = HttpUtility.HtmlEncode(accno);
                //}
               
                var rewardPointCall = await _unitOfWork.RewardPointRepo.CurrentMonthStmtRPBal(accno);
                //if (rewardPointCall == null)
                //{
                //    return NotFound();
                //}
                return Json(new { data = rewardPointCall, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }

        public async Task<IActionResult> TranDetails(string tranId, DateTime valueDate)
        {
            string seachType = "CustomerID";
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            ClientBorrowerViewModel data = new ClientBorrowerViewModel();
            string message = "Sorry!!! No Data Found!!!";

            data.tranDetails = (await _unitOfWork.CustomerSearchRepo.SearchByTranId(tranId, valueDate)).ToList();

            string patCollection = _configuration.GetSection("AppSettings").GetSection("CARD_BIN_LIST").Value;//Configuration.GetValue<string>("MySettings:DbConnection");

            var splitcollection = patCollection.Split(',');
            string maskResult = "";

            decimal withdraw = 0;
            decimal deposit = 0;
            decimal balance = 0;

            foreach (var mask in data.tranDetails.ToList())
            {
                maskResult = string.IsNullOrEmpty(mask.tran_particular) ? "" : mask.tran_particular.ToString().Length > 10 ? new Helpers.MaskCardNumber().Mask(mask.tran_particular.ToString(), splitcollection) : mask.tran_particular.ToString();
                mask.tran_id = mask.tran_id;
                mask.value_date = mask.value_date;
                mask.tran_particular = maskResult;
                mask.instrmnt_num = mask.instrmnt_num;
                mask.withdraw = mask.withdraw;
                mask.deposit = mask.deposit;
                mask.balance = mask.balance;
            }
           // data1.Add(summary);

            if (data.tranDetails == null)
            {
                data.tranDetails = new List<FinStatementDetails>();
            }
            return PartialView("_tranDetail", data.tranDetails);
            // return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }

    }
}
