using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Common;

namespace XCRV.Web.Controllers
{
    //[Filters.AuthorizeActionFilter]
    public class Account_ADC_TransactionController : Controller
    {
        private readonly ILogger<CustomerLimitController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public Account_ADC_TransactionController(ILogger<CustomerLimitController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public AJaxResponse CommonAjaxResponse(string messageType, string message, string responseCode)
        {
            using (var response = new AJaxResponse())
            {
                response._AjaxCode = responseCode;
                response._DisplayMessage = message;
                response._DisplayMessageType = messageType.ToString();
                return response;
            }
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getCustomerBal(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                //string extensiveType = string.Empty;

                //var claims = User.Claims;
                //string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();

                accno = HttpUtility.HtmlEncode(accno);
                

                var data = await _unitOfWork.AccountSchemRepo.GetCustomerBal(accno, FromDate, ToDate);//.CustomerLimitRepo.GetCustomerLimitByCustid(Custid.Trim());

                return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }


        [HttpGet]
        public async Task<IActionResult> getADCTransactionDetails(string accno, DateTime FromDate, DateTime ToDate)
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
                    else {
                        data = await _unitOfWork.TransactionDetailsRepo.GetADCTransactionDetails(accno, FromDate, ToDate);//.GetTransactionDetails(seachString, FromDate, ToDate);//CustomerLimitRepo.getCustomerLimit(seachString, isStatementTrue);
                        if (data.Count == 0 || data == null)
                        {
                            msg = "<font color='red'><b>No Data Found!</b></font>";
                        }
                        else if (data.Count == 1 && string.IsNullOrEmpty(data[0].tran_id))
                        {
                            msg = "<font color='red'><b>No Data Found!</b></font>";
                            data = new List<AccountADCTransaction>();
                        }
                        else {
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

        [HttpPost]
        public async Task<IActionResult> ExportData(string accno, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string extensiveType = string.Empty;

                accno = HttpUtility.HtmlEncode(accno);

                var claims = User.Claims;
                string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();

                var data = await _unitOfWork.TransactionDetailsRepo.GetADCTransactionDetails(accno, FromDate, ToDate);//CustomerLimitRepo.getCustomerLimit(seachString, isStatementTrue);


                DataTable dataTable = new DataTable(typeof(AccountADCTransaction).Name);

                //Get all the properties
                PropertyInfo[] Props = typeof(AccountADCTransaction).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Defining type of data column gives proper data table 
                    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name.ToUpper(), type);
                }
                foreach (AccountADCTransaction item in data)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                    
                }

                string rpt_name = "ADC Transaction Report";
                byte[] filecontent = ExportExcel(dataTable, rpt_name);
                var ms = new MemoryStream(filecontent);

                var contentType = "application/vnd.ms-excel";
                return File(ms, contentType.ToString(), "ADCTransaction.xlsx");

                // return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
            }
        }

        public static byte[] ExportExcel(DataTable dataTable, string heading)
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
                int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 3;

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
                    workSheet.Cells["A1"].Style.Font.Size = 12;
                    workSheet.Cells["A1"].Style.Font.Bold = true;

                    //workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 5;


                    workSheet.Cells["A1"].Value = heading;
                    workSheet.Cells["A1"].Style.Font.Size = 12;
                    workSheet.Cells["A1"].Style.Font.Bold = true;





                }

                result = package.GetAsByteArray();
            }

            return result;
        }

    }
}
