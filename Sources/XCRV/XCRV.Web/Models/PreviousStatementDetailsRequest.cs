namespace XCRV.Web.Models
{
    public class PreviousStatementDetailsRequest
    {
        public string CardNo { get;set;}    
        public string Month { get; set;}    
        public string Year { get;set; }     
        public string AccountType { get; set; }
    }

    public class CreditHistoryRequest
    {
        public string CardNo { get; set; }
        public string AccountType { get; set; }
    }
}
