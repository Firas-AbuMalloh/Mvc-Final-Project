using Microsoft.AspNetCore.Mvc;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly MyDbContext _context;
        public ContactUsController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult ContactUs()
        {



            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult HandelContactUs(Feedback model)
        {
            if (ModelState.IsValid)
            {
                model.SubmittedAt = DateTime.Now;  // تخزين وقت الإرسال
                _context.Feedbacks.Add(model);       // إضافة البيانات للجدول Feedback
                _context.SaveChanges();             // حفظ التغييرات بالداتابيس
                return Json(new { success = true, message = "Sent successfully!" });
            }
            return Json(new { success = false, message = "An error occurred while sending!" });
        }
        public IActionResult Success()
        {
            return View();  // صفحة بسيطة تعرض رسالة نجاح
        }
    }
}
