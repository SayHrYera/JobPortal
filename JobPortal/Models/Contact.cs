using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        [Required]
        [MaxLength(255)]
        public string ContactName { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        [MaxLength(255)]
        public string Subject { get; set; }
        [MaxLength(5000)]
        public string Message { get; set; }
    }
}
