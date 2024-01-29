using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class CustomerDashboardController : BaseController
    {
        private readonly ILogger<CustomerGeneralDetailsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerDashboardController(ILogger<CustomerGeneralDetailsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string customerId)
        {
            CustomerDetails customerDetails = null;
            ViewBag.CustomerId = customerId;

            customerId = HttpUtility.HtmlEncode(customerId);

            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    customerDetails = await _unitOfWork.CustomerDetailsRepo.GetCustomerDetailsByCif(customerId.Trim());
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (customerDetails == null)
            {
                customerDetails = new CustomerDetails();
            }


            return View(customerDetails);
        }

        public async Task<ActionResult> ShowListOfAccount(string customerId)
        {
            IList<CustomerSearch> AccountList = null;
            ViewBag.CustomerId = customerId;
            customerId = HttpUtility.HtmlEncode(customerId);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    AccountList = await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria( Domain.CommonEnums.CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR, customerId.Trim(), isStatementTrue, string.Empty, "1");
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (AccountList == null)
            {
                AccountList = new List<CustomerSearch>();
            }
            return PartialView("_ListOfAccount",AccountList);
        }


        public async Task<IActionResult> ShowAccountDetails(string customerId)
        {
            CustomerMatrixViewModel viewModel = new CustomerMatrixViewModel();

            ViewBag.CustomerId = customerId;
            ViewBag.IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();

            customerId = HttpUtility.HtmlEncode(customerId);

            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    var ageClassification = _unitOfWork.CustomerMatrixRepo.GetCustomerAgeClassification(customerId);
                    var termDepositAccount = _unitOfWork.CustomerMatrixRepo.GetTDA(customerId);
                    ViewBag.termDepositAccount = termDepositAccount.Result.Count();
                    var loanAccount = _unitOfWork.CustomerMatrixRepo.GetLAA(customerId);
                    ViewBag.LoanAccount = loanAccount.Result.Count();
                    var savingAccount = _unitOfWork.CustomerMatrixRepo.GetSBA(customerId);
                    ViewBag.savingAccount = savingAccount.Result.Count();
                    var odAccount = _unitOfWork.CustomerMatrixRepo.GetCAA(customerId);
                    ViewBag.odAccount = odAccount.Result.Count();
                    var transactionFrequencies = _unitOfWork.CustomerMatrixRepo.GetTransactionFrequency(customerId);
                    ViewBag.transactionFrequencies = transactionFrequencies.Result.Count();

                    viewModel.AgeClassification = (await ageClassification).FirstOrDefault();
                    if (viewModel.AgeClassification == null)
                    {
                        viewModel.AgeClassification = new CustomerAgeClassification();
                    }

                    viewModel.TermDepositAccount = (await termDepositAccount).ToList();
                    viewModel.LoanAccount = (await loanAccount).ToList();
                    viewModel.SavingAccount = (await savingAccount).ToList();
                    viewModel.OdAccount = (await odAccount).ToList();
                    viewModel.TransactionFrequency = (await transactionFrequencies).ToList();

                    var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();

                    viewModel.SavingAccount.Where(w => w.schm_code == "SRSTF" && IsStatementTrue == "N").ToList().ForEach(i =>
                    {
                        i.bal = "*******";
                        i.dpst = "*******";
                        i.wtdl = "*******";
                    });
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";

                }
            }

            if (viewModel.AgeClassification == null)
            {
                viewModel.AgeClassification = new CustomerAgeClassification();
            }
            if (viewModel.TermDepositAccount == null)
            {
                viewModel.TermDepositAccount = new List<CustomerMatrix>();
            }
            if (viewModel.LoanAccount == null)
            {
                viewModel.LoanAccount = new List<CustomerMatrix>();
            }
            if (viewModel.SavingAccount == null)
            {
                viewModel.SavingAccount = new List<CustomerMatrix>();
            }
            if (viewModel.OdAccount == null)
            {
                viewModel.OdAccount = new List<CustomerMatrix>();
            }
            if (viewModel.TransactionFrequency == null)
            {
                viewModel.TransactionFrequency = new List<CustomerTransactionFrequency>();
            }

            return PartialView("_AccountDetails",viewModel);
        }

        public async Task<IActionResult> ShowNominees(string customerId)
        {
            IList<NomineeViewModel> nominees = new List<NomineeViewModel> ();
            IList<CustomerSearch> AccountList = new List<CustomerSearch>();
            ViewBag.CustomerId = customerId;
            customerId = HttpUtility.HtmlEncode(customerId);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    AccountList = (await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(Domain.CommonEnums.CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR, customerId.Trim(), isStatementTrue, string.Empty, "1")).Where(p => p.schm_type == "SBA" || p.schm_type == "CAA").ToList();
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (AccountList.Count == 0)
            {
                nominees = new List<NomineeViewModel>();
            }
            else
            {
                foreach(var ac in AccountList)
                {
                    var nm = new NomineeViewModel();
                    nm.AccountNumber = ac.foracid;
                    nm.Nominees = ( await _unitOfWork.NomineeRepo.GetCaSaNominees(ac.foracid)).ToList();

                    nominees.Add(nm);
                }
            }
            return PartialView("_NomineeDetails", nominees);
        }

        public async Task<IActionResult> ShowRelatedParty(string customerId)
        {
            IList<RelatedPartyDashboarViewModel> relatedParties = new List<RelatedPartyDashboarViewModel>();
            IList<CustomerSearch> AccountList = new List<CustomerSearch>();
            ViewBag.CustomerId = customerId;
            customerId = HttpUtility.HtmlEncode(customerId);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    AccountList = (await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(Domain.CommonEnums.CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR, customerId.Trim(), isStatementTrue, string.Empty, "1"));
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (AccountList.Count == 0)
            {
                relatedParties = new List<RelatedPartyDashboarViewModel>();
            }
            else
            {
                foreach (var ac in AccountList)
                {
                    var nm = new RelatedPartyDashboarViewModel();
                    nm.AccountNumber = ac.foracid;

                    var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                    string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                    string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(ac.foracid.Trim());
                    bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(ac.foracid, userName); //_unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);

                    nm.RelatedPartyViewModel = new RelatedPartyViewModel();
                    nm.RelatedPartyViewModel.RelatedPartyInfo = new InterestDetails();
                    nm.RelatedPartyViewModel.RelatedPartyDetails = new List<InterestDetails>();


                    if (!IsAccessable)
                    {
                        nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    }
                    else if (schemeCode == "SRSTF" && IsStatementTrue == "N")
                    {
                        nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    }
                    else
                    {
                        nm.RelatedPartyViewModel = new RelatedPartyViewModel();
                        nm.RelatedPartyViewModel.RelatedPartyInfo = (await _unitOfWork.TransactionDetailsRepo.GetAccRelatedParty(ac.foracid.Trim())).FirstOrDefault();
                        nm.RelatedPartyViewModel.RelatedPartyDetails = await _unitOfWork.TransactionDetailsRepo.GetAccRelatedParty(ac.foracid.Trim());

                        nm.RelatedPartyViewModel.AccountNumber = nm.RelatedPartyViewModel.RelatedPartyInfo.FORACID;
                        nm.RelatedPartyViewModel.RelatedPartyInfo.FORACID = nm.RelatedPartyViewModel.RelatedPartyInfo.FORACID + " " + nm.RelatedPartyViewModel.RelatedPartyInfo.acct_crncy_code + " / " + nm.RelatedPartyViewModel.RelatedPartyInfo.sol_id;

                    }



                    relatedParties.Add(nm);
                }
            }
            return PartialView("_RelatedPartyDashboard", relatedParties);
        }

        public async Task<IActionResult> ShowChequeDetails(string customerId)
        {
            IList<ChequeDetailsViewModel> chequeDetailsViewModels = new List<ChequeDetailsViewModel>();
            IList<CustomerSearch> AccountList = new List<CustomerSearch>();
            ViewBag.CustomerId = customerId;
            customerId = HttpUtility.HtmlEncode(customerId);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    AccountList = (await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(Domain.CommonEnums.CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR, customerId.Trim(), isStatementTrue, string.Empty, "1")).Where(p => p.schm_type == "SBA" || p.schm_type == "CAA").ToList();
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (AccountList.Count == 0)
            {
                chequeDetailsViewModels = new List<ChequeDetailsViewModel>();
            }
            else
            {
                foreach (var ac in AccountList)
                {
                    var nm = new ChequeDetailsViewModel();
                    nm.AccountNumber = ac.foracid;

                    var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                    string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                    string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(ac.foracid.Trim());
                    bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(ac.foracid, userName); //_unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);

                    nm.InwardChequeList = new List<InwardCheque>();
                    nm.OutwardChequeList = new List<OutwardCheque>();

                    if (!IsAccessable)
                    {
                        nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    }
                    else if (schemeCode == "SRSTF" && IsStatementTrue == "N")
                    {
                        nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    }
                    else
                    {
                        nm.InwardChequeList = (await _unitOfWork.ChequeDetailsRepository.GetTop20InwardCheques(ac.foracid)).ToList();
                        nm.OutwardChequeList = (await _unitOfWork.ChequeDetailsRepository.GetTop20OutwardCheques(ac.foracid)).ToList();
                    }
                    chequeDetailsViewModels.Add(nm);
                }
            }
            return PartialView("_ChequeDetails", chequeDetailsViewModels);
        }

        public async Task<IActionResult> ShowEftDetails(string customerId)
        {
            IList<ChequeDetailsViewModel> chequeDetailsViewModels = new List<ChequeDetailsViewModel>();
            IList<CustomerSearch> AccountList = new List<CustomerSearch>();
            ViewBag.CustomerId = customerId;
            customerId = HttpUtility.HtmlEncode(customerId);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    AccountList = (await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(Domain.CommonEnums.CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR, customerId.Trim(), isStatementTrue, string.Empty, "1")).Where(p => p.schm_type == "SBA" || p.schm_type == "CAA").ToList();
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (AccountList.Count == 0)
            {
                chequeDetailsViewModels = new List<ChequeDetailsViewModel>();
            }
            else
            {
                foreach (var ac in AccountList)
                {
                    var nm = new ChequeDetailsViewModel();
                    nm.AccountNumber = ac.foracid;

                    var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                    string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                    string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(ac.foracid.Trim());
                    bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(ac.foracid, userName); //_unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);

                    nm.InwardChequeList = new List<InwardCheque>();
                    nm.OutwardChequeList = new List<OutwardCheque>();

                    if (!IsAccessable)
                    {
                        nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    }
                    else if (schemeCode == "SRSTF" && IsStatementTrue == "N")
                    {
                        nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    }
                    else
                    {
                        nm.InwardChequeList = (await _unitOfWork.ChequeDetailsRepository.GetTop20InwardCheques(ac.foracid)).ToList();
                        nm.OutwardChequeList = (await _unitOfWork.ChequeDetailsRepository.GetTop20OutwardCheques(ac.foracid)).ToList();
                    }
                    chequeDetailsViewModels.Add(nm);
                }
            }
            return PartialView("_EftDetails", chequeDetailsViewModels);
        }

        public async Task<IActionResult> ShowCollectedDocuments(string customerId)
        {
            IList<LoanDocumentDashboardViewModel> chequeDetailsViewModels = new List<LoanDocumentDashboardViewModel>();
            IList<CustomerSearch> AccountList = new List<CustomerSearch>();
            ViewBag.CustomerId = customerId;
            customerId = HttpUtility.HtmlEncode(customerId);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    AccountList = (await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(Domain.CommonEnums.CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR, customerId.Trim(), isStatementTrue, string.Empty, "1")).Where(p => p.schm_type == "LAA").ToList();
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (AccountList.Count == 0)
            {
                chequeDetailsViewModels = new List<LoanDocumentDashboardViewModel>();
            }
            else
            {
                foreach (var ac in AccountList)
                {
                    var nm = new LoanDocumentDashboardViewModel();
                    nm.AccountNumber = ac.foracid;

                    var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                    string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                    string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(ac.foracid.Trim());
                    bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(ac.foracid, userName); //_unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);

                    nm.DocList = new List<LoanDocument>();

                    if (!IsAccessable)
                    {
                        nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    }
                    else if (schemeCode == "SRSTF" && IsStatementTrue == "N")
                    {
                        nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    }
                    else
                    {
                        nm.DocList = (await _unitOfWork.LoanAccountRepo.GetLoanDocumentsByLoanNumber(ac.foracid)).ToList();

                    }

                    chequeDetailsViewModels.Add(nm);
                }
            }
            return PartialView("_CollectedDocument", chequeDetailsViewModels);
        }


        public async Task<IActionResult> ShowDebitCardInfo(string customerId)
        {
            IList<LoanDocumentDashboardViewModel> chequeDetailsViewModels = new List<LoanDocumentDashboardViewModel>();
            IList<string> cardList = new List<string>();
            ViewBag.CustomerId = customerId;
            customerId = HttpUtility.HtmlEncode(customerId);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    cardList = (await _unitOfWork.OracleBaseRepo.GetDebitCardByCustomerNumber(customerId));
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (cardList.Count == 0)
            {
                chequeDetailsViewModels = new List<LoanDocumentDashboardViewModel>();
            }
            else
            {
                foreach (var ac in cardList)
                {
                    var nm = new LoanDocumentDashboardViewModel();
                    nm.AccountNumber = ac;

                    //var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
                    //string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                    //string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(ac.foracid.Trim());
                    //bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(ac.foracid, userName); //_unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);

                    //nm.DocList = new List<LoanDocument>();

                    //if (!IsAccessable)
                    //{
                    //    nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    //}
                    //else if (schemeCode == "SRSTF" && IsStatementTrue == "N")
                    //{
                    //    nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
                    //}
                    //else
                    //{
                    //    nm.DocList = (await _unitOfWork.LoanAccountRepo.GetLoanDocumentsByLoanNumber(ac.foracid)).ToList();

                    //}

                    chequeDetailsViewModels.Add(nm);
                }
            }
            return PartialView("_DebitCardInfo", chequeDetailsViewModels);
        }


        public async Task<IActionResult> GetCustomerMemosByCif(string cif)
        {
            cif = HttpUtility.HtmlEncode(cif);

            
            var customerMemos = await _unitOfWork.CustomerMemoRepo.GetCustomerMemosByCif(cif);
            return PartialView("_DasboardCustomerMemo", customerMemos);

        }


        public async Task<IActionResult> ShowIBInfo(string customerId)
        {
            IList<IbViewModel> chequeDetailsViewModels = new List<IbViewModel>();
            IList<CustomerSearch> cardList = new List<CustomerSearch>();
            ViewBag.CustomerId = customerId;
            customerId = HttpUtility.HtmlEncode(customerId);
            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (IsNumberString(customerId))
                {
                    cardList = (await _unitOfWork.CustomerSearchRepo.SearchCustomerBySearchCriteria(Domain.CommonEnums.CustomerSearchType.SP_SEARCH_CUST_BY_CUST_ID_HR, customerId.Trim(), isStatementTrue, string.Empty, "1")).Where(p => p.schm_type != "LAA").ToList(); //(await _unitOfWork.OracleBaseRepo.GetDebitCardByCustomerNumber(customerId));
                }
                else
                {
                    TempData["ErrorMessage"] = "Sorry!!! Customer Id Must be numeric!!!!";
                }
            }

            if (cardList.Count == 0)
            {
                chequeDetailsViewModels = new List<IbViewModel>();
            }
            else
            {
                foreach (var ac in cardList)
                {
                    var nm = new IbViewModel();
                    nm.AccountNumber = ac.foracid;

                    chequeDetailsViewModels.Add(nm);
                }
            }
            return PartialView("_IbInfo", chequeDetailsViewModels);
        }


        public async Task<IActionResult> ShowIBDetailsInfo(string acno)
        {
            IbViewModel nm = new IbViewModel();

            var IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            string schemeCode = await _unitOfWork.OracleBaseRepo.GetAccountSchemCodeByAccountNumber(acno.Trim());
            bool IsAccessable = await _unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(acno.Trim(), userName); //_unitOfWork.OracleBaseRepo.IsAccountAccessableByUser(accountNo, userName);

            if (!IsAccessable)
            {
                nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
            }
            else if (schemeCode == "SRSTF" && IsStatementTrue == "N")
            {
                nm.ErrorMessage = "Sorry!!! You are not authorized to view this information!!!";
            }
            else
            {
                nm.CustInfo = (await _unitOfWork.InhouseIBRepository.GetIbCusotmerInfo(acno)).FirstOrDefault();
                nm.IbTransactions = (await _unitOfWork.InhouseIBRepository.GetTransactionByAccNo(acno, "10")).ToList();

            }
            if(nm.CustInfo == null)
            {
                nm.CustInfo = new IbCustomerInfo();
            }
            return PartialView("_IbInfoDetails", nm);
        }
    }
}
