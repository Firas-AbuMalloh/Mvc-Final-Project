using Microsoft.AspNetCore.Mvc;
using SpaceSoftSolutions.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
namespace SpaceSoftSolutions.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MyDbContext _context;
        public ProfileController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId").Value;
            var userType = HttpContext.Session.GetString("UserType");

            if (userType == "HR")
            {
                var hrUser = _context.Hrs.Find(userId);
                return View(hrUser);
            }
            else if (userType == "Manager")
            {
                var managerUser = _context.Managers
                                          .Include(m => m.Department)
                                          .FirstOrDefault(m => m.Id == userId);
                return View(managerUser);
            }
            else if (userType == "Employee")
            {
                var employeeUser = _context.Employees
                                           .Include(e => e.Department)
                                           .Include(e => e.Manager)
                                           .FirstOrDefault(e => e.Id == userId);
                return View(employeeUser);
            }

            return RedirectToAction("Login");
        }

        // ✅ صفحة تعديل الملف الشخصي
        public IActionResult EditProfile()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId").Value;
            var userType = HttpContext.Session.GetString("UserType");

            if (userType == "HR")
            {
                var hrUser = _context.Hrs.Find(userId);
                return View(hrUser);
            }
            else if (userType == "Manager")
            {
                var managerUser = _context.Managers.Find(userId);
                return View(managerUser);
            }
            else if (userType == "Employee")
            {
                var employeeUser = _context.Employees.Find(userId);
                return View(employeeUser);
            }

            return RedirectToAction("Login");
        }

        // ✅ حفظ التعديلات على الملف الشخصي
        [HttpPost]
        public IActionResult EditProfile(string name, string email, string address, IFormFile? profileImage)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId").Value;
            var userType = HttpContext.Session.GetString("UserType");

            dynamic user = null;

            if (userType == "HR")
            {
                user = _context.Hrs.Find(userId);
            }
            else if (userType == "Manager")
            {
                user = _context.Managers.Find(userId);
            }
            else if (userType == "Employee")
            {
                user = _context.Employees.Find(userId);
            }

            if (user == null)
            {
                return NotFound();
            }

            // ✅ تحديث البيانات
            user.Name = name;
            user.Email = email;
            user.Address = address;

            // ✅ حفظ الصورة إذا تم تحميلها
            if (profileImage != null && profileImage.Length > 0)
            {
                string filePath = "/images/profiles/" + profileImage.FileName;
                using (var stream = new FileStream("wwwroot" + filePath, FileMode.Create))
                {
                    profileImage.CopyTo(stream);
                }
                user.ImagePath = filePath;
            }

            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
    }
}
