using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class Candidates
    {
        [Key]
        public int CandidateId { get; set; }
        public int UserId { get; set; }
        [MaxLength(100)]
        public string? FullName { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        [MaxLength(100)]
        public string? TenthGrade { get; set; }
        [MaxLength(100)]
        public string? TwelfthGrade { get; set; }
        [MaxLength(100)]
        public string? GraduationGrade { get; set; }
        [MaxLength(100)]
        public string? Phd { get; set; }
        [MaxLength(100)]
        public string? WorksOn { get; set; }
        [MaxLength(100)]
        public string? Experience { get; set; }
        [MaxLength(100)]
        public string? Resume { get; set; }
        [MaxLength(4000)]
        public string? Address { get; set; }
        public int? CountryId { get; set; }

        // Navigation properties
        public virtual Country? Country { get; set; }
        public virtual User? User { get; set; }
    }
}
