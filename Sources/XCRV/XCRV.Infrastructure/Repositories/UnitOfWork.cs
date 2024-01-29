using System;
using System.Collections.Generic;
using System.Text;
using XCRV.Application.Interfaces;

namespace XCRV.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IUserRepository userRepo, IMenuRepository menuRepository, IMenuPrevilegeRepository menuPrevilegeRepo
            , IOracleBaseRepository oracleBaseRepo, ICustomerSearchRepository customerSearchRepo, ICustomerDetailsRepository customerDetailsRepo
            , ICustomerMemoRepository customerMemoRepository, ICustomerLimitRepository customerLimitRepo, ICustomerMatrixRepository customerMatrixRepo
            , ITransactionDetailsRepository transactionDetailsRepo, IAccountSchemeRepository accountSchemRepo,ICustomerCollateralRepository customerCollateralRepo
            , IUnclearedChqRepository unclearedChqRepo, IRewardPointRepository rewardPointRepo, INomineeRepository nomineeRepo
            , ILoanAccountRepository loanAccountRepo, ICreditCardRepository creditCardRepo, IAccountBufferToolRepository accountBufferToolRepo
            , IMobileVsAccountRepository mobileVsAccountRepo, ISmsRepository smsRepo, IDebitCardRepository debitCardRepo, IMbsRepository mbsRepo
            , ICardChqRepository cardChqRepo, ICrvCardChqRepository crvCardChqRepo, IQmsCardProRepository qmsCardProRepository, IAccessCodeRepository accessKeyRepo
            , IChequeDetailsRepository chequeDetailsRepository, IInhouseIBRepository inhouseIBRepository, ICardProRepository cardProRepository)
        {
            UserRepo = userRepo;
            MenuRepo = menuRepository;
            MenuPrevilegeRepo = menuPrevilegeRepo;
            CustomerMemoRepo = customerMemoRepository;

            OracleBaseRepo = oracleBaseRepo;
            CustomerSearchRepo = customerSearchRepo;
            CustomerDetailsRepo = customerDetailsRepo;
            CustomerLimitRepo = customerLimitRepo;
            CustomerMatrixRepo = customerMatrixRepo;
            TransactionDetailsRepo = transactionDetailsRepo;
            AccountSchemRepo = accountSchemRepo;
            CustomerCollateralRepo = customerCollateralRepo;
            UnclearedChqRepo = unclearedChqRepo;
            RewardPointRepo = rewardPointRepo;
            NomineeRepo = nomineeRepo;

            LoanAccountRepo = loanAccountRepo;

            CreditCardRepo = creditCardRepo;
            AccountBufferToolRepo = accountBufferToolRepo;
            MobileVsAccountRepo = mobileVsAccountRepo;
            SmsRepo = smsRepo;
            DebitCardRepo = debitCardRepo;
            MbsRepo = mbsRepo;
            CardChqRepo = cardChqRepo;
            CrvCardChqRepo = crvCardChqRepo;

            QmsCardProRepo = qmsCardProRepository;
            AccessKeyRepo = accessKeyRepo;
            ChequeDetailsRepository = chequeDetailsRepository;
            InhouseIBRepository = inhouseIBRepository;
            CardProRepository = cardProRepository;
        }

        public IUserRepository UserRepo { get; }
        public IMenuRepository MenuRepo { get; }
        public IMenuPrevilegeRepository MenuPrevilegeRepo { get; }
        public ICustomerMemoRepository CustomerMemoRepo { get; }


        public IOracleBaseRepository OracleBaseRepo { get; }
        public ICustomerSearchRepository CustomerSearchRepo { get; }
        public ICustomerDetailsRepository CustomerDetailsRepo { get; }
        public ICustomerLimitRepository CustomerLimitRepo { get; }
        public ICustomerMatrixRepository CustomerMatrixRepo { get; }
        public ITransactionDetailsRepository TransactionDetailsRepo { get; }
        public IAccountSchemeRepository AccountSchemRepo { get; }
        public ICustomerCollateralRepository CustomerCollateralRepo { get; }

        public IUnclearedChqRepository UnclearedChqRepo { get; }
        public IRewardPointRepository RewardPointRepo { get; }
        public INomineeRepository NomineeRepo { get; }

        public ILoanAccountRepository LoanAccountRepo { get; }

        public ICreditCardRepository CreditCardRepo { get; }

        public IAccountBufferToolRepository AccountBufferToolRepo { get; }
        public IMobileVsAccountRepository MobileVsAccountRepo { get; }
        public IDebitCardRepository DebitCardRepo { get; }

        public IMbsRepository MbsRepo { get; }

        public ISmsRepository SmsRepo { get; }
        public ICardChqRepository CardChqRepo { get; }
        public ICrvCardChqRepository CrvCardChqRepo { get; }

        public IQmsCardProRepository QmsCardProRepo { get; }

        public IAccessCodeRepository AccessKeyRepo { get; }
        public IChequeDetailsRepository ChequeDetailsRepository { get; }
        public IInhouseIBRepository InhouseIBRepository { get; }
        public ICardProRepository CardProRepository { get; }
    }
}
