using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XCRV.Web.Models
{
    public class CustomerMemoViewModel
    {
		
		public long id { get; set; }
		[Required]
		[Display(Name = "Memo Text")]
		public string memoText { get; set; }
		public string entryDate { get; set; }
		public string lastUpdateDate { get; set; }
		[Display(Name = "User Id")]
		public string userId { get; set; }
		public string customerId { get; set; }
	}
}
