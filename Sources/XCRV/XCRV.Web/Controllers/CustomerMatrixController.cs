using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Common;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class CustomerMatrixController : BaseController
    {
        private readonly ILogger<CustomerMatrixController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerMatrixController(ILogger<CustomerMatrixController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index(string customerId)
        {
            CustomerMatrixViewModel viewModel = new CustomerMatrixViewModel();

            ViewBag.CustomerId = customerId;
            ViewBag.IsStatementTrue = User.Claims.FirstOrDefault(p => p.Type == "IsStatementTrue").Value.ToString();

            customerId = HttpUtility.HtmlEncode(customerId);

            if (!string.IsNullOrEmpty(customerId))
            {
                if(IsNumberString(customerId))
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
            
            if(viewModel.AgeClassification == null)
            {
                viewModel.AgeClassification = new CustomerAgeClassification();
            }
            if(viewModel.TermDepositAccount == null)
            {
                viewModel.TermDepositAccount = new List<CustomerMatrix>();
            }
            if(viewModel.LoanAccount == null)
            {
                viewModel.LoanAccount = new List<CustomerMatrix>();
            }
            if(viewModel.SavingAccount == null)
            {
                viewModel.SavingAccount = new List<CustomerMatrix>();
            }
            if(viewModel.OdAccount == null)
            {
                viewModel.OdAccount = new List<CustomerMatrix>();
            }
            if(viewModel.TransactionFrequency == null)
            {
                viewModel.TransactionFrequency = new List<CustomerTransactionFrequency>();
            }

            return View(viewModel);
        }
    }
}
