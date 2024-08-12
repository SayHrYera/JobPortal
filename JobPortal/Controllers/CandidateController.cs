using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace JobPortal.Controllers
{
    public class CandidateController : Controller
    {
        AppDbContext db = new AppDbContext();
        
        [Authorize(Roles = "Candidate")]
        public IActionResult CandidateProfile()
        {
            var username = User.Identity.Name;

            // Tìm người dùng trong cơ sở dữ liệu
            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return NotFound(); // Hoặc xử lý lỗi phù hợp
            }

            // Lấy thông tin ứng viên dựa trên UserId
            var candidate = db.Candidates
                .Include(c => c.Country).Include(c => c.User) // Nếu bạn cần thông tin quốc gia
                .FirstOrDefault(c => c.UserId == user.UserId);

            if (candidate == null)
            {
                return NotFound(); // Hoặc xử lý lỗi phù hợp
            }

            // Trả về view với mô hình Candidate
            return View(candidate);
        }
        [Authorize(Roles = "Candidate")]
        public IActionResult EditCandidateProfile()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }

            var candidate = db.Candidates
                .Include(c => c.Country) // Nếu bạn cần thông tin quốc gia
                .FirstOrDefault(c => c.UserId == user.UserId);

            if (candidate == null)
            {
                return NotFound();
            }

            // Nếu cần dropdown cho quốc gia
            // var countries = GetCountry();
            // ViewBag.Country = new SelectList(countries, "CountryId", "CountryName");

            return View(candidate);
        }
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public IActionResult UpdateCandidateProfile(Candidates model, IFormFile imgFile)
        {
            try
            {
                var username = User.Identity.Name;
                var user = db.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var candidate = db.Candidates.FirstOrDefault(c => c.UserId == user.UserId);
                if (candidate == null)
                {
                    return NotFound("Candidate not found");
                }

                // Update candidate fields
                candidate.FullName = model.FullName;
                candidate.Email = model.Email;
                candidate.PhoneNumber = model.PhoneNumber;
                candidate.Address = model.Address;
                candidate.TenthGrade = model.TenthGrade;
                candidate.TwelfthGrade = model.TwelfthGrade;
                candidate.GraduationGrade = model.GraduationGrade;
                candidate.Phd = model.Phd;
                candidate.Experience = model.Experience;

                // Handle profile image upload
                if (imgFile != null && imgFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImageUser", imgFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imgFile.CopyTo(stream);
                    }
                    candidate.Resume = "/ImageUser/" + imgFile.FileName;
                }

                db.SaveChanges();
                TempData["SuccessMessagee"] = "Profile updated successfully.";
                return RedirectToAction("CandidateProfile");
            }
            catch (Exception ex)
            {
                // Log exception
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Candidate")]
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
        [Authorize(Roles = "Candidate")]
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
                return RedirectToAction("CandidateProfile");
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
        [Authorize(Roles = "Candidate")]
        public IActionResult AppliedJobs()
        {
            // Giả sử userId được lưu dưới dạng int trong User.Identity
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var appliedJobs = db.AppliedJobs
                .Include(a => a.Jobs)
                .Include(a => a.Jobs.Employers)
                .Where(a => a.UserId == userId)
                .ToList();

            return View(appliedJobs);
        }

        [Authorize(Roles = "Candidate")]
        public IActionResult ApplicationDetails(int jobId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var application = db.AppliedJobs
                .Include(a => a.Jobs)
                .Include(a => a.Jobs.Employers)
                .FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);

            if (application == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin ứng tuyển.";
                return RedirectToAction("AppliedJobs");
            }

            return View(application);
        }

        [Authorize(Roles = "Candidate")]
        public IActionResult DeleteApplication(int jobId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var application = db.AppliedJobs.FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);

            if (application != null)
            {
                db.AppliedJobs.Remove(application);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Hủy ứng tuyển thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy công việc đã ứng tuyển.";
            }

            return RedirectToAction("AppliedJobs");
        }

    }
}
