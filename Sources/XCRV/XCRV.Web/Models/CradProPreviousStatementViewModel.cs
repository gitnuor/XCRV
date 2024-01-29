using System.Collections.Generic;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class CradProPreviousStatementViewModel
    {
        public CradProStatementSummary CradProStatementSummary { get; set; }

        public IList<CardProStatementDetails> CardProTransactions { get; set; }
    }
}
