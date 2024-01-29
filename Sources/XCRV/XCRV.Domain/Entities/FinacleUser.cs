using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class FinacleUser
    {
        public string access_id { get; set; }
        public string user_id { get; set; }
        public string xcrv_user_id { get; set; }
        public string acct_access_code { get; set; }
    }
}
