using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class MenuPrevilege
    {
		public string MainMenuTitle { get; set; }
		public string MainMenuLinkPage { get; set; }
	 	public string SubMenuTitle { get; set; }
		public string SubMenuLinkPage { get; set; }
		public string SubChildMenuTitle { get; set; }
		public string SubChildMenuLinkPage { get; set; }
	}
}
