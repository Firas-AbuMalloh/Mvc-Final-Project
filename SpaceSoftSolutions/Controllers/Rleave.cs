using Microsoft.AspNetCore.Mvc;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class Rleave : Controller
    {
        private readonly MyDbContext _R;

        public Rleave(MyDbContext r)
        {
            _R = r;
        }

        public IActionResult Index()
        {

            var leaveRequests = _R.HourlyLeaves.ToList();
            return View(leaveRequests);
        }


        public IActionResult RequestLeave(int Id)
        {
            ViewBag.empId = Id;
            var firas = _R.HourlyLeaves.Find(Id);

            return View(firas);
        }

        [HttpPost]
        public IActionResult RequestLeave(HourlyLeave hourlyLeave)
        {


            hourlyLeave.Id = 0;
            hourlyLeave.Status = "Pending";


            //leaveRequest.EmployeeId = Id;


            _R.HourlyLeaves.Add(hourlyLeave);
            _R.SaveChanges();


            return RedirectToAction("Index");
        }


    }

}