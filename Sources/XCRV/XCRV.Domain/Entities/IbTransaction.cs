using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
	public class IbTransaction
	{
		public string txnStanNo { get; set; }
		public string txnFromAcNo { get; set; }
		public string txnToAcNo { get; set; }
		public string txnTime { get; set; }
		public string txnParticulars { get; set; }
		public string txnAmount { get; set; }
	}
}
