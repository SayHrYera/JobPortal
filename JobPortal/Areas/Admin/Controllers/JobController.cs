using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Job")]
    [Authorize(Roles = "Admin")]
    public class JobController : Controller
    {
        AppDbContext db;
        public JobController()
        {
            db = new AppDbContext();
        }
        public IActionResult Index()
        {
            var joblist = db.Jobs.Include(j => j.Employers).Include(j => j.Country).Include(j => j.JobCaterogies).ToList();
            return View(joblist);
        }
        [HttpGet("Create-Jobs")]

        public IActionResult CreateJob()
        {
            var jobcategory = GetCaterogies();
            ViewBag.Categories = new SelectList(jobcategory, "JobCaterogyId", "JobCaterogyName");
            var emp = GetEmployers();
            ViewBag.Employers = new SelectList(emp, "EmployerId", "CompanyName");
            var country = GetCountry();
            ViewBag.Country = new SelectList(country, "CountryId", "CountryName");
            return View();
        }
        [HttpPost("Create-Jobs")]
        public IActionResult CreateJob(Jobs jobs, IFormFile imgFile)
        {
            if (jobs == null)
            {
                return BadRequest("Đối tượng công việc không thể rỗng.");
            }

            if (imgFile == null || imgFile.Length == 0)
            {
                return BadRequest("Tệp hình ảnh không được chọn hoặc trống.");
            }
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imagejob", imgFile.FileName);
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }   
                var stream = new FileStream(path, FileMode.Create);
                {
                    imgFile.CopyTo(stream);
                }
                jobs.CompanyImage = imgFile.FileName;
                jobs.CreateDate = DateTime.Now;
                db.Jobs.Add(jobs);  
                db.SaveChanges();
                TempData["SuccessMessage"] = "Job tạo thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Job-Detail/{id}")]
        public IActionResult DetailJob(int id)
        {
            var job = db.Jobs.Include(j => j.Employers).Include(j => j.Country).Include(j => j.JobCaterogies).FirstOrDefault(j => j.JobId == id);
            if (job == null)
            {
                return NotFound("Job not found.");
            }

            return View(job);
        }
        [HttpGet("Job-Edit/{id}")]
        public IActionResult EditJob(int id)
        {
            var jobcategory = GetCaterogies();
            ViewBag.Categories = new SelectList(jobcategory, "JobCaterogyId", "JobCaterogyName");
            var emp = GetEmployers();
            ViewBag.Employers = new SelectList(emp, "EmployerId", "CompanyName");
            var country = GetCountry();
            ViewBag.Country = new SelectList(country, "CountryId", "CountryName");

            var job = db.Jobs.Include(j => j.Employers).Include(j => j.Country).Include(j => j.JobCaterogies).FirstOrDefault(j => j.JobId == id);
            if (job == null)
            {
                return NotFound("Job not found.");
            }

            return View(job);
        }

        [HttpPost("Job-Edit/{id}")]
        public IActionResult EditJob(int id, Jobs jobs, IFormFile imgFile)
        {
            if (id != jobs.JobId)
            {
                return BadRequest("Job ID không khớp");
            }

            var job = db.Jobs.Find(id);
            if (job == null)
            {
                return NotFound();
            }

            try
            {
                if (imgFile != null && imgFile.Length > 0)
                {
                    // Đường dẫn thư mục ảnh
                    string imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imagejob");

                    // Đường dẫn ảnh cũ
                    string oldImagePath = Path.Combine(imageFolderPath, job.CompanyImage);

                    // Xóa ảnh cũ nếu tồn tại
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Tạo tên file mới với GUID để tránh trùng lặp
                    string newFileName = Path.GetFileNameWithoutExtension(imgFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imgFile.FileName);
                    string newImagePath = Path.Combine(imageFolderPath, newFileName);

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(imageFolderPath))
                    {
                        Directory.CreateDirectory(imageFolderPath);
                    }

                    // Lưu ảnh mới
                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        imgFile.CopyTo(stream);
                    }

                    // Cập nhật tên ảnh trong model
                    job.CompanyImage = newFileName;
                }

                // Cập nhật các thuộc tính khác
                job.Title = jobs.Title;
                job.NoOfPost = jobs.NoOfPost;
                job.Description = jobs.Description;
                job.Qualification = jobs.Qualification;
                job.Experience = jobs.Experience;
                job.Skill = jobs.Skill;
                job.Specialization = jobs.Specialization;
                job.LastDateToApply = jobs.LastDateToApply;
                job.Salary = jobs.Salary;
                job.JobType = jobs.JobType;
                job.City = jobs.City;
                job.State = jobs.State;
                job.EmployerId = jobs.EmployerId;
                job.CountryId = jobs.CountryId;
                job.JobCategoryId = jobs.JobCategoryId;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                TempData["SuccessMessage"] = "Job chỉnh sửa thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về thông báo lỗi
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Job-Delete/{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var job = db.Jobs.Find(id);
                if (job == null)
                {
                    return NotFound("Job not found.");
                }

                // Nếu job có các liên kết, xử lý chúng trước khi xóa
                var appliedJobs = db.AppliedJobs.Where(aj => aj.JobId == id).ToList();
                db.AppliedJobs.RemoveRange(appliedJobs);

                db.Jobs.Remove(job);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Job xóa thành công!";
            }
            catch (Exception ex)
            {
                // Logging exception (bạn có thể sử dụng một logger thực sự ở đây)
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình xóa Job.";
                return BadRequest("Có lỗi xảy ra trong quá trình xóa Job.");
            }

            return RedirectToAction("Index");
        }

        private List<JobCaterogies> GetCaterogies()
        {
            var joblist = db.JobCategories.ToList();
            return joblist;
        }
        private List<Employers> GetEmployers()
        {
            var emplist = db.Employers.ToList();
            return emplist;
        }
        private List<Country> GetCountry()
        {
            var countrylist = db.Countries.ToList();
            return countrylist;
        }
    }
}
