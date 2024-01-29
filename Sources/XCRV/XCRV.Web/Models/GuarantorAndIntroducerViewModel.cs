using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class GuarantorAndIntroducerViewModel
    {
        public IEnumerable<Guarantor> Guarantors { get; set; }
        public IEnumerable<Introducer> Introducers { get; set; }
    }
}
