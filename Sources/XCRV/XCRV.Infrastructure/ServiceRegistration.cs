using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XCRV.Application.Interfaces;
using XCRV.Infrastructure.Repositories;
using XCRV.OracleInfrastructure.Repositories;

namespace XCRV.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {            
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IMenuPrevilegeRepository, MenuPrevilegeRepository>();
            services.AddTransient<ICustomerMemoRepository, CustomerMemoRepository>();

            services.AddTransient<IOracleBaseRepository, OracleBaseRepository>();
            services.AddTransient<ICustomerSearchRepository, CustomerSearchRepository>();
            services.AddTransient<ICustomerDetailsRepository, CustomerDetailsRepository>();
            services.AddTransient<ICustomerLimitRepository, CustomerLimitRepository>();
            services.AddTransient<ICustomerMatrixRepository, CustomerMatrixRepository>();
            services.AddTransient<ITransactionDetailsRepository, TransactionDetailsRepository>();
            services.AddTransient<IAccountSchemeRepository, AccountSchemeRepository>();
            services.AddTransient<ICustomerCollateralRepository, CustomerCollateralRepository>();

            services.AddTransient<IUnclearedChqRepository, UnclearedChqRepository>();
            services.AddTransient<IRewardPointRepository, RewardPointRepository>();
            services.AddTransient<INomineeRepository, NomineeRepository>();

            services.AddTransient<ILoanAccountRepository, LoanAccountRepository>();


            services.AddTransient<ICreditCardRepository, CreditCardRepository>();
            services.AddTransient<IAccountBufferToolRepository, AccountBufferToolRepository>();
            services.AddTransient<IMobileVsAccountRepository, MobileVsAccountRepository>();
            services.AddTransient<ISmsRepository, SmsRepository>();
            services.AddTransient<IDebitCardRepository, DebitCardRepository>();
            services.AddTransient<IMbsRepository, MbsRepository>();
            services.AddTransient<ICardChqRepository, CardChqRepository>();
            services.AddTransient<ICrvCardChqRepository, CrvCardChqRepository>();
            services.AddTransient<IQmsCardProRepository, QmsCardProRepository>();
            services.AddTransient<IAccessCodeRepository, AccessCodeRepository>();
            services.AddTransient<IChequeDetailsRepository, ChequeDetailsRepository>();
            services.AddTransient<IInhouseIBRepository, InhouseIBRepository>();
            services.AddTransient<ICardProRepository, CardProRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
