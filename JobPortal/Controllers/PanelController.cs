using JobPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobPortal.Controllers
{
    public class PanelController : Controller
    {
       AppDbContext db;

        public PanelController()
        {
            db = new AppDbContext();
        }

        public IActionResult Index()
        {
            var jobs = db.Jobs
                .Include(j => j.JobCaterogies)
                .Include(j => j.Country)
                .Include(j => j.Employers)  // Include thông tin của employer
                .Where(j => j.State != "Đã hủy")
                .ToList();

            var jobCategories = db.JobCategories
                .Select(category => new JobCaterogies
                {
                    JobCaterogyName = category.JobCaterogyName,
                    JobCount = db.Jobs.Count(job => job.JobCategoryId == category.JobCaterogyId)
                })
                .ToList();

            var viewModel = new JobListViewModel
            {
                Jobs = jobs,
                JobCategories = jobCategories
            };

            return View(viewModel);
        }

        public IActionResult DetailJob(int id)
        {
            var job = db.Jobs
                .Include(j => j.JobCaterogies)
                .Include(j => j.Country)
                .Include(j => j.Employers)
                .FirstOrDefault(j => j.JobId == id);

            if (job == null)
            {
                return NotFound("Job not found.");
            }

            return View(job);
        }

        public IActionResult JobsByCategory(string category)
        {
            return RedirectToAction("Index", "Job", new { category = category });
        }


        [HttpPost]
        public IActionResult ApplyJob(int jobId, string name, string email, string portfolios, IFormFile cvFile, string coverLetter)
        {
            // Kiểm tra người dùng có đăng nhập và có vai trò candidate không
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Candidate"))
            {
                TempData["ErrorMessage"] = "Bạn phải đăng nhập với vai trò candidate để apply cho công việc.";
                return RedirectToAction("DetailJob", new { id = jobId });
            }

            // Lấy UserId từ claims và chuyển đổi sang int
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Không thể lấy thông tin người dùng.";
                return RedirectToAction("DetailJob", new { id = jobId });
            }

            // Kiểm tra trạng thái của công việc
            var job = db.Jobs.FirstOrDefault(j => j.JobId == jobId);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Công việc không tồn tại.";
                return RedirectToAction("DetailJob", new { id = jobId });
            }

            if (job.State != "Đang mở")
            {
                TempData["ErrorMessage"] = "Công việc này không còn hoặc chưa mở để ứng tuyển.";
                return RedirectToAction("DetailJob", new { id = jobId });
            }

            // Kiểm tra xem người dùng đã apply công việc này chưa
            var existingApplication = db.AppliedJobs
                .FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "Bạn đã apply cho công việc này rồi.";
                return RedirectToAction("DetailJob", new { id = jobId });
            }

            // Lưu thông tin ứng tuyển
            var appliedJob = new AppliedJobs
            {
                JobId = jobId,
                UserId = userId,
                YourName = name,
                Email = email,
                Portfolios = portfolios,
                Coverletter = coverLetter,
                ApplicationDate = DateTime.Now,
                Status = "Đang chờ"
            };

            if (cvFile != null && cvFile.Length > 0)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Cvs", cvFile.FileName);
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    cvFile.CopyTo(stream);
                }
                appliedJob.ImageUrl = cvFile.FileName;
            }

            db.AppliedJobs.Add(appliedJob);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Bạn đã apply thành công!";
            return RedirectToAction("DetailJob", new { id = jobId });
        }

    }
}
