using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class MbsAccountBufferViewModel
    {
        public IEnumerable<MbsBufferCharge> MbsBufferCharges { get; set; }
        public IEnumerable<MbsBufferInterest> MbsBufferInterests { get; set; }

        public MbsAccountBufferViewModel()
        {
            MbsBufferCharges = new List<MbsBufferCharge>();
            MbsBufferInterests = new List<MbsBufferInterest>();
        }
    }
}
