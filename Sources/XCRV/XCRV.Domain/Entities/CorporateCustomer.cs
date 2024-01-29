using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class CorporateCustomer
    {
        public string Cif { get; set; }
        public string Business_Name { get; set; }
        public string Address { get; set; }
        public string Business_Sector { get; set; }
        public string Bbl_Segmentation_Code { get; set; }
        public string Business_Ownership { get; set; }
        public string Trade_License_Number { get; set; }
        public string Incorpration_Date { get; set; }
        public string Business_Tin { get; set; }
        public string Monthly_Business_Income { get; set; }
        public string Contact_Number { get; set; }
        public string Email_Address { get; set; }
        public string Bb_Sector_Code { get; set; }
    }
}
