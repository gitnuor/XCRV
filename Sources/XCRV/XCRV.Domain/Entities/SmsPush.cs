using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class SmsPush
    {
        public string Mob_No { get; set; }
	    public string Body { get; set; }
		public string Status { get; set; }
        public DateTime SysDate { get; set; }
    }
}
