using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class DebitCardDetail
    {
        public string Emboss_Name { get; set; }
        public string Customer_Id { get; set; }
        public string Card_Type { get; set; }
        public string CardType { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string Daily_Card_Limit { get; set; }
        public string Pos_Purchase_Limit { get; set; }
        public string Address { get; set; }
        public string CustomerDOB { get; set; }

        public string Card_Number { get; set; }
        public string Masked_Card_Number { get; set; }
    }
}
