using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        [Required]
        [MaxLength(255)]
        public string CountryName { get; set; }

        // Navigation properties
        public virtual ICollection<Candidates>? Candidates { get; set; }
        public virtual ICollection<Employers>? Employers { get; set; }
        public virtual ICollection<Jobs>? Jobs { get; set; }
    }
}
