using JobPortal.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    public class AccountController : Controller
    {
        AppDbContext dbContext;
        public AccountController()
        {
            dbContext = new AppDbContext();
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return View();
            }

            var user = dbContext.Users.FirstOrDefault(p => p.UserName == username);
            var data = dbContext.Users.FirstOrDefault(p => p.UserName == username && p.Password == password);

            if (user == null || data == null)
            {
                TempData["status"] = "Đăng nhập thất bại XD";
                return RedirectToAction("Login");
            }

            var role = dbContext.Roles.FirstOrDefault(r => r.RoleId == user.RoleId);
            if (role == null)
            {
                TempData["status"] = "Đăng nhập thất bại XD";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role.RoleName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()) // Add UserId claim
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            switch (role.RoleName)
            {
                case "Admin":
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                case "Employer":
                    return RedirectToAction("Index", "Panel");
                case "Candidate":
                    return RedirectToAction("Index", "Panel");
                default:
                    return BadRequest();
            }
        }
        public IActionResult Registration()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            }
            return View();
        }
        [HttpPost]
        public IActionResult Registration(RegistrationCandidate model)
        {
            // Clear existing model state errors
            ModelState.Clear();

            // Check if username is already taken
            if (dbContext.Users.Any(u => u.UserName == model.Username))
            {
                ModelState.AddModelError("Username", "Username đã tồn tại");
            }

            // Check if email is already registered
            if (dbContext.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
            }

            // Check if phone number is already registered
            if (dbContext.Users.Any(u => u.NumberPhone == model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Phone number đã tồn tại");
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Confirm password không trùn với password.");
            }
            // Check if ModelState is valid
            if (ModelState.IsValid)
            {
                // Create User entity
                var user = new User
                {
                    UserName = model.Username,
                    Password = model.Password, // Remember to hash the password
                    Email = model.Email,
                    NumberPhone = model.PhoneNumber,
                    RoleId = 4 // Assuming RoleId assignment
                };

                // Add user to DbContext and save changes
                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                // Create Candidates entity
                var candidates = new Candidates
                {
                    UserId = user.UserId,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    // Additional properties of Candidates if any
                };

                // Add candidates to DbContext and save changes
                dbContext.Candidates.Add(candidates);
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Tạo tài khoản thành công";
                // Redirect to login page after successful registration
                return RedirectToAction("Login", "Account");
            }

            // If ModelState is not valid, return to registration view with errors
            return View(model);
        }


        [HttpGet]
        public IActionResult RegistrationEmployer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegistrationEmployer(RegistrationEmployers model)
        {
            // Clear existing model state errors
            ModelState.Clear();

            // Check if username is already taken
            if (dbContext.Users.Any(u => u.UserName == model.Username))
            {
                ModelState.AddModelError("Username", "Username đã tồn tại");
            }

            // Check if email is already registered
            if (dbContext.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
            }
            // Check if phone number is already registered
            if (dbContext.Users.Any(u => u.NumberPhone == model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Phone number đã tồn tại");
            }
            if (dbContext.Employers.Any(u => u.CompanyName == model.CompanyName))
            {
                ModelState.AddModelError("CompanyName", "Company Name đã tồn tại");
            }
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Confirm password không trùng với password.");
            }
            // Check if ModelState is valid
            if (ModelState.IsValid)
            {
                // Create User entity
                var user = new User
                {
                    UserName = model.Username,
                    Password = model.Password, // Remember to hash the password
                    Email = model.Email,
                    NumberPhone = model.PhoneNumber,
                    RoleId = 3 // Assuming RoleId assignment
                };

                // Add user to DbContext and save changes
                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                // Create Candidates entity
                var employers = new Employers
                {
                    UserId = user.UserId,
                    CompanyName = model.CompanyName,
                    ContactName = model.ContactName,
                    Email = model.Email,
                    Phone = model.PhoneNumber,
                    Address = model.Address,
                    // Additional properties of Candidates if any
                };

                // Add candidates to DbContext and save changes
                dbContext.Employers.Add(employers);
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Tạo tài khoản thành công";
                // Redirect to login page after successful registration
                return RedirectToAction("Login", "Account");
            }

            // If ModelState is not valid, return to registration view with errors
            return View(model);
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [Authorize]
        public IActionResult UserProfile()
        {
            var user = User.Identity.Name;
            // Giả sử bạn có một phương thức để lấy role của người dùng
            var role = GetUserRole(user);

            switch (role)
            {
                case "Employer":
                    return RedirectToAction("EmployerProfile","Employer");
                case "Candidate":
                    return RedirectToAction("CandidateProfile","Candidate");
                default:
                    return BadRequest(ModelState);
            }
        }
        private string GetUserRole(string username)
        {
            var user = dbContext.Users.Include(u => u.Role).FirstOrDefault(u => u.UserName == username);
            return user?.Role?.RoleName ?? "User";
        }
    }
}
