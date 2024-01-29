using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class IbCustomerInfo
    {
        public string MasterAccName { get; set; }
		public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string LastLoginDateTime { get; set; }
        public string InternetBanking { get; set; }
    }
}
