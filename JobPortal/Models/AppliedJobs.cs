using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class AppliedJobs
    {
        [Key]
        public int AppliedJobId { get; set; }
        public int? JobId { get; set; }
        public int UserId { get; set; }
        [MaxLength(255)]
        public string YourName { get; set; }
        public string Email { get; set; }
        public string? Coverletter { get; set; }
        public string? Portfolios { get; set; }
        public string? ImageUrl { get; set; }
        public string? Status { get; set; }
        public DateTime? ApplicationDate { get; set; }

        // Navigation properties
        public virtual Jobs? Jobs { get; set; }
        public virtual User? User { get; set; }

    }
}
