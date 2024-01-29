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
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using IronPdf;
using System.Globalization;

namespace XCRV.Web.Controllers
{
    public class FinStatementHRController : BaseController
    {
        private readonly ILogger<FinStatementHRController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public FinStatementHRController(ILogger<FinStatementHRController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
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

        public async Task<IActionResult> ShowPdf(string accno, DateTime FromDate, DateTime ToDate)
        {
            accno = HttpUtility.HtmlEncode(accno);
            string sb = await GetHTMLString(accno, FromDate, ToDate);
            return Content(sb.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> ShowPdfDownload(string accno, DateTime FromDate, DateTime ToDate)
        {
            accno = HttpUtility.HtmlEncode(accno);
            string sb = await GetHTMLString(accno, FromDate, ToDate);

            byte[] buffer = await ExportPdf(accno, FromDate, ToDate);
            var ms = new MemoryStream(buffer);
            var contentType = "application/pdf";
            return File(ms, contentType);
        }

        [HttpGet]
        public async Task<FileContentResult> ShowPdfDownload1(string accno, DateTime FromDate, DateTime ToDate)
        {
            var csvString = await  GetHTMLString(accno, FromDate, ToDate);
            var fileName = "HR_Data " + DateTime.Now.ToString() + ".csv";
           return File(new System.Text.UTF8Encoding().GetBytes(csvString), "text/csv", fileName);

          // var renderer = new HtmlToPdf();
           // renderer.RenderHtmlAsPdf("<h1>This is test file</h1>").SaveAs("Example.pdf");

           
        }

        [HttpGet]
        public async Task<IActionResult> getCustomerBal(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                accno = HttpUtility.HtmlEncode(accno);

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

                // var data = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);//.AccountSchemRepo.GetCustomerBal(accno, FromDate, ToDate);

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
                string msg = string.Empty;
                string sb = await GetHTMLString(accno, FromDate, ToDate);
                ViewBag.ReportData = sb.ToString();

                accno = HttpUtility.HtmlEncode(accno);

                var data = await _unitOfWork.TransactionDetailsRepo.GetFinTransactionDetails(accno, FromDate, ToDate);
                if (data.Count == 0 || data == null)
                {
                    msg = "<font color='red'><b>No Data Found!</b></font>";
                }
                else if (data.Count == 1 && string.IsNullOrEmpty(data[0].tran_id))
                {
                    msg = "<font color='red'><b>No Data Found!</b></font>";
                    data = new List<FinStatementDetails>();
                }
                else
                {
                    decimal withdraw = 0;
                    decimal deposit = 0;

                    foreach (var mask in data.ToList())
                    {
                        withdraw += (string.IsNullOrEmpty(mask.withdraw) ? 0 : Convert.ToDecimal(mask.withdraw));
                        deposit += (string.IsNullOrEmpty(mask.deposit) ? 0 : Convert.ToDecimal(mask.deposit));
                    }
                    var summary = new FinStatementDetails();                    
                    summary.withdraw = withdraw.ToString();
                    summary.deposit = deposit.ToString();
                    data.Add(summary);
                }

                return Json(new { data = data, status = "success", message = msg, result = CommonAjaxResponse("Success", "Success", "200") });
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
                    d.value_date= string.IsNullOrEmpty(d.value_date)? "" : Convert.ToDateTime(d.value_date).ToString("dd-MMM-yyyy");

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
                    footer.withdraw = withdraw.ToString(".00").PadLeft(20,' ');
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
                    var data1 = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);

                    string name = data1.acct_name;
                    string address = data1.address;
                    string currency = data1.acct_crncy_code;
                    string acctno = data1.foracid;
                    string acctype = data1.account_type;
                    string custid = data1.cust_id;

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
                    
                    string valdate=string.Empty;
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
                    dataTable.Columns.Remove("INSTRMNT_NUM");

                    DataTable dtCloned = dataTable.Clone();
                    dtCloned.Columns[2].DataType = typeof(decimal);
                    dtCloned.Columns[3].DataType = typeof(decimal);
                    dtCloned.Columns[4].DataType = typeof(decimal);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        dtCloned.ImportRow(row);
                    }
                    string rpt_name = "Account Statement Report";
                    byte[] buffer = ExportExcel(dtCloned, rpt_name, accno, FromDate, ToDate, name, address, currency, acctno, acctype, custid);
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
                    workSheet.Cells["A1"].Value = heading;
                    workSheet.Cells["A1"].Style.Font.Size = 11;
                    workSheet.Cells["A1"].Style.Font.Bold = true;
                    
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
                        + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") +" "+"To"+" " + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy");
                    workSheet.Cells["A9"].Style.Font.Size = 11;
                    workSheet.Cells["A9"].Style.Font.Bold = true;

                }
               // workSheet.Cells["A"+(11 + dataTable.Rows.Count + 3)].Value = "This is a computer-generated statement and does not require any signature.";

                result = package.GetAsByteArray();
            }

            return result;
        }

        public async Task<byte[]> ExportPdf(string accno, DateTime fdate, DateTime tdate)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;
            var data = await _unitOfWork.TransactionDetailsRepo.GetFinTransactionDetails(accno, fdate, tdate);
            var data1 = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
            var sb = new StringBuilder();

            sb.AppendFormat("<html><head></head><body> ");

            sb.AppendFormat("<table style='margin-bottom: 17px;'>");

            sb.AppendFormat(@"<tr style ='line-height:16px;' >
                                    <td>
                                        <table>
                                            <tr>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;font-weight: 500;'>{0}</td>
                                            </tr>
                                            <tr>
                                                <td style = 'width: 25%; text-align: left;font-size:12px'> {1} </td>
                                            </tr>
                                        </table>
                                    <td/>
                                    <td>
                                        <table>
                                            <tr>
                                                <th style='font-size:12px;'>Cust ID </th>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;'>{2}</td>
                                            </tr>
                                            <tr>
                                                <th style='font-size:12px;'>Account No </th>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;'>{3}</td>
                                            </tr> 
                                            <tr>
                                                <th style='font-size:12px;'>A/C Type </th>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;'>{4}</td>
                                            </tr> 
                                            <tr>
                                                <th style='font-size:12px;'>Currency </th>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;'>{5}</td>
                                            </tr>
                                           
                                        </table>
                                    </td>
                                </tr>
                                <br/><br/>
                                <tr> 
									<td colspan='2'>
                                        <div class='header' style='font-weight: bold;font-size: 11px;margin-top: 5px;'><h7>STATEMENT OF ACCOUNT FOR THE PERIOD OF {6} To {7}</h7></div>
									</td>
								</tr>", data1.acct_name, data1.address, data1.cust_id, data1.foracid, data1.account_type, data1.acct_crncy_code, fdate.ToString("dd-MMM-yyyy"), tdate.ToString("dd-MMM-yyyy"));

            sb.AppendFormat("<tr><td colspan='3'><table align='center' style='width:700px'>");
            sb.AppendFormat(@"<tr style='line-height:9px;border-bottom: dashed 1px;border-top: dashed 1px;font-size: 12px;' ><th style='text-align:left'>Date</th><th style='text-align:left'>Particular</th><th style='text-align:right'>Witdraw</th><br/><th style='text-align:right'>Deposit</th><th style='text-align:right'>Balance</th></tr>");
            decimal withdraw = 0;
            decimal deposit = 0;
            foreach (var emp in data)
            {
                deposit = deposit + Convert.ToDecimal(emp.deposit);
                withdraw = withdraw + Convert.ToDecimal(emp.withdraw);
                sb.AppendFormat(@"<tr style='font-size:12px;'><td style='text-align:left'>{0}</td><td style='text-align:left'>{1}</td><td style='text-align:right'>{2}</td><td style='text-align:right'>{3}</td><td style='text-align:right'>{4}</td></tr>", string.IsNullOrEmpty(emp.value_date) ? "" : Convert.ToDateTime(emp.value_date).ToString("dd-MMM-yyyy"), emp.tran_particular, emp.withdraw, emp.deposit, emp.balance);
            }
            sb.AppendFormat(@"<tr style='font-size:12px;border-top: dashed 0.5px'><td style='text-align:left'>{0}</td><td style='text-align:left'>{1}</td><td style='text-align:right'>{2}</td><td style='text-align:right'>{3}</td><td style='text-align:right'>{4}</td></tr>", string.Empty, string.Empty, withdraw, deposit, string.Empty);
            sb.AppendFormat(" </table></td></tr>");
            sb.AppendFormat("</table>");
            sb.AppendFormat("<div class='header' style='font-size: 11px;font-weight: 500'><h7>This is a computer-generated statement and does not require any signature.</h7></div><br/>");
            sb.AppendFormat("<div class='header' style='font-weight: 700;text-align: center;font-size: 12px;'><h7>****END OF STATEMENT**** </h7></div>");
            sb.AppendFormat(@"<body></html>");

            // result = package.GetAsByteArray();
            byte[] result = null;
            result = System.Text.Encoding.UTF8.GetBytes(sb.ToString()); //sb.GetAsByteArray();
            return result;
        }
        public async Task<string> GetHTMLString(string accno, DateTime fdate, DateTime tdate)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;
            var data = await _unitOfWork.TransactionDetailsRepo.GetFinTransactionDetails(accno, fdate, tdate);
            var data1 = await _unitOfWork.TransactionDetailsRepo.GetCustInfo(accno);
            var sb = new StringBuilder();

            sb.AppendFormat("<html><head></head><body> ");

            sb.AppendFormat("<table style='margin-bottom: 17px;'>");

            sb.AppendFormat(@"<tr style ='line-height:16px;' >
                                    <td>
                                        <table>
                                            <tr>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;font-weight: 500;'>{0}</td>
                                            </tr>
                                            <tr>
                                                <td style = 'width: 25%; text-align: left;font-size:12px'> {1} </td>
                                            </tr>
                                        </table>
                                    <td/>
                                    <td>
                                        <table>
                                            <tr>
                                                <th style='font-size:12px;'>Cust ID </th>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;'>{2}</td>
                                            </tr>
                                            <tr>
                                                <th style='font-size:12px;'>Account No </th>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;'>{3}</td>
                                            </tr> 
                                            <tr>
                                                <th style='font-size:12px;'>A/C Type </th>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;'>{4}</td>
                                            </tr> 
                                            <tr>
                                                <th style='font-size:12px;'>Currency </th>
                                                <td style = 'width: 25%; text-align: left;font-size:12px;'>{5}</td>
                                            </tr>
                                           
                                        </table>
                                    </td>
                                </tr>
                                <br/><br/>
                                <tr> 
									<td colspan='2'>
                                        <div class='header' style='font-weight: bold;font-size: 11px;margin-top: 5px;'><h7>STATEMENT OF ACCOUNT FOR THE PERIOD OF {6} To {7}</h7></div>
									</td>
								</tr>", data1.acct_name, data1.address, data1.cust_id, data1.foracid, data1.account_type, data1.acct_crncy_code, fdate.ToString("dd-MMM-yyyy"), tdate.ToString("dd-MMM-yyyy"));

            sb.AppendFormat("<tr><td colspan='3'><table align='center' style='width:700px'>");
            sb.AppendFormat(@"<tr style='line-height:9px;border-bottom: dashed 1px;border-top: dashed 1px;font-size: 12px;' ><th style='text-align:left'>Date</th><th style='text-align:left'>Particular</th><th style='text-align:right'>Witdraw</th><br/><th style='text-align:right'>Deposit</th><th style='text-align:right'>Balance</th></tr>");
            decimal withdraw = 0;
            decimal deposit = 0;
            foreach (var emp in data)
            {
                deposit = deposit + Convert.ToDecimal(emp.deposit);
                withdraw = withdraw + Convert.ToDecimal(emp.withdraw);
                sb.AppendFormat(@"<tr style='font-size:12px;'><td style='text-align:left'>{0}</td><td style='text-align:left'>{1}</td><td style='text-align:right'>{2}</td><td style='text-align:right'>{3}</td><td style='text-align:right'>{4}</td></tr>", string.IsNullOrEmpty(emp.value_date) ? "" : Convert.ToDateTime(emp.value_date).ToString("dd-MMM-yyyy"), emp.tran_particular, emp.withdraw, emp.deposit, emp.balance);
            }
            sb.AppendFormat(@"<tr style='font-size:12px;border-top: dashed 0.5px'><td style='text-align:left'>{0}</td><td style='text-align:left'>{1}</td><td style='text-align:right'>{2}</td><td style='text-align:right'>{3}</td><td style='text-align:right'>{4}</td></tr>", string.Empty, string.Empty
                , withdraw.ToString("#,#.##", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                , deposit.ToString("#,#.##", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")), string.Empty);
            sb.AppendFormat(" </table></td></tr>");
            sb.AppendFormat("</table>");
            sb.AppendFormat("<div class='header' style='font-size: 11px;font-weight: 500'><h7>This is a computer-generated statement and does not require any signature.</h7></div><br/>");
            sb.AppendFormat("<div class='header' style='font-weight: 700;text-align: center;font-size: 12px;'><h7>****END OF STATEMENT**** </h7></div>");
            sb.AppendFormat(@"<body></html>");
            return sb.ToString();
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
                var reportHeader = "STATEMENT OF ACCOUNT FOR THE PERIOD OF " + FromDate.ToString("dd-MMM-yyyy") + " To "+ToDate.ToString("dd-MMM-yyyy")+"";

                return Json(new { data = data, customer = customer, header = reportHeader, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });


            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }
    }
}
