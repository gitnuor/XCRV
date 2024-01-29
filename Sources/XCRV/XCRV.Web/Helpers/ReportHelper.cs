using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XCRV.Web.Helpers
{
    public class ReportHelper<T> where T:new()
    {
        public static string ConvertExcel(IList<T> oDataTable)
        {
            System.Text.StringBuilder oStringBuilder = new System.Text.StringBuilder();

            try
            {
                int borderWidth = 0;
                if (_ShowExcelTableBorder)
                {
                    borderWidth = 1;
                }

                string boldTagStart = "";
                string boldTagEnd = "";
                if (_ExcelHeaderBold)
                {
                    boldTagStart = "<B>";
                    boldTagEnd = "</B>";
                }

                DateTime print_date = new DateTime();
                print_date = DateTime.Today;

                string _print_date = print_date.ToString("dd-MMM-yyyy");

                oStringBuilder.Append(@"<style>.text { mso-number-format:\@; } </style>");

                oStringBuilder.Append("<Table border=" + borderWidth + ">");

                oStringBuilder.Append("<TR align='center' style='width:100%;'>");

                foreach (var prop in typeof(T).GetProperties())
                {
                    oStringBuilder.Append("<TD>" + boldTagStart + prop.Name.ToUpper() + boldTagEnd + "</TD>");

                }

                oStringBuilder.Append("</TR>");

                string value = string.Empty;
                foreach (var oDataRow in oDataTable)
                {
                    oStringBuilder.Append("<TR>");
                    foreach (var oDataColumn in oDataRow.GetType().GetProperties())
                    {
                        value = string.Empty;
                        object valueObj = oDataColumn.GetValue(oDataRow, null);
                        if (valueObj != null && !string.IsNullOrEmpty(valueObj.ToString()))
                        {
                            value = valueObj.ToString();
                        }
                        oStringBuilder.Append("<TD class='text'>" + value + "</TD>");
                    }
                    oStringBuilder.Append("</TR>");
                }

                oStringBuilder.Append("</Table>");
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return oStringBuilder.ToString();
        }

        private static bool _ShowExcelTableBorder = true;

        public bool ShowExcelTableBorder
        {
            get
            {
                return _ShowExcelTableBorder;
            }
            set
            {
                _ShowExcelTableBorder = value;
            }
        }

        private static bool _ExcelHeaderBold = true;

        public bool ExcelHeaderBold
        {
            get
            {
                return _ExcelHeaderBold;
            }
            set
            {
                ExcelHeaderBold = value;
            }
        }
    }
}
