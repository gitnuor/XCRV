using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Common;
using System.Data;
using System.Collections.Generic;
using OfficeOpenXml.Style;
using System.Reflection;
using System.Web;

namespace XCRV.Web.Controllers
{
    public class TransactionDetailsController : Controller
    {
        private readonly ILogger<TransactionDetailsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
       // public static string GetMimeMapping(string fileName);
        public TransactionDetailsController(ILogger<TransactionDetailsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
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
        public async Task <IActionResult>  Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getTransactionDetails(string seachString, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if(!string.IsNullOrEmpty(seachString))
                {
                    seachString = HttpUtility.HtmlEncode(seachString);
                }
                string msg = string.Empty;
                string fdate = FromDate.ToString();
                string tdate = ToDate.ToString();
                var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                string controllerName = "TransactionDetails";

                IList<TransactionDetails> data = new List<TransactionDetails>();
                if (seachString == null)
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
                else {
                    string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(seachString.Trim());
                    if (((schemeCode == "SRSTF" && IsStatementTrue == "N")
                    || (!await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(seachString.Trim(), userName))))
                    {
                        msg = "You are not authorized to view this information!!!";
                    }
                    else {
                        data = await _unitOfWork.TransactionDetailsRepo.GetTransactionDetails(seachString, FromDate, ToDate);
                        if (data.Count == 0 || data == null)
                        {
                            msg = "<font color='red'><b>No Data Found!</b></font>";
                        }
                        
                        _logger.LogInformation("successfully generated. Accno:{seachString},FromDate:{fdate},ToDate:{tdate},User:{userName},Contoller:{controllerName}", seachString, fdate, tdate, userName, controllerName);
                    }
                }
                
                return Json(new { data = data, status = "success", message = msg, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error");
                return Json(new { status = "error", result = CommonAjaxResponse("eror", ex.Message, "000") });
               
            }
        }


        [HttpPost]
        public async Task<IActionResult> ExportMyData(string seachString, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(seachString))
                {
                  seachString = HttpUtility.HtmlEncode(seachString);
                }
                var claims = User.Claims;
               
                var  data = await _unitOfWork.TransactionDetailsRepo.GetTransactionDetails(seachString, FromDate, ToDate);
                if (data.Count == 0 || data == null)
                { 
                    return NotFound();      
                }
                else
                {
                    DataTable dataTable = new DataTable(typeof(TransactionDetails).Name);

                    //Get all the properties
                    PropertyInfo[] Props = typeof(TransactionDetails).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo prop in Props)
                    {
                        //Defining type of data column gives proper data table 
                        var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                        //Setting column names as Property names
                        dataTable.Columns.Add(prop.Name.ToUpper(), type);
                    }
                    foreach (TransactionDetails item in data)
                    {
                        var values = new object[Props.Length];
                        for (int i = 0; i < Props.Length; i++)
                        {
                            //inserting property values to datatable rows
                            values[i] = Props[i].GetValue(item, null);
                        }
                        dataTable.Rows.Add(values);
                    }
                    dataTable.Columns.Remove("SEACHSTRING");
                    dataTable.Columns.Remove("FROMDATE");
                    dataTable.Columns.Remove("TODATE");
                    string rpt_name = "Transaction details Report";
                    byte[] filecontent = ExportExcel(dataTable, rpt_name);
                    var ms = new MemoryStream(filecontent);

                    var contentType = "application/vnd.ms-excel";
                    return File(ms, contentType.ToString(), "TransactionDetails.xlsx");
                }
                

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
