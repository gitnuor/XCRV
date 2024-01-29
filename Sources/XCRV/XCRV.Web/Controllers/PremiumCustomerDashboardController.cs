using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using System.Linq;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace XCRV.Web.Controllers
{
    public class PremiumCustomerDashboardController : BaseController
    {
        private readonly ILogger<PremiumCustomerDashboardController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PremiumCustomerDashboardController(ILogger<PremiumCustomerDashboardController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            DateTime dtCurrentDate = DateTime.Now;
            ViewBag.Schm_Code = "Scheme Code";
            ViewBag.dtCurrentDate1 = dtCurrentDate.AddMonths(-6).ToString("MMM yyyy");
            ViewBag.dtCurrentDate2 = dtCurrentDate.AddMonths(-5).ToString("MMM yyyy");
            ViewBag.dtCurrentDate3 = dtCurrentDate.AddMonths(-4).ToString("MMM yyyy");
            ViewBag.dtCurrentDate4 = dtCurrentDate.AddMonths(-3).ToString("MMM yyyy");
            ViewBag.dtCurrentDate5 = dtCurrentDate.AddMonths(-2).ToString("MMM yyyy");
            ViewBag.dtCurrentDate6 = dtCurrentDate.AddMonths(-1).ToString("MMM yyyy");
            ViewBag.Avgmon = "6 Months Avg.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> IsPermiumCustomer(string seachString)
        {
            //if (IsPermiumCustomer(seachString).Result==false)
            //{
            //    TempData["ErrorMessage"] = "Provided Customer ID is not of a premium banking customer.";

            //}
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            PremimumAc preAcData = new PremimumAc();
            preAcData = await _unitOfWork.DebitCardRepo.GetPremimumCustId(seachString);
            string message = "Sorry!!! No Data Found!!!";
            return Json(new { data = preAcData, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpGet]
        public async Task<IActionResult> getPersonelInfo(string seachString)
        {
            //if (IsPermiumCustomer(seachString).Result==false)
            //{
            //    TempData["ErrorMessage"] = "Provided Customer ID is not of a premium banking customer.";
 
            //}

            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            PremimumPersonalInfo data = new PremimumPersonalInfo();

            string message = "Sorry!!! No Data Found!!!";
            data = await _unitOfWork.DebitCardRepo.GetPremimumPersonelInfo(seachString);

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        //protected async Task<bool>  IsPermiumCustomer(string pstrCustID)
        //{
        //    PremimumAc preAcData = new PremimumAc();
        //    preAcData= await _unitOfWork.DebitCardRepo.GetPremimumCustId(pstrCustID);
        //    if (preAcData != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> getAccountTotal(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            PremimumPersonalInfo data1 = new PremimumPersonalInfo();
            PremimumPersonalInfo data2 = new PremimumPersonalInfo();
            PremimumPersonalInfo data3 = new PremimumPersonalInfo();
            PremimumPersonalInfo data4 = new PremimumPersonalInfo();

            string message = "Sorry!!! No Data Found!!!";
            data1 = await _unitOfWork.DebitCardRepo.GetPremimumAccountTotal(seachString);           
            data2= await _unitOfWork.DebitCardRepo.GetPremimumTotDepositBal(seachString);
            data3= await _unitOfWork.DebitCardRepo.GetPremimumTotLoanBal(seachString);

            data4.tot_int_paid = data1.tot_int_paid;
            data4.tot_int_rec = data1.tot_int_rec;
            data4.tot_balance=data2.tot_balance;
            data4.total=data3.total;

            return Json(new { data = data4, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpGet]
        public async Task<IActionResult> getDeposits(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            IList<PremimumPersonalInfo> data = new List<PremimumPersonalInfo>();

            string message = "Sorry!!! No Data Found!!!";
            data = (await _unitOfWork.DebitCardRepo.GetPremimumDeposits(seachString)).ToList();

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpGet]
        public async Task<IActionResult> getDepositAllaccinfo(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            PremimumPersonalInfo data = new PremimumPersonalInfo();

            string message = "Sorry!!! No Data Found!!!";
            data = await _unitOfWork.DebitCardRepo.GetPremimumTotDepositBal(seachString);

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpGet]
        public async Task<IActionResult> getAssets(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            IList<PremimumAsset> data = new List<PremimumAsset>();

            string message = "Sorry!!! No Data Found!!!";
            data = (await _unitOfWork.DebitCardRepo.GetPremimumAssets(seachString)).ToList();

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpGet]
        public async Task<IActionResult> getAssetAllaccinfo(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            PremimumAsset data = new PremimumAsset();

            string message = "Sorry!!! No Data Found!!!";
            data = await _unitOfWork.DebitCardRepo.GetPremimumTotAssetBal(seachString);

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpGet]
        public async Task<IActionResult> getHistoricalBal(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            PremimumAsset data1 = new PremimumAsset();
            PremimumAsset data2 = new PremimumAsset();
            PremimumAsset data3 = new PremimumAsset();

            string message = "Sorry!!! No Data Found!!!";
            data1 = await _unitOfWork.DebitCardRepo.Get6monthavgTotal(seachString);
            data2 = await _unitOfWork.DebitCardRepo.Get1yearavgTotal(seachString);

           // string  6monthavg1 = string.Empty;
           var  monthavg16 = (Convert.ToDouble(data1.avgbalance) < 0.0 ? "0.00" : Convert.ToDouble(data1.avgbalance).ToString("0,000.00"));
           var monthavg1year = (Convert.ToDouble(data2.avgbalance2) < 0.0 ? "0.00" : Convert.ToDouble(data2.avgbalance).ToString("0,000.00"));


            data3.avgbalance = monthavg16;
            data3.avgbalance2 = monthavg1year;

            return Json(new { data = data3, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpGet]
        public async Task<IActionResult> getMonthWiseAvgBal(string seachString)
        {
            seachString = HttpUtility.HtmlEncode(seachString);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            IList<PremimumAvgBal> data = new List<PremimumAvgBal>();
            PremimumAvgBal data1 =new PremimumAvgBal();
            PremimumAvgBal data2 = new PremimumAvgBal();
            PremimumAvgBal data3 = new PremimumAvgBal();
            //PremimumAvgBal[] data4=new PremimumAvgBal[6];


            string message = "Sorry!!! No Data Found!!!";
            data = (await _unitOfWork.DebitCardRepo.GetDepMove12MonBySchm(seachString)).ToList();

            data.ToList().ForEach(p=>p.avgBal = (
                                                    ((String.IsNullOrEmpty(p.mon1)?0: Convert.ToDouble(p.mon1)) +
                                                    (String.IsNullOrEmpty(p.mon2) ? 0 : Convert.ToDouble(p.mon2)) +
                                                    (String.IsNullOrEmpty(p.mon3) ? 0 : Convert.ToDouble(p.mon3)) +
                                                    (String.IsNullOrEmpty(p.mon4) ? 0 : Convert.ToDouble(p.mon4))+
                                                    (String.IsNullOrEmpty(p.mon5) ? 0 : Convert.ToDouble(p.mon5))+ 
                                                    (String.IsNullOrEmpty(p.mon6) ? 0 : Convert.ToDouble(p.mon6))) /6

                                                ).ToString());
            data.ToList().ForEach(p => p.mon6 = String.Format("{0:#,##0.##}", Convert.ToDouble(p.mon6.ToString() == string.Empty ? "0" : p.mon6)));
            data.ToList().ForEach(p => p.mon5 = String.Format("{0:#,##0.##}", Convert.ToDouble(p.mon5.ToString() == string.Empty ? "0" : p.mon5)));
            data.ToList().ForEach(p => p.mon4 = String.Format("{0:#,##0.##}", Convert.ToDouble(p.mon4.ToString() == string.Empty ? "0" : p.mon4)));
            data.ToList().ForEach(p => p.mon3 = String.Format("{0:#,##0.##}", Convert.ToDouble(p.mon3.ToString() == string.Empty ? "0" : p.mon3)));
            data.ToList().ForEach(p => p.mon2 = String.Format("{0:#,##0.##}", Convert.ToDouble(p.mon2.ToString() == string.Empty ? "0" : p.mon2)));
            data.ToList().ForEach(p => p.mon1 = String.Format("{0:#,##0.##}", Convert.ToDouble(p.mon1.ToString() == string.Empty ? "0" : p.mon1)));


            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
        }

    }
}
