using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class Proprietor
    {
        public string Cif { get; set; }
        public string Proprietor_Cif { get; set; }
        public string Cust_Reltn_Code { get; set; }
        public string Name_Proprietor { get; set; }
        public string Date_Of_Birth { get; set; }
        public string Father_Name { get; set; }
        public string Mother_Name { get; set; }
        public string Spouse_Name { get; set; }
        public string Marital_Status { get; set; }
        public string Present_Address { get; set; }
        public string Permanent_Address { get; set; }
    }
}
