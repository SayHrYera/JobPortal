using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class JobCaterogies
    {
        [Key]
        public int JobCaterogyId { get; set; }
        public string JobCaterogyName { get; set; }
        public string? JobCaterogyDesc { get; set; }
        public int? JobCount { get; set; }
        public virtual ICollection<Jobs>? Jobs { get; set; }
    }
}
