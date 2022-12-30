    public class PaymentAccount
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string AccountId { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
    }
