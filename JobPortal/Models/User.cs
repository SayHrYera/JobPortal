using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }
        [MaxLength(15)]
        public string NumberPhone { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        public int RoleId { get; set; }

        // Navigation properties
        public virtual ICollection<AppliedJobs>? AppliedJobs { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Candidates? Candidates { get; set; }
        public virtual Employers? Employers { get; set; }
    }
}
