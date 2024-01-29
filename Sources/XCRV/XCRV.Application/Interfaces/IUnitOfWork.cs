using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Application.Interfaces
{
    public interface IUnitOfWork
    {        
        IUserRepository UserRepo { get; }
        IMenuRepository MenuRepo { get; }
        IMenuPrevilegeRepository MenuPrevilegeRepo { get; }
        ICustomerMemoRepository CustomerMemoRepo { get; }

        IOracleBaseRepository OracleBaseRepo { get; }
        ICustomerSearchRepository CustomerSearchRepo { get; }
        ICustomerDetailsRepository CustomerDetailsRepo { get; }
        ICustomerLimitRepository CustomerLimitRepo { get; }
        ICustomerMatrixRepository CustomerMatrixRepo { get; }
        IAccountSchemeRepository AccountSchemRepo { get; }
        ITransactionDetailsRepository TransactionDetailsRepo { get; }
        ICustomerCollateralRepository CustomerCollateralRepo { get; }

        IUnclearedChqRepository UnclearedChqRepo { get; }
        IRewardPointRepository RewardPointRepo { get; }
        INomineeRepository NomineeRepo { get; }

        ILoanAccountRepository LoanAccountRepo { get; }

        ICreditCardRepository CreditCardRepo { get; }

        IAccountBufferToolRepository AccountBufferToolRepo { get; }
        IMobileVsAccountRepository MobileVsAccountRepo { get; }

        IDebitCardRepository DebitCardRepo { get; }

        IMbsRepository MbsRepo { get; }

        ISmsRepository SmsRepo { get; }
        ICardChqRepository CardChqRepo { get; }
        ICrvCardChqRepository CrvCardChqRepo { get; }

        IQmsCardProRepository QmsCardProRepo { get; }
        IAccessCodeRepository AccessKeyRepo { get; }

        IChequeDetailsRepository ChequeDetailsRepository { get; }

        IInhouseIBRepository InhouseIBRepository { get; }

        ICardProRepository CardProRepository { get;  }
    }
}
