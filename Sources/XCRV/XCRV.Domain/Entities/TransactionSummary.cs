namespace XCRV.Domain.Entities
{
    public class TransactionSummary
    {
        public string Basic_Card_No { get; set; }
        public string CARDHOLDER_NAME { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string REQUEST_TIME { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string TRXN_AMOUNT { get; set; }
        public string BILL_AMOUNT { get; set; }
        public string DECISION { get; set; }
        public string RESPONSE { get; set; }
        public string MCC { get; set; }
        public string MERCHANT_DESCRIPTION { get; set; }
        public string BIN_ICA { get; set; }
        public string POS_ENTRY_MODE { get; set; }
        public string SETTLED_INDICATOR { get; set; }
    }
}
