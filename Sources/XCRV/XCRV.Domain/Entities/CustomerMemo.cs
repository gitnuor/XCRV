using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class CustomerMemo
    {
		public long id { get; set; }
		public string memoText { get; set; }
		public string entryDate { get; set; }
		public string lastUpdateDate { get; set; }
		public string userId { get; set; }
		public string customerId { get; set; }
	}
}
