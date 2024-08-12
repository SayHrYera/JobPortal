using JobPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    public class JobController : Controller
    {
        private readonly AppDbContext db;

        public JobController()
        {
            db = new AppDbContext();
        }

        public IActionResult Index(int page = 1, string keyword = "", string category = "", string experience = "", string jobType = "")
        {
            // Lấy danh sách các danh mục công việc và gán vào ViewBag
            var categories = db.JobCategories.ToList();
            ViewBag.Categories = new SelectList(categories, "JobCaterogyId", "JobCaterogyName");

            int pageSize = 10; // Số lượng công việc mỗi trang

            var jobsQuery = db.Jobs.Include(j => j.Country)
                               .Include(j => j.Employers)  // Include thông tin Employer
                               .AsQueryable();

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(keyword))
            {
                jobsQuery = jobsQuery.Where(j => j.Title.Contains(keyword));
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(category))
            {
                var categoryId = db.JobCategories
                    .Where(c => c.JobCaterogyName.ToLower() == category.ToLower())
                    .Select(c => c.JobCaterogyId)
                    .FirstOrDefault();

                if (categoryId != 0)
                {
                    jobsQuery = jobsQuery.Where(j => j.JobCategoryId == categoryId);
                }
            }

            // Lọc theo kinh nghiệm
            if (!string.IsNullOrEmpty(experience))
            {
                jobsQuery = jobsQuery.Where(j => j.Experience == experience);
            }

            // Lọc theo loại công việc
            if (!string.IsNullOrEmpty(jobType))
            {
                jobsQuery = jobsQuery.Where(j => j.JobType == jobType);
            }

            jobsQuery = jobsQuery.Where(j => j.State != "Đã hủy");

            var jobs = jobsQuery.OrderBy(j => j.JobId)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            int totalJobs = jobsQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalJobs / pageSize);

            var viewModel = new JobListViewModel
            {
                Jobs = jobs,
                CurrentPage = page,
                TotalPages = totalPages
            };
            ViewData["Count"] = totalJobs;

            return View(viewModel);
        }

    }

}
