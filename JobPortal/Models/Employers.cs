using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class Employers
    {
        [Key]
        public int EmployerId { get; set; }
        public int UserId { get; set; }
        [Required]
        [MaxLength(255)]
        public string CompanyName { get; set; }
        [MaxLength(255)]
        public string ContactName { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(15)]
        public string Phone { get; set; }
        [MaxLength(4000)]
        public string Address { get; set; }
        [MaxLength(255)]
        public string? City { get; set; }
        [MaxLength(300)]
        public string? Website { get; set; }
        [MaxLength(5000)]
        public string? Description { get; set; }
        [MaxLength(255)]
        public string? Logo { get; set; }
        [MaxLength(255)]
        public string? Industry { get; set; }
        public int? CountryId { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Country? Country { get; set; }
        public virtual ICollection<Jobs>? Jobs { get; set; }
    }
}
