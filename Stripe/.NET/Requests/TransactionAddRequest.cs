    public class TransactionAddRequest
    {
        [Required, Range(1, Int32.MaxValue)]
        public int PaymentTypeId { get; set; }

        [Required, StringLength(255, MinimumLength = 2)]
        public string ExternalTransactionId { get; set; }

        [Required, StringLength(255, MinimumLength = 2)]
        public string ExternalUserId { get; set; }

        public Decimal AmountCharged { get; set; }

        public int CreatedBy { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public string InvoiceId { get; set; }
    }
