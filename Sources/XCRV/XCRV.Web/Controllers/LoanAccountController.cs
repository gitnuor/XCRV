using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Models;
using XCRV.Web.Helpers;
using System.Web;

namespace XCRV.Web.Controllers
{
    public class LoanAccountController : BaseController
    {
        private readonly ILogger<LoanAccountController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _webHostEnvironment;

        public LoanAccountController(ILogger<LoanAccountController> logger, IUnitOfWork unitOfWork, Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Loan Account

        [NonAction]
        private async Task<IList<LoanPayoffInfo>> GetLoanPayload(string accountNo, string esFee, DateTime payoffDate)
        {
            IList<LoanPayoffInfo> loanPayoffInfos = new List<LoanPayoffInfo>();
            var data = await _unitOfWork.LoanAccountRepo.GetLoanPayOff(accountNo, payoffDate.ToString("dd MMM yyyy"));

            if (data.Count() > 0)
            {
                string valueHeader = "Entity Value";
                string currentHeader = "";
                string futureHeader = "";
                if (payoffDate != null && payoffDate != DateTime.MinValue)
                {
                    currentHeader = valueHeader + " as on " + DateTime.Now.Date.ToString("dd MMM yyyy");
                    futureHeader = valueHeader + " as on " + payoffDate.Date.ToString("dd MMM yyyy");
                }
                if (!string.IsNullOrEmpty(esFee))
                {
                    futureHeader = futureHeader + " (ES FEE: " + esFee + "%)";
                }

                ViewBag.currentHeader = currentHeader;
                ViewBag.futureHeader = futureHeader;


                var payoffRow = data.FirstOrDefault(p => p.Fu_Entity_Name.Contains("FIN PAYOFF AMT"));

              decimal finPayoffAmount = 0;
                if (payoffRow != null)
                {
                    finPayoffAmount = !string.IsNullOrEmpty(payoffRow.Fu_Val) ? Convert.ToDecimal(Convert.ToDecimal(payoffRow.Fu_Val)) : 0;
                    //if (finPayoffAmount < 0)
                    //finPayoffAmount = finPayoffAmount * (-1);
                }

                string esFeesEntityName;
                decimal esFees = 0;
                decimal esVAT = 0;
                decimal exciseBuff = 0;
                decimal exciseCurr = 0;
                decimal netPayoff = 0;

                LoanPayoffInfo loanPayoffInfo = new LoanPayoffInfo();




                foreach (var row in data)
                {
                    if (row.Fu_Entity_Name.Contains("ES FEE"))
                    {
                        if (!string.IsNullOrEmpty(esFee))
                        {
                            esFeesEntityName = "ES FEE " + esFee + "%";
                            esFees = (finPayoffAmount * Convert.ToDecimal(esFee)) / 100;
                        }
                        else
                        {
                            esFeesEntityName = row.Fu_Entity_Name;
                            esFees = Convert.ToDecimal(row.Fu_Val);
                        }

                        loanPayoffInfo = new LoanPayoffInfo();
                        loanPayoffInfo.Cur_Entity_Name = row.Cur_Entity_Name;
                        loanPayoffInfo.Cur_Val = row.Cur_Val;
                        loanPayoffInfo.Fu_Entity_Name = esFeesEntityName;

                        CultureInfo cultureInfo = new CultureInfo("bn-BD");
                        cultureInfo.NumberFormat.CurrencySymbol = "";

                        string esFeesStr = String.Format(cultureInfo, "{0:C2}", Convert.ToDouble(esFees.ToString("F")));
                        loanPayoffInfo.Fu_Val = esFeesStr;
                        loanPayoffInfos.Add(loanPayoffInfo);
                    }
                    else if (row.Fu_Entity_Name.Contains("NET PAYOFF AMT"))
                    {
                        var esVATRow = data.FirstOrDefault(p => p.Fu_Entity_Name.Contains("ES VAT %"));
                        if (esVATRow != null)
                        {
                            esVAT = !string.IsNullOrEmpty(esVATRow.Fu_Val) ? Convert.ToDecimal(Convert.ToDecimal(esVATRow.Fu_Val)) : 0;
                        }


                        var exciseBuffRow = data.FirstOrDefault(p => p.Fu_Entity_Name.Contains("EXCISE BUFF"));
                        if (exciseBuffRow != null)
                            exciseBuff = !string.IsNullOrEmpty(exciseBuffRow.Fu_Val.ToString()) ? Convert.ToDecimal(Convert.ToDecimal(exciseBuffRow.Fu_Val.Trim())) : 0;

                        var exciseCurrRow = data.FirstOrDefault(p => p.Fu_Entity_Name.Contains("EXCISE CURR"));
                        if (exciseCurrRow != null)
                            exciseCurr = !string.IsNullOrEmpty(exciseCurrRow.Fu_Val) ? Convert.ToDecimal(Convert.ToDecimal(exciseCurrRow.Fu_Val.Trim())) : 0;

                        netPayoff = finPayoffAmount + esFees + esVAT + exciseBuff + exciseCurr;

                        loanPayoffInfo = new LoanPayoffInfo();
                        loanPayoffInfo.Cur_Entity_Name = row.Cur_Entity_Name;
                        loanPayoffInfo.Cur_Val = row.Cur_Val;
                        loanPayoffInfo.Fu_Entity_Name = row.Fu_Entity_Name;


                        CultureInfo cultureInfo = new CultureInfo("bn-BD");
                        cultureInfo.NumberFormat.CurrencySymbol = "";

                        string netPayoffStr = String.Format(cultureInfo, "{0:C2}", Convert.ToDouble(netPayoff.ToString("F")));
                        loanPayoffInfo.Fu_Val = netPayoffStr;
                        loanPayoffInfos.Add(loanPayoffInfo);
                    }
                    else
                    {
                        loanPayoffInfo = new LoanPayoffInfo();
                        loanPayoffInfo.Cur_Entity_Name = row.Cur_Entity_Name;
                        loanPayoffInfo.Cur_Val = row.Cur_Val;
                        loanPayoffInfo.Fu_Entity_Name = row.Fu_Entity_Name;
                        loanPayoffInfo.Fu_Val = row.Fu_Val;
                        loanPayoffInfos.Add(loanPayoffInfo);
                    }
                }
            }

            return loanPayoffInfos;

        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string accountNo)
        {
            ViewBag.Title = "Loan Account Info";
            ViewBag.AccountNo = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);

            LoanAccountViewModel accountInforation = new LoanAccountViewModel();
            if (!string.IsNullOrEmpty(accountNo))
            {
                if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo.Trim()))
                {
                    TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                }                
                else
                {
                    if (IsNumberString(accountNo))
                    {
                        int accVal;
                        if(accountNo.Trim().Length == 16)
                        {
                            accVal = Convert.ToInt32(accountNo.Substring(4, 1));
                        }
                        else
                        {
                            accVal = Convert.ToInt32(accountNo.Substring(0, 1));
                        }
                        

                        if (accVal != 6)
                        {
                            TempData["ErrorMessage"] = "Sorry!!! This is not a Loan Account!!!";
                        }
                        else
                        {
                            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
                            if (!IsAccessable)
                            {
                                TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";
                            }
                            else
                            {
                                accountInforation.LoanAccountInfo = (await _unitOfWork.LoanAccountRepo.GetLoanAccountInfoByAcno(accountNo)).FirstOrDefault();

                                if (accountInforation.LoanAccountInfo == null || accountInforation.LoanAccountInfo.Cust_Id == null)
                                {
                                    TempData["ErrorMessage"] = "Sorry!!! Loan Account not found!!!";
                                }
                                else
                                {
                                    accountInforation.LoanAccountDetails = (await _unitOfWork.LoanAccountRepo.GetLoanAccountDetailsInfoByAcno(accountNo));
                                }
                            }
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }
                }
            }


            if (accountInforation.LoanAccountDetails == null)
            {
                accountInforation.LoanAccountDetails = new List<LoanAccountDetails>();
            }
            if (accountInforation.LoanAccountInfo == null)
            {
                accountInforation.LoanAccountInfo = new LoanAccountInfo();
            }
            
            return View(accountInforation);
        }

        public async Task<ActionResult> ShowLoanPaymentHistory(string accountNo)
        {
            try
            {
                accountNo = HttpUtility.HtmlEncode(accountNo);

                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
                if (!IsAccessable)
                {
                    //TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";

                    return Content("<h3>Sorry!!! You are not authorized to view this information!!!</h3>");
                }
                else
                {
                    var model = await _unitOfWork.LoanAccountRepo.GetRepaymentHistoryByAcno(accountNo);
                    return PartialView("_LoanPaymentHistory", model);
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> LoanPayoff(string accountNo, string payOffDate, string esFee)
        {
            IList<LoanPayoffInfo> loanPayoffInfos = new List<LoanPayoffInfo>();
            ViewBag.currentHeader = "Entity Value";
            ViewBag.futureHeader = "Future Entity Value";

            ViewBag.accountNo = accountNo;
            ViewBag.payOffDate = payOffDate;
            ViewBag.esFee = esFee;

            accountNo = HttpUtility.HtmlEncode(accountNo);
            payOffDate = HttpUtility.HtmlEncode(payOffDate);
            esFee = HttpUtility.HtmlEncode(esFee);

            if (string.IsNullOrEmpty(accountNo))
            {
                return View(loanPayoffInfos);
            }

            if (!IsNumberString(accountNo))
            {
                TempData["ErrorMessage"] = "Sorry!!! Account Number must be numeric!!!";
                return View(loanPayoffInfos);
            }

            if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo.Trim()))
            {
                TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                return View(loanPayoffInfos);
            }

            string accVal;

            if(accountNo.Trim().Length == 16)
            {
                accVal = accountNo.Substring(4, 1);
            }
            else
            {
                accVal = accountNo.Substring(0, 1);
            }
            
            if (accVal != "6")
            {
                TempData["ErrorMessage"] = "Sorry!!! This is not a Loan Account!!!";
                return View(loanPayoffInfos);
            }

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";
                return View(loanPayoffInfos);
            }

            if (!string.IsNullOrEmpty(esFee) && Convert.ToDecimal(esFee) > 100)
            {
                TempData["ErrorMessage"] = "Sorry!!! Interest rate should be maximum 100!!!";
                return View(loanPayoffInfos);
            }

            DateTime payoffDate = new DateTime();
            if (!string.IsNullOrEmpty(payOffDate))
            {
                DateTime.TryParse(payOffDate, out payoffDate);
                if (payoffDate.Date < DateTime.Now.Date)
                {
                    TempData["ErrorMessage"] = "Sorry!!! Payoff date should be current or future date!!!";
                    return View(loanPayoffInfos);
                }
            }
            else
            {
                payoffDate = DateTime.Now;
                ViewBag.payOffDate = payoffDate.ToString("dd-MMM-yyyy");
            }
            loanPayoffInfos = await GetLoanPayload(accountNo, esFee, payoffDate);

            if (loanPayoffInfos.Count == 0)
            {
                TempData["ErrorMessage"] = "Sorry!!! No data found!!!";
            }
            return View(loanPayoffInfos);
        }

        public async Task<IActionResult> ShowLoanPayoff(string accountNo, string payOffDate, string esFee)
        {
            IList<LoanPayoffInfo> loanPayoffInfos = new List<LoanPayoffInfo>();
            ViewBag.currentHeader = "Entity Value";
            ViewBag.futureHeader = "Future Entity Value";

            ViewBag.accountNo = accountNo;
            ViewBag.payOffDate = payOffDate;
            ViewBag.esFee = esFee;

            accountNo = HttpUtility.HtmlEncode(accountNo);
            payOffDate = HttpUtility.HtmlEncode(payOffDate);
            esFee = HttpUtility.HtmlEncode(esFee);

            if (string.IsNullOrEmpty(accountNo))
            {
                return Content("<h3>Sorry!!! Account Number is Required!!!</h3>");
            }

            if (!IsNumberString(accountNo))
            {                
                return Content("<h3>Sorry!!! Account Number must be numeric!!!</h3>");
            }

            string accVal;
            if(accountNo.Trim().Length == 16)
            {
                accVal = accountNo.Substring(4, 1);
            }
            else
            {
                accVal = accountNo.Substring(0, 1);
            }
            
            if (accVal != "6")
            {                
                return Content("<h3>Sorry!!! This is not a Loan Account!!!</h3>");
            }

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                return Content("<h3>Sorry!!! You are not authorized to view this information!!!</h3>");
            }

            if (!string.IsNullOrEmpty(esFee) && Convert.ToDecimal(esFee) > 100)
            {
                return Content("<h3>Sorry!!! Interest rate should be maximum 100!!!</h3>");
            }

            DateTime payoffDate = new DateTime();
            if (!string.IsNullOrEmpty(payOffDate))
            {
                DateTime.TryParse(payOffDate, out payoffDate);
                if (payoffDate.Date < DateTime.Now.Date)
                {                    
                    return Content("<h3>Sorry!!! Payoff date should be current or future date!!!</h3>");
                }
            }
            else
            {
                payoffDate = DateTime.Now;
                ViewBag.payOffDate = payoffDate.ToString("dd-MMM-yyyy");
            }

            loanPayoffInfos = await GetLoanPayload(accountNo, esFee, payoffDate);

            if (loanPayoffInfos.Count == 0)
            {
                return Content("<h3>Sorry!!! No data found!!!</h3>");
            }

            return PartialView("_LoanPayoffDetails",loanPayoffInfos);
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> LoanGuarantor(string accountNo)
        {
            IEnumerable<Guarantor> guarantors = new List<Guarantor>();

            ViewBag.AccountNo = accountNo;
            accountNo = HttpUtility.HtmlEncode(accountNo);

            if (string.IsNullOrEmpty(accountNo))
            {
                return View(guarantors);
            }

            if (!IsNumberString(accountNo))
            {
                TempData["ErrorMessage"] = "Sorry!!! Account Number must be numeric!!!";
                return View(guarantors);
            }

            if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo.Trim()))
            {
                TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                return View(guarantors);
            }

            string accVal;
            if(accountNo.Trim().Length == 16)
            {
                accVal = accountNo.Substring(4, 1);
            }
            else
            {
                accVal = accountNo.Substring(0, 1);
            }
            
            if (accVal != "6")
            {
                TempData["ErrorMessage"] = "Sorry!!! This is not a Loan Account!!!";
                return View(guarantors);
            }

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";
                return View(guarantors);
            }


            guarantors = await _unitOfWork.LoanAccountRepo.GetLoanGuarantorByLoanNumber(accountNo);


            guarantors.ToList().ForEach(i =>
            {
                i.address = i.addr1??"" + "<br/>"
                        + (!string.IsNullOrEmpty(i.addr2.Trim('.')) ? i.addr2??"" + "<br/>" : "")
                        + (!string.IsNullOrEmpty(i.city)  ? "City: " + i.city??"" : "")
                        + (!string.IsNullOrEmpty(i.state)  ? " State: " + i.state??"" : "")
                        + (!string.IsNullOrEmpty(i.post_code)  ? " Post Code: " + i.post_code??"" : "")
                        + (!string.IsNullOrEmpty(i.cntry)  ? "<br/>Country: " + i.cntry??"" : "")
                        + (!string.IsNullOrEmpty(i.telephone)  ? "<br/>Telephone: " + i.telephone??"" : "")
                        + (!string.IsNullOrEmpty(i.telexno)  ? "<br/>Tellex: " + i.telexno??"" : "")
                        + (!string.IsNullOrEmpty(i.fax)  ? "<br/>Fax: " + i.fax??"" : "");
            });



            if (guarantors.Count() == 0)
            {
                TempData["ErrorMessage"] = "Sorry!!! No data found!!!";

                ViewBag.PrincipalCustomerId = "";
                ViewBag.PrincipleCustomerName = "";
                ViewBag.SchmCode = "";
                ViewBag.SchmDesc = "";
            }
            else
            {
                ViewBag.PrincipalCustomerId = "<i class='fa fa-user'></i><a href='/CustomerGeneralDetails/Index?customerId="+ guarantors.First().cust_id + "' target='_blank'>" + guarantors.First().cust_id + "</a>" ;
                ViewBag.PrincipleCustomerName = guarantors.First().cust_name;
                ViewBag.SchmCode = guarantors.First().schm_code;
                ViewBag.SchmDesc = guarantors.First().schm_desc;

            }

            return View(guarantors);
        }

        public async Task<IActionResult> ShowLoanGuarantor(string accountNo)
        {
            IEnumerable<Guarantor> guarantors = new List<Guarantor>();

            accountNo = HttpUtility.HtmlEncode(accountNo);

            if (string.IsNullOrEmpty(accountNo))
            {
                return Content("<h3>Sorry!!! Account Number is Required!!!</h3>");
            }

            if (!IsNumberString(accountNo))
            {
                return Content("<h3>Sorry!!! Account Number must be numeric!!!</h3>");
            }

            string accVal;
            if(accountNo.Trim().Length == 16)
            {
                accVal = accountNo.Substring(4, 1);
            }
            else
            {
                accVal = accountNo.Substring(0, 1);
            }
            
            if (accVal != "6")
            {
                return Content("<h3>Sorry!!! This is not a Loan Account!!!</h3>");
            }

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                return Content("<h3>Sorry!!! You are not authorized to view this information!!!</h3>");
            }


            guarantors = await _unitOfWork.LoanAccountRepo.GetLoanGuarantorByLoanNumber(accountNo);


            guarantors.ToList().ForEach(i =>
            {
                i.address = i.addr1??"" + "<br/>"
                        + (!string.IsNullOrEmpty(i.addr2.Trim('.')) ? i.addr2??"" + "<br/>" : "")
                        + (!string.IsNullOrEmpty(i.city) ? "City: " + i.city??"" : "")
                        + (!string.IsNullOrEmpty(i.state) ? " State: " + i.state??"" : "")
                        + (!string.IsNullOrEmpty(i.post_code) ? " Post Code: " + i.post_code??"" : "")
                        + (!string.IsNullOrEmpty(i.cntry) ? "<br/>Country: " + i.cntry??"" : "")
                        + (!string.IsNullOrEmpty(i.telephone) ? "<br/>Telephone: " + i.telephone??"" : "")
                        + (!string.IsNullOrEmpty(i.telexno) ? "<br/>Tellex: " + i.telexno??"" : "")
                        + (!string.IsNullOrEmpty(i.fax) ? "<br/>Fax: " + i.fax??"" : "");
            });



            if (guarantors.Count() == 0)
            {
                ViewBag.PrincipalCustomerId = "";
                ViewBag.PrincipleCustomerName = "";
                ViewBag.SchmCode = "";
                ViewBag.SchmDesc = "";

                return Content("<h3>Sorry!!! No data found!!!</h3>");
            }
            else
            {
                ViewBag.PrincipalCustomerId = "<i class='fa fa-user'></i><a href='/CustomerGeneralDetails/Index?customerId=" + guarantors.First().cust_id + "' target='_blank'>" + guarantors.First().cust_id + "</a>";
                ViewBag.PrincipleCustomerName = guarantors.First().cust_name;
                ViewBag.SchmCode = guarantors.First().schm_code;
                ViewBag.SchmDesc = guarantors.First().schm_desc;
            }
            return PartialView("_LoanGuarantorInfo", guarantors);
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> LoanDocument(string accountNo)
        {
            IEnumerable<LoanDocument> loanDocuments = new List<LoanDocument>();

            ViewBag.AccountNo = accountNo;
            accountNo = HttpUtility.HtmlEncode(accountNo);
            if (string.IsNullOrEmpty(accountNo))
            {
                return View(loanDocuments);
            }

            if (!IsNumberString(accountNo))
            {
                TempData["ErrorMessage"] = "Sorry!!! Account Number must be numeric!!!";
                return View(loanDocuments);
            }

            if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo.Trim()))
            {
                TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                return View(loanDocuments);
            }


            string accVal;
            if(accountNo.Trim().Length == 16)
            {
                accVal = accountNo.Substring(4, 1);
            }
            else
            {
                accVal = accountNo.Substring(0, 1);
            }
            
            if (accVal != "6")
            {
                TempData["ErrorMessage"] = "Sorry!!! This is not a Loan Account!!!";
                return View(loanDocuments);
            }

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";
                return View(loanDocuments);
            }
            
            loanDocuments = await _unitOfWork.LoanAccountRepo.GetLoanDocumentsByLoanNumber(accountNo);
            if (loanDocuments.Count() == 0)
            {
                TempData["ErrorMessage"] = "Sorry!!! No data found!!!";
            }
            

            return View(loanDocuments);
        }

        public async Task<IActionResult> ShowLoanDocument(string accountNo)
        {
            IEnumerable<LoanDocument> documents = new List<LoanDocument>();

            accountNo = HttpUtility.HtmlEncode(accountNo);

            if (string.IsNullOrEmpty(accountNo))
            {
                return Content("<h3>Sorry!!! Account Number is Required!!!</h3>");
            }

            if (!IsNumberString(accountNo))
            {
                return Content("<h3>Sorry!!! Account Number must be numeric!!!</h3>");
            }

            string accVal;

            if(accountNo.Trim().Length == 16)
            {
                accVal = accountNo.Substring(4, 1);
            }
            else
            {
                accVal = accountNo.Substring(0, 1);
            }
            
            if (accVal != "6")
            {
                return Content("<h3>Sorry!!! This is not a Loan Account!!!</h3>");
            }

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                return Content("<h3>Sorry!!! You are not authorized to view this information!!!</h3>");
            }
            documents = await _unitOfWork.LoanAccountRepo.GetLoanDocumentsByLoanNumber(accountNo);
            if (documents.Count() == 0)
            {               
                return Content("<h3>Sorry!!! No data found!!!</h3>");
            }
            return PartialView("_LoanDocumentList", documents);
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> LoanAccountLimit(string accountNo)
        {
            LoanAccountLimit loanAccountLimit = new LoanAccountLimit();

            ViewBag.AccountNo = accountNo;

            accountNo = HttpUtility.HtmlEncode(accountNo);

            if (string.IsNullOrEmpty(accountNo))
            {
                return View(loanAccountLimit);
            }

            if (!IsNumberString(accountNo))
            {
                TempData["ErrorMessage"] = "Sorry!!! Account Number must be numeric!!!";
                return View(loanAccountLimit);
            }

            if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo.Trim()))
            {
                TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                return View(loanAccountLimit);
            }

            string accVal;

            if(accountNo.Trim().Length == 16)
            {
                accVal = accountNo.Substring(4, 1);
            }
            else
            {
                accVal = accountNo.Substring(0, 1);
            }

            
            if (accVal != "6")
            {
                TempData["ErrorMessage"] = "Sorry!!! This is not a Loan Account!!!";
                return View(loanAccountLimit);
            }

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";
                return View(loanAccountLimit);
            }

            loanAccountLimit = (await _unitOfWork.LoanAccountRepo.GetLoanAccountLimitByLoanNumber(accountNo)).FirstOrDefault();

            if (loanAccountLimit == null)
            {
                TempData["ErrorMessage"] = "Sorry!!! No data found!!!";
            }
            else
            {
                loanAccountLimit.Foracid = loanAccountLimit.Foracid + " " + loanAccountLimit.Acct_Crncy_Code + " / " + loanAccountLimit.Sol_Id;
            }

            if(loanAccountLimit == null)
            {
                loanAccountLimit = new LoanAccountLimit();
            }

            return View(loanAccountLimit);
        }

        #endregion

        #region Digital Loan Application 

        [NonAction]
        private async Task<StringBuilder> GeneratePrintableApplicationFormLiquidHtml(string customerId)
        {
            CorporateCustomer CustomerBasicInfo = new CorporateCustomer();
            IList<LoanAccountInfo> LoanAccountInfoList = new List<LoanAccountInfo>();
            IList<Proprietor> LoanProprietors = new List<Proprietor>();
            IList<Guarantor> Guarantors = new List<Guarantor>();

            string Bb_Sector_Code = string.Empty;

            if (!string.IsNullOrEmpty(customerId))
            {

                CustomerBasicInfo = (await _unitOfWork.LoanAccountRepo.GetLoanCustomerInfo(customerId)).FirstOrDefault();

                if (CustomerBasicInfo == null || CustomerBasicInfo.Cif == null)
                {
                    TempData["ErrorMessage"] = "Sorry!!! No Data found. Please provide a valid CIF No!!!";
                }
                else
                {
                    LoanProprietors = (await _unitOfWork.LoanAccountRepo.GetLoanProprietorInfo(customerId)).ToList();

                    LoanAccountInfoList = (await _unitOfWork.LoanAccountRepo.GetLoanAccountInfoByCif(customerId)).ToList();

                    if (LoanAccountInfoList.Count > 0)
                    {
                        Guarantors = (await _unitOfWork.LoanAccountRepo.GetLoanAccountGuarantorInfoByLoanNo(LoanAccountInfoList.First().Acid)).ToList();

                        Bb_Sector_Code = LoanAccountInfoList.First().Bb_Sector_Code;
                    }

                    CustomerBasicInfo.Bb_Sector_Code = Bb_Sector_Code;
                }
            }

            StringBuilder sb = new StringBuilder();

            LoanApplicationViewModel viewModel = new LoanApplicationViewModel();
            viewModel.CustomerBasicInfo = CustomerBasicInfo;
            viewModel.LoanProprietors = LoanProprietors;
            viewModel.LoanAccountInfoList = LoanAccountInfoList;
            viewModel.Guarantors = Guarantors;


            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = contentRootPath + @"/Templates/loan-application.liquid.html";

            Helpers.LiquidFunctions.RegisterViewModel(typeof(LoanApplicationViewModel));

            Helpers.LiquidFunctions.RegisterSafeTypeWithAllProperties(typeof(CorporateCustomer));
            Helpers.LiquidFunctions.RegisterSafeTypeWithAllProperties(typeof(Proprietor));
            Helpers.LiquidFunctions.RegisterSafeTypeWithAllProperties(typeof(LoanAccountInfo));
            Helpers.LiquidFunctions.RegisterSafeTypeWithAllProperties(typeof(Guarantor));

            var template = DotLiquid.Template.Parse(
                                new System.IO.StreamReader(path)
                                                .ReadToEnd());

            var result = template.RenderViewModel(viewModel);

            sb.Append(result.Replace("\r\n", "").Replace("\t", ""));
            return sb;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Application(string customerId)
        {
            ViewBag.Title = "Digital Loan Application";

            ViewBag.CustomerId = customerId;

            customerId = HttpUtility.HtmlEncode(customerId);

            LoanApplicationViewModel accountInforation = new LoanApplicationViewModel();

            string Bb_Sector_Code = string.Empty;
            if (!string.IsNullOrEmpty(customerId))
            {

                accountInforation.CustomerBasicInfo = (await _unitOfWork.LoanAccountRepo.GetLoanCustomerInfo(customerId)).FirstOrDefault();

                if (accountInforation.CustomerBasicInfo == null || accountInforation.CustomerBasicInfo.Cif  == null)
                {
                    TempData["ErrorMessage"] = "Sorry!!! No Data found. Please provide a valid CIF No!!!";
                }
                else
                {
                    accountInforation.LoanProprietors = (await _unitOfWork.LoanAccountRepo.GetLoanProprietorInfo(customerId)).ToList();

                    accountInforation.LoanAccountInfoList = (await _unitOfWork.LoanAccountRepo.GetLoanAccountInfoByCif(customerId)).ToList();

                    if(accountInforation.LoanAccountInfoList.Count >0)
                    {
                        accountInforation.Guarantors = (await _unitOfWork.LoanAccountRepo.GetLoanAccountGuarantorInfoByLoanNo(accountInforation.LoanAccountInfoList.First().Acid)).ToList();
                        Bb_Sector_Code = accountInforation.LoanAccountInfoList.First().Bb_Sector_Code;
                    }

                    accountInforation.CustomerBasicInfo.Bb_Sector_Code = Bb_Sector_Code;
                }
            }

            if (accountInforation.CustomerBasicInfo == null)
            {
                accountInforation.CustomerBasicInfo = new CorporateCustomer();
            }
            if (accountInforation.LoanProprietors == null)
            {
                accountInforation.LoanProprietors = new List<Proprietor>();
            }
            if (accountInforation.LoanAccountInfoList == null)
            {
                accountInforation.LoanAccountInfoList = new List<LoanAccountInfo>();
                accountInforation.LoanAccountInfoList.Add(new LoanAccountInfo());
            }
            if (accountInforation.Guarantors == null)
            {
                accountInforation.Guarantors = new List<Guarantor>();
            }
            
            return View(accountInforation);
        }

        public async Task<IActionResult> ShowPrintableApplication(string customerId)
        {
            customerId = HttpUtility.HtmlEncode(customerId);

            StringBuilder sb = await GeneratePrintableApplicationFormLiquidHtml(customerId);
            return Content(sb.ToString());
        }

        public async Task<ActionResult> DownloadExcelLoanApplication(string customerId)
        {
            customerId = HttpUtility.HtmlEncode(customerId);

            StringBuilder sb = await GeneratePrintableApplicationFormLiquidHtml(customerId);

            if(sb.Length ==0)
            {
                TempData["ErrorMessage"] = "Sorry!!! No Data found. Please provide a valid CIF No!!!";
                return RedirectToAction("Application");
            }
            string fileName = "DigitalLoanApplication_" + customerId.Trim();
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/vnd.ms-excel", fileName+ ".xls");
        }


        #endregion


        #region Live Loan Account

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> LiveInfo(string accountNo)
        {
            ViewBag.Title = "Live Loan Account Info";

            ViewBag.AccountNo = accountNo;
            accountNo = HttpUtility.HtmlEncode(accountNo);

            LoanAccountViewModel accountInforation = new LoanAccountViewModel();
            if (!string.IsNullOrEmpty(accountNo))
            {
                if (!AccountNumberValidationHelper.IsAccountNoValid(accountNo.Trim()))
                {
                    TempData["ErrorMessage"] = "Sorry!!! Account Number must be 13 or 16 digits & Can't contain Special/Normal characters!!!";
                }
                else
                {
                    if (IsNumberString(accountNo))
                    {
                        int accVal;

                        if(accountNo.Trim().Length == 16)
                        {
                            accVal = Convert.ToInt32(accountNo.Substring(4, 1));
                        }
                        else
                        {
                            accVal = Convert.ToInt32(accountNo.Substring(0, 1));
                        }
                        

                        if (accVal != 6 )
                        {
                            TempData["ErrorMessage"] = "Sorry!!! This is not a Loan Account!!!";
                        }
                        else
                        {
                            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
                            if (!IsAccessable)
                            {
                                TempData["ErrorMessage"] = "Sorry!!! You are not authorized to view this information!!!";
                            }
                            else
                            {
                                accountInforation.LoanAccountInfo = (await _unitOfWork.LoanAccountRepo.GetLoanAccountInfoByAcno(accountNo)).FirstOrDefault();

                                if (accountInforation.LoanAccountInfo == null || accountInforation.LoanAccountInfo.Cust_Id == null)
                                {
                                    TempData["ErrorMessage"] = "Sorry!!! Loan Account not found!!!";
                                }
                                else
                                {
                                    accountInforation.LoanAccountDetails = (await _unitOfWork.LoanAccountRepo.GetLoanAccountDetailsInfoByAcno(accountNo));
                                }
                            }
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sorry!!! Account Number Must be numeric!!!!";
                    }
                }
            }


            if (accountInforation.LoanAccountDetails == null)
            {
                accountInforation.LoanAccountDetails = new List<LoanAccountDetails>();
            }
            if (accountInforation.LoanAccountInfo == null)
            {
                accountInforation.LoanAccountInfo = new LoanAccountInfo();
            }
            
            return View(accountInforation);
        }

        public async Task<IActionResult> ShowLiveLoanPayoff(string accountNo)
        {
            IList<LoanPayOff> loanPayoffInfos = new List<LoanPayOff>();

            accountNo = HttpUtility.HtmlEncode(accountNo);

            if (string.IsNullOrEmpty(accountNo))
            {
                return Content("<h3>Sorry!!! Account Number is Required!!!</h3>");
            }

            if (!IsNumberString(accountNo))
            {
                return Content("<h3>Sorry!!! Account Number must be numeric!!!</h3>");
            }

            string accVal;
            if(accountNo.Trim().Length == 16)
            {
                accVal = accountNo.Substring(4, 1);
            }
            else
            {
                accVal = accountNo.Substring(0, 1);
            }
            
            if (accVal != "6")
            {
                return Content("<h3>Sorry!!! This is not a Loan Account!!!</h3>");
            }

            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);
            if (!IsAccessable)
            {
                return Content("<h3>Sorry!!! You are not authorized to view this information!!!</h3>");
            }

            loanPayoffInfos = (await _unitOfWork.LoanAccountRepo.GetLiveLoanPayOff(accountNo)).ToList();

            if (loanPayoffInfos.Count == 0)
            {
                return Content("<h3>Sorry!!! No loan payoff has been found!!!</h3>");
            }

            return PartialView("_LiveLoanPayoffDetails", loanPayoffInfos);
        }

        #endregion

    }
}
