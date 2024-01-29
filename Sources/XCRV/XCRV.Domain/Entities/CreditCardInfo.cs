using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class CreditCardInfo
    {
        public string Customer_Id { get; set; }
        public string Card_No { get; set; }
        public string Cardholder_Name { get; set; }
        public string Delivery_Address { get; set; }
        public string Mobile_No { get; set; }

        public string Occupation { get; set; }
        public string Home_Full_Address { get; set; }
        public string Home_City { get; set; }
        public string Home_State { get; set; }
        public string Home_Post_Code { get; set; }
        public string Home_Country { get; set; }
        public string Organization { get; set; }
        public string Designation { get; set; }
        public string Company_Full_Address { get; set; }
        public string Company_City { get; set; }
        public string Company_State { get; set; }
        public string Company_Post_Code { get; set; }
        public string Company_Country { get; set; }
        public string Pso_Code { get; set; }
        public string Production_Date { get; set; }
        public string Activation_Date { get; set; }
        public string First_Use_Date { get; set; }
        public string Card_Limit { get; set; }
        public string Current_Outstanding { get; set; }
    }
}
