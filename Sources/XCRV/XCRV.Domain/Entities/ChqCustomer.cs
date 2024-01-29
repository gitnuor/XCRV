using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class ChqCustomer
    {
        public string Cust_Id { get; set; }
        public string Cust_Name { get; set; }
        public string Acct_Name { get; set; }
        public string Schm_Type { get; set; }
        public string Dateofbirth { get; set; }
        public string Commu_Add { get; set; }
        public string Perm_Add { get; set; }
        public string Mobile { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public IList<ChqStatus> ChqStatusList { get; set; }
    }
}
