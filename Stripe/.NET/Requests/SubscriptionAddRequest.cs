    public class SubscriptionAddRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string SubscriptionId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string CustomerId { get; set; }
        public DateTime DateEnded { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string InvoiceId { get; set; }
    }
