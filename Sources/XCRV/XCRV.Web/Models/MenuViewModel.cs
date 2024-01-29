using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XCRV.Web.Models
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }

        public List<MenuViewModel> SubMenu { get; set; }
    }
}
