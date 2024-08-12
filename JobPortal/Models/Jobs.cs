using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class Jobs
    {
        [Key]
        public int JobId { get; set; }
        public int JobCategoryId { get; set; }
        public int? EmployerId { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }
        public int? NoOfPost { get; set; }
        [MaxLength(5000)]
        public string? Description { get; set; }
        [MaxLength(100)]
        public string? Qualification { get; set; }
        public string? Skill {  get; set; }
        [MaxLength(100)]
        public string? Experience { get; set; }
        [MaxLength(5000)]
        public string? Specialization { get; set; }
        public DateTime LastDateToApply { get; set; }
        [MaxLength(100)]
        public string? Salary { get; set; }
        [MaxLength(100)]
        public string? JobType { get; set; }
        [MaxLength(200)]
        public string? CompanyName { get; set; }
        [MaxLength(100)]
        public string? CompanyImage { get; set; }
        [MaxLength(200)]
        public string? City { get; set; }
        [MaxLength(100)]
        public string? State { get; set; }
        public DateTime? CreateDate { get; set; }

        public int? CountryId { get; set; }

        // Navigation properties
        public virtual Employers? Employers { get; set; }
        public virtual Country? Country { get; set; }
        public virtual JobCaterogies? JobCaterogies { get; set; }
        public virtual ICollection<AppliedJobs>? ApplyJobs { get; set; }
    }
}
