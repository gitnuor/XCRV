using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class CustomerMatrixViewModel
    {
        public CustomerAgeClassification AgeClassification { get; set; }

        public IList<CustomerMatrix> TermDepositAccount { get; set; }
        public IList<CustomerMatrix> LoanAccount { get; set; }
        public IList<CustomerMatrix> SavingAccount { get; set; }

        public IList<CustomerMatrix> OdAccount { get; set; }

        public IList<CustomerTransactionFrequency> TransactionFrequency { get; set; }

    }
}
