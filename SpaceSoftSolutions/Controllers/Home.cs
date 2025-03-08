using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class Home : Controller
    {

        private readonly MyDbContext _logger;

        public Home(MyDbContext logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            int Id = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            var s = _logger.Employees.FirstOrDefault(x => x.Id == Id);
            return View(s);
        }
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult Create(Employee employee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _logger.Employees.Add(employee);
        //        _logger.SaveChanges();
        //        return RedirectToAction("Index");

        //    }
        //    return View();
        //}
        //public IActionResult Privacy()
        //{
        //    return View();
        //}


    }
}
