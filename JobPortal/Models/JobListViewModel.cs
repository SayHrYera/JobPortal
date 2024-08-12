namespace JobPortal.Models
{
    public class JobListViewModel
    {
        public IEnumerable<Jobs> Jobs { get; set; }
        public IEnumerable<JobCaterogies> JobCategories { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
