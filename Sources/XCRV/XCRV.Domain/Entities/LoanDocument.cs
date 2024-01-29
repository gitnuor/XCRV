using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class LoanDocument
    {
        public string document_code { get; set; }
        public string ref_desc { get; set; }
        public string received_date { get; set; }
    }
}
