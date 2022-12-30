public class PhishingAddRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string FromEmail { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string FromName { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
