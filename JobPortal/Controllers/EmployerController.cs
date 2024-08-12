using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobPortal.Controllers
{
    public class EmployerController : Controller
    {
        AppDbContext db = new AppDbContext();
        [Authorize(Roles = "Employer")]
        public IActionResult EmployerProfile()
        {
            var username = User.Identity.Name;

            // Tìm người dùng trong cơ sở dữ liệu
            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return NotFound(); // Hoặc xử lý lỗi phù hợp
            }

            // Lấy thông tin ứng viên dựa trên UserId
            var employer = db.Employers
                .Include(c => c.Country).Include(c => c.User) // Nếu bạn cần thông tin quốc gia
                .FirstOrDefault(c => c.UserId == user.UserId);

            if (employer == null)
            {
                return NotFound(); // Hoặc xử lý lỗi phù hợp
            }

            // Trả về view với mô hình Candidate
            return View(employer);
        }
        [Authorize(Roles = "Employer")]
        public IActionResult EditEmployerProfile()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var employer = db.Employers
                .Include(c => c.Country) // Nếu bạn cần thông tin quốc gia
                .FirstOrDefault(c => c.UserId == user.UserId);

            if (employer == null)
            {
                return NotFound("Employer not found");
            }

            // Nếu cần dropdown cho quốc gia
            // var countries = GetCountry();
            // ViewBag.Country = new SelectList(countries, "CountryId", "CountryName");

            return View(employer);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public IActionResult UpdateEmployerProfile(Employers model, IFormFile imgFile)
        {
            try
            {
                var username = User.Identity.Name;
                var user = db.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Kiểm tra nếu tên công ty đã tồn tại
                bool companyNameExists = db.Employers
                    .Any(e => e.CompanyName == model.CompanyName && e.UserId != user.UserId);
                if (companyNameExists)
                {
                    ModelState.AddModelError("CompanyName", "The company name is already in use.");
                }

                if (!ModelState.IsValid)
                {
                    return View("EditEmployerProfile", model); // Trả về view với thông báo lỗi
                }

                var employer = db.Employers.FirstOrDefault(c => c.UserId == user.UserId);
                if (employer == null)
                {
                    return NotFound("Employer not found");
                }

                // Cập nhật thông tin
                employer.CompanyName = model.CompanyName;
                employer.Email = model.Email;
                employer.Phone = model.Phone;
                employer.Address = model.Address;
                employer.ContactName = model.ContactName;
                employer.City = model.City;
                employer.Website = model.Website;

                // Xử lý hình ảnh hồ sơ
                if (imgFile != null && imgFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImageUser", imgFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imgFile.CopyTo(stream);
                    }
                    employer.Logo = "/ImageUser/" + imgFile.FileName;
                }

                db.SaveChanges();
                TempData["SuccessMessagee"] = "Profile updated successfully.";
                return RedirectToAction("EmployerProfile");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và thông báo lỗi
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "An error occurred while updating the profile.";
                return View("EditEmployerProfile", model); // Trả về view với thông báo lỗi
            }
        }

        [Authorize(Roles = "Employer")]
        public IActionResult EditUserProfile()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Chỉ cần thông tin User cho việc chỉnh sửa tài khoản
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public IActionResult UpdateUserProfile(User model)
        {
            try
            {
                var username = User.Identity.Name;
                var user = db.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                bool emailExists = db.Users
                    .Any(e => e.Email == model.Email && e.UserId != user.UserId);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "The email is already in use.");
                }

                bool phoneExists = db.Users
                    .Any(e => e.NumberPhone == model.NumberPhone && e.UserId != user.UserId);
                if (phoneExists)
                {
                    ModelState.AddModelError("NumberPhone", "The phone number is already in use.");
                }
                if (!ModelState.IsValid)
                {
                    return View("EditUserProfile", model);
                }
                // Cập nhật các trường của user
                user.Email = model.Email;
                user.NumberPhone = model.NumberPhone;

                db.SaveChanges();
                TempData["SuccessMessagee"] = "Profile updated successfully.";
                return RedirectToAction("EmployerProfile");
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về thông báo lỗi
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                TempData["ErrorMessage"] = "An error occurred while updating the profile.";
                return View(model); // Trả về view với model hiện tại
            }
        }

        [Authorize(Roles = "Employer")]
        public IActionResult Workmanagement(int page = 1, string keyword = "", string category = "", string experience = "", string jobType = "")
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }
            // Lấy danh sách các danh mục công việc
            var categories = db.JobCategories.ToList();         
            ViewBag.Categories = new SelectList(categories, "JobCaterogyId", "JobCaterogyName");

            int pageSize = 10; // Số lượng công việc mỗi trang

            var jobsQuery = db.Jobs
                .Where(j => j.Employers.UserId == userId) // Lọc công việc của Employer hiện tại
                .Include(j => j.Country)
                .AsQueryable();
            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(keyword))
            {
                jobsQuery = jobsQuery.Where(j => j.Title.Contains(keyword));
            }

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

        [Authorize(Roles = "Employer")]
        public IActionResult DetailJob(int id)
        {
            // Lấy thông tin chi tiết công việc dựa trên JobId
            var job = db.Jobs
                              .Include(j => j.Country)  // Bao gồm thông tin về quốc gia
                              .Include(j => j.ApplyJobs) // Bao gồm danh sách ứng viên đã ứng tuyển
                              .FirstOrDefault(j => j.JobId == id);

            if (job == null)
            {
                // Xử lý trường hợp không tìm thấy công việc
                return NotFound();
            }

            return View(job);
        }
        [HttpPost]
        public IActionResult ChangeStatus(int id, string status)
        {
            var application = db.AppliedJobs.Find(id);
            if (application == null)
            {
                return NotFound();
            }

            application.Status = status;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Trạng thái ứng viên đã được cập nhật thành công.";

            return RedirectToAction("DetailApplyJob", new { id = application.AppliedJobId });
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
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }
                // Tìm EmployerId từ UserId
                var employer = db.Employers.FirstOrDefault(e => e.UserId == userId);
                if (employer == null)
                {
                    return BadRequest("Không tìm thấy nhà tuyển dụng cho người dùng hiện tại.");
                }

                jobs.EmployerId = employer.EmployerId;
                db.Jobs.Add(jobs);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Job tạo thành công!";
                return RedirectToAction("Workmanagement");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public IActionResult DetailApplyJob(int id)
        {
            var application = db.AppliedJobs
                .Include(a => a.Jobs)
                .FirstOrDefault(a => a.AppliedJobId == id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }
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
        [HttpPost]
        public IActionResult EditJob(Jobs jobs, int id, IFormFile imgFile)
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
                job.CountryId = jobs.CountryId;
                job.JobCategoryId = jobs.JobCategoryId;

                db.SaveChanges();
                TempData["SuccessMessage"] = "Job chỉnh sửa thành công!";
                return RedirectToAction("Workmanagement");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
