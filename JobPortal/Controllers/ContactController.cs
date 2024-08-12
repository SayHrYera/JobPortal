using JobPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    public class ContactController : Controller
    {
        AppDbContext dbContext = new AppDbContext();
        public IActionResult Index()
        {
            return View(new Contact());
        }
        [HttpPost]
        public IActionResult CreateContact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                dbContext.Contacts.Add(contact);
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Gửi thành công";
                return RedirectToAction("Index");
            }

            return View("Index", contact);
        }

    }
}
