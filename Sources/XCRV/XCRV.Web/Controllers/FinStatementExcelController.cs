using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class FinStatementExcelController : BaseController
    {
        private readonly ILogger<FinStatementExcelController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public FinStatementExcelController(ILogger<FinStatementExcelController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
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

                var data = await _unitOfWork.AccountSchemRepo.GetCustomerBal(accno, FromDate, ToDate);

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
                if (!string.IsNullOrEmpty(accno))
                {
                    accno = HttpUtility.HtmlEncode(accno);
                }
                string msg = string.Empty;
                string fdate = FromDate.ToString();
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
                else
                {
                    data = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
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
                        else
                        {
                            string patCollection = _configuration.GetSection("AppSettings").GetSection("CARD_BIN_LIST").Value;//Configuration.GetValue<string>("MySettings:DbConnection");

                            var splitcollection = patCollection.Split(',');
                            string maskResult = "";

                            decimal withdraw = 0;
                            decimal deposit = 0;
                            decimal balance = 0;

                            foreach (var mask in data1.ToList())
                            {
                                maskResult = string.IsNullOrEmpty(mask.tran_particular)?"": mask.tran_particular.ToString().Length > 10 ? new Helpers.MaskCardNumber().Mask(mask.tran_particular.ToString(), splitcollection) : mask.tran_particular.ToString();
                                //maskResult =  mask.tran_particular.ToString().Length > 10 ? new Helpers.MaskCardNumber().Mask(mask.tran_particular.ToString(), splitcollection) : mask.tran_particular.ToString();
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
                                balance = (string.IsNullOrEmpty(mask.balance) ? 0 : Convert.ToDecimal(mask.balance));
                            }

                            var summary = new FinStatementDetails();
                            summary.balance = balance.ToString();
                            summary.withdraw = withdraw.ToString();
                            summary.deposit = deposit.ToString();

                            data1.Add(summary);
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

        [HttpPost]
        public async Task<IActionResult> ExportData(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string extensiveType = string.Empty;

                accno = HttpUtility.HtmlEncode(accno);

                var claims = User.Claims;
                string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();

                var data = await _unitOfWork.TransactionDetailsRepo.GetFinTransactionDetails(accno, FromDate, ToDate);//CustomerLimitRepo.getCustomerLimit(seachString, isStatementTrue);

                decimal withdraw = 0;
                decimal deposit = 0;
                decimal balance = 0;

                foreach (var d in data.ToList())
                {
                    d.value_date = string.IsNullOrEmpty(d.value_date) ? "" : Convert.ToDateTime(d.value_date).ToString("dd-MMM-yyyy");

                    withdraw = withdraw + Convert.ToDecimal(string.IsNullOrEmpty(d.withdraw) ? "0" : d.withdraw);
                    deposit = deposit + Convert.ToDecimal(string.IsNullOrEmpty(d.deposit) ? "0" : d.deposit);
                    balance = Convert.ToDecimal(string.IsNullOrEmpty(d.balance) ? "0" : d.balance);


                    //d.withdraw = Convert.ToDecimal(string.IsNullOrEmpty(d.withdraw) ? "0" : d.withdraw).ToString(".00").PadLeft(20, ' ');
                    //d.deposit = Convert.ToDecimal(string.IsNullOrEmpty(d.deposit) ? "0" : d.deposit).ToString(".00").PadLeft(20, ' ');
                    //d.balance = Convert.ToDecimal(string.IsNullOrEmpty(d.balance) ? "0" : d.balance).ToString(".00").PadLeft(20, ' ');

                }

                if (data.Count > 0)
                {
                    FinStatementDetails footer = new FinStatementDetails();
                    footer.withdraw = withdraw.ToString(".00").PadLeft(20, ' ');
                    footer.deposit = deposit.ToString(".00").PadLeft(20, ' ');
                    footer.balance = balance.ToString(".00").PadLeft(20, ' ');

                    data.Add(footer);
                }


                if (data.Count == 0 || data == null)
                {
                    return NotFound();
                }
                else if (data.Count == 1 && string.IsNullOrEmpty(data[0].tran_id))
                {
                    return NotFound();
                }
                else
                {
                    string name = string.Empty;
                    string address = string.Empty;
                    string currency = string.Empty;
                    string acctno = string.Empty;
                    string acctype = string.Empty;
                    string custid = string.Empty;
                    var data1 = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
                    if(data1 == null)
                    {
                        name = string.Empty;
                        address = string.Empty;
                        currency = string.Empty;
                        acctno = accno;
                        acctype = string.Empty;
                        custid = string.Empty;
                    }
                    else
                    {
                        name = data1.acct_name;
                        address = data1.address;
                        currency = data1.acct_crncy_code;
                        acctno = data1.foracid;
                        acctype = data1.account_type;
                        custid = data1.cust_id;
                    }
                    

                    DataTable dataTable = new DataTable(typeof(FinStatementDetails).Name);
                    //Get all the properties
                    PropertyInfo[] Props = typeof(FinStatementDetails).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo prop in Props)
                    {
                        //Defining type of data column gives proper data table 
                        var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                        //Setting column names as Property names
                        dataTable.Columns.Add(prop.Name.ToUpper(), type);
                    }
                    
                    string valdate = string.Empty;
                    string tran = string.Empty;
                    foreach (FinStatementDetails item in data)
                    {
                        var values = new object[Props.Length];
                        for (int i = 0; i < Props.Length; i++)
                        {
                            //inserting property values to datatable rows
                            values[i] = Props[i].GetValue(item, null);
                        }


                        dataTable.Rows.Add(values);
                    }


                    dataTable.Columns.Remove("CUST_ID");
                    dataTable.Columns.Remove("ACCT_NAME");
                    dataTable.Columns.Remove("ACCT_CRNCY_CODE");
                    dataTable.Columns.Remove("ADDRESS");
                    dataTable.Columns.Remove("FORACID");
                    dataTable.Columns.Remove("ACCOUNT_TYPE");
                    dataTable.Columns.Remove("TRAN_ID");
                    // dataTable.Columns.Remove("INSTRMNT_NUM");
                    dataTable.Columns["VALUE_DATE"].ColumnName = "DATE";
                    dataTable.Columns["TRAN_PARTICULAR"].ColumnName = "PARTICULARS";
                    dataTable.Columns["INSTRMNT_NUM"].ColumnName = "INST NO.";
                    string rpt_name = "Account Statement Report";
                    byte[] buffer = ExportExcel(dataTable, rpt_name, accno, FromDate, ToDate, name, address, currency, acctno, acctype, custid);
                    //byte[] buffer = Encoding.ASCII.GetBytes(filecontent.ToString().ToCharArray());               
                    var ms = new MemoryStream(buffer);
                    var contentType = "application/vnd.ms-excel";
                    return File(ms, contentType.ToString(), "Statement_HR.xls");
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }

        public static byte[] ExportExcel(DataTable dataTable, string heading, string accno,
           DateTime FromDate, DateTime ToDate, string name, string address, string currency, string acctno, string acctype, string custid)
        {
            bool showSrNo = false;

            var columnsToTake = new List<string>();

            foreach (DataColumn dc in dataTable.Columns)
            {
                columnsToTake.Add(dc.ColumnName);
            }

            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {

                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data ", heading));
                int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 11;


                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("Serial#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }

                // add the content into the Excel file  
                workSheet.Cells[String.Format("A{0}", startRowFrom)].LoadFromDataTable(dataTable, true);
                // format header 
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {

                    r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // format cells - add borders  
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // removed ignored columns  
                for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && showSrNo)
                    {
                        continue;
                    }
                    if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                    {
                        workSheet.DeleteColumn(i + 1);
                    }
                }

                if (!String.IsNullOrEmpty(heading))
                {
                    workSheet.Cells["A1"].Value = "Report Generation Date: " + DateTime.Today.ToString("dd-MMM-yyyy");
                    workSheet.Cells["A1"].Style.Font.Size = 11;
                    workSheet.Cells["A1"].Style.Font.Bold = true;

                    //workSheet.Cells["A1"].Value = heading;
                    //workSheet.Cells["A1"].Style.Font.Size = 11;
                    //workSheet.Cells["A1"].Style.Font.Bold = true;

                    workSheet.Cells["A2"].Value = "Name : " + name;
                    workSheet.Cells["A2"].Style.Font.Size = 10;
                    workSheet.Cells["A2"].Style.Font.Bold = true;

                    workSheet.Cells["A3"].Value = "Address : " + address;
                    workSheet.Cells["A3"].Style.Font.Size = 10;
                    workSheet.Cells["A3"].Style.Font.Bold = true;

                    workSheet.Cells["A4"].Value = "Customer ID: " + custid;
                    workSheet.Cells["A4"].Style.Font.Size = 10;
                    workSheet.Cells["A4"].Style.Font.Bold = true;

                    workSheet.Cells["A5"].Value = "Account No : " + acctno;
                    workSheet.Cells["A5"].Style.Font.Size = 10;
                    workSheet.Cells["A5"].Style.Font.Bold = true;

                    workSheet.Cells["A6"].Value = "A/C Type: " + acctype;
                    workSheet.Cells["A6"].Style.Font.Size = 10;
                    workSheet.Cells["A6"].Style.Font.Bold = true;

                    workSheet.Cells["A7"].Value = "Currency : " + currency;
                    workSheet.Cells["A7"].Style.Font.Size = 10;
                    workSheet.Cells["A7"].Style.Font.Bold = true;

                    workSheet.Cells["A9"].Value = "STATEMENT OF ACCOUNT FOR THE PERIOD OF : "
                        + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + " " + "To" + " " + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy");
                    workSheet.Cells["A9"].Style.Font.Size = 11;
                    workSheet.Cells["A9"].Style.Font.Bold = true;

                }
                // workSheet.Cells["A"+(11 + dataTable.Rows.Count + 3)].Value = "This is a computer-generated statement and does not require any signature.";

                result = package.GetAsByteArray();
            }

            return result;
        }

        public async Task<IActionResult> GetTransactionStatement(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string extensiveType = string.Empty;

                accno = HttpUtility.HtmlEncode(accno);

                var claims = User.Claims;
                string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();

                var data = await _unitOfWork.TransactionDetailsRepo.GetFinTransactionDetails(accno, FromDate, ToDate);//CustomerLimitRepo.getCustomerLimit(seachString, isStatementTrue);

                decimal withdraw = 0;
                decimal deposit = 0;
                decimal balance = 0;

                foreach (var d in data.ToList())
                {
                    d.value_date = string.IsNullOrEmpty(d.value_date) ? "" : Convert.ToDateTime(d.value_date).ToString("dd-MMM-yyyy");
                    withdraw = withdraw + Convert.ToDecimal(string.IsNullOrEmpty(d.withdraw) ? "0" : d.withdraw);
                    deposit = deposit + Convert.ToDecimal(string.IsNullOrEmpty(d.deposit) ? "0" : d.deposit);
                    balance = Convert.ToDecimal(string.IsNullOrEmpty(d.balance) ? "0" : d.balance);
                }

                if (data.Count > 0)
                {
                    FinStatementDetails footer = new FinStatementDetails();
                    footer.withdraw = withdraw.ToString("#,#.##", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")).PadLeft(20, ' ');
                    footer.deposit = deposit.ToString("#,#.##", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")).PadLeft(20, ' ');
                    footer.balance = balance.ToString("#,#.##", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")).PadLeft(20, ' ');
                    data.Add(footer);
                }

                var customer = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
                string StatementHeader = "STATEMENT OF ACCOUNT FOR THE PERIOD OF ";
                StatementHeader = StatementHeader.Replace(StatementHeader, @"\b" + StatementHeader + @"\b0");
                //StatementHeader ="<b>STATEMENT OF ACCOUNT FOR THE PERIOD OF </b>";  
                var reportHeader = "STATEMENT OF ACCOUNT FOR THE PERIOD OF " + FromDate.ToString("dd-MMM-yyyy") + " To " + ToDate.ToString("dd-MMM-yyyy") + "";

                return Json(new { data = data, customer = customer, header = reportHeader, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });


            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }

        public async Task<IActionResult> TranDetails(string tranId,DateTime valueDate)
        {
            string seachType = "CustomerID";
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            // IList<CustomerSearch> data = new List<CustomerSearch>();
            ClientBorrowerViewModel data = new ClientBorrowerViewModel();
            string message = "Sorry!!! No Data Found!!!";

            data.tranDetails = (await _unitOfWork.CustomerSearchRepo.SearchByTranId(tranId, valueDate)).ToList();

            if (data.tranDetails == null)
            {
                data.tranDetails = new List<FinStatementDetails>();
            }
            return PartialView(data.tranDetails);
            // return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }

    }
}
