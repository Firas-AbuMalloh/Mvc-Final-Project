using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class BaseHomeController : Controller
    {
        private readonly MyDbContext _context;

        public BaseHomeController(MyDbContext context)
        {
            _context = context;
        }



        public IActionResult Index()
        {

            return View();
        }
        public IActionResult About()
        {

            return View();
        }
        public IActionResult Portfolio()
        {
            return View();
        }
        public IActionResult PortfolioDetails()
        {
            return View();
        }
        public IActionResult Contact1()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.SubmittedAt = DateTime.Now;
                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();

                return Json(new { success = true, message = "Your message has been sent successfully!" });
            }

            return Json(new { success = false, message = "Invalid data. Please check your inputs." });
        }


        public IActionResult Department1()
        {
            var departments = _context.Departments.ToList();

            return View(departments);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
