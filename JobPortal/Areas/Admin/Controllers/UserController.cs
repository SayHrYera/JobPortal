using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        AppDbContext _context = new AppDbContext();
        [HttpGet]
        [Route("Admin/User/Users")]
        public IActionResult Users()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .ToList();

            return View(users);
        }

        [HttpGet]
        [Route("Admin/User/EditUser/{id}")]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        [HttpPost]
        [Route("Admin/User/EditUser/{id}")]
        public IActionResult EditUser(int id, User user)
        {
            var existingUser = _context.Users.Find(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Các kiểm tra trùng lặp dữ liệu
            if (_context.Users.Any(u => u.UserName == user.UserName && u.UserId != id))
            {
                TempData["ErrorMessage"] = "Tên người dùng đã tồn tại.";
                return View(user);
            }

            if (_context.Users.Any(u => u.Email == user.Email && u.UserId != id))
            {
                TempData["ErrorMessage"] = "Email đã tồn tại.";
                return View(user);
            }

            if (_context.Users.Any(u => u.NumberPhone == user.NumberPhone && u.UserId != id))
            {
                TempData["ErrorMessage"] = "Số điện thoại đã tồn tại.";
                return View(user);
            }

            // Cập nhật các thông tin
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.NumberPhone = user.NumberPhone;

            _context.Update(existingUser);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Người dùng đã được cập nhật thành công.";
            return RedirectToAction("Users");                       
        }
        
    }
}
