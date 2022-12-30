  public class EmailForgotRequest
    {
        [Required]
        [EmailAddress]
       public string Email { get; set; }
    }
