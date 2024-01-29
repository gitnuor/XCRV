using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class SmsBankingDetailsViewModel
    {
        public IReadOnlyList<MobileVsAccount> MobileVsAccounts { get; set; }
        public IReadOnlyList<SmsPull> SmsPulls { get; set; }
        public IReadOnlyList<SmsPush> SmsPushes { get; set; }

        public SmsBankingDetailsViewModel()
        {
            MobileVsAccounts = new List<MobileVsAccount>();
            SmsPulls = new List<SmsPull>();
            SmsPushes = new List<SmsPush>();
        }
    }
}
