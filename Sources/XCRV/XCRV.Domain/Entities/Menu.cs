using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class Menu
    {
        public int MenuId { get; set; }
		public int ParentID { get; set; }
		public string MenuText { get; set; }
		public string NavigateUrl { get; set; }
		public int MenuOrder { get; set; }
    }
}
