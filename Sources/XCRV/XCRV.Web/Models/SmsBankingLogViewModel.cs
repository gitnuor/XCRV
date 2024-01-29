using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class SmsBankingLogViewModel
    {
        public IEnumerable<AcMobile> AcMobiles { get; set; }

        public IEnumerable<SmsPull> SmsLog { get; set; }

        public SmsBankingLogViewModel()
        {
            AcMobiles = new List<AcMobile>();
            SmsLog = new List<SmsPull>();
        }
    }
}
