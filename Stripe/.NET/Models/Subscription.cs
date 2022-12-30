    public class Subscription
    {
        public int Id { get; set; }
        public string SubscriptionId { get; set; }
        public string CustomerId { get; set; }
        public DateTime DateEnded { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string InvoiceId { get; set; }

    }
