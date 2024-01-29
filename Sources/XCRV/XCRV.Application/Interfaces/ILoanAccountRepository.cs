using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ILoanAccountRepository : IGenericRepository<LoanAccountInfo>
    {
        Task<IEnumerable<LoanAccountInfo>> GetLoanAccountInfoByAcno(string acno);
        Task<IEnumerable<LoanAccountDetails>> GetLoanAccountDetailsInfoByAcno(string acno);
        Task<IEnumerable<RepaymentHistory>> GetRepaymentHistoryByAcno(string acno);

        Task<IEnumerable<LoanPayoffInfo>> GetLoanPayOff(string acno, string payOffDate);
        Task<IEnumerable<LoanPayOff>> GetLiveLoanPayOff(string acno);

        Task<IEnumerable<Guarantor>> GetLoanGuarantorByLoanNumber(string loanNo);
        Task<IEnumerable<LoanDocument>> GetLoanDocumentsByLoanNumber(string loanNo);
        Task<IEnumerable<LoanAccountLimit>> GetLoanAccountLimitByLoanNumber(string loanNo);

        Task<IEnumerable<CorporateCustomer>> GetLoanCustomerInfo(string cif);
        Task<IEnumerable<Proprietor>> GetLoanProprietorInfo(string cif);
        Task<IEnumerable<LoanAccountInfo>> GetLoanAccountInfoByCif(string cif);
        Task<IEnumerable<Guarantor>> GetLoanAccountGuarantorInfoByLoanNo(string loanNo);
    }
}
