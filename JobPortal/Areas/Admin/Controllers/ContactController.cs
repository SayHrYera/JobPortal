using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[Controller]")]
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        AppDbContext db = new AppDbContext();
        public IActionResult Index()
        {
            var contacts = db.Contacts.ToList();
            return View(contacts);
        }
        [HttpPost]
        public IActionResult DeleteContact(int id)
        {
            var deletecontact = db.Contacts.Find(id);
            if(deletecontact == null)
            {
                return NotFound("");
            }
            db.Contacts.Remove(deletecontact);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Contact xóa thành công!";
            return RedirectToAction("Index");
        }
    }
}
