    public class Transaction
    {
        public int Id { get; set; }
        public int PaymentTypeId { get; set; }
        public string ExternalTransactionId { get; set; }
        public string ExternalUserId { get; set; }
        public Decimal AmountCharged { get; set; }
        public int CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string InvoiceId { get; set; }
    }
