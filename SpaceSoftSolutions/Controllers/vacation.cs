using Microsoft.AspNetCore.Mvc;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class vacation : Controller
    {
        
            private readonly MyDbContext _V;

            public vacation(MyDbContext v)
            {
                _V = v;
            }

            public IActionResult Index()
            {

                var LeaveRequest = _V.LeaveRequests.ToList();
                return View(LeaveRequest);
            }


            public IActionResult Requestvaca(int Id)
            {
                ViewBag.empId = Id;
                var firas = _V.LeaveRequests.Find(Id);

                return View(firas);
            }

            [HttpPost]
            public IActionResult Requestvaca(LeaveRequest leaveRequest)
            {


            leaveRequest.Id = 0;
            leaveRequest.Status = "Pending";


                //leaveRequest.EmployeeId = Id;


                _V.LeaveRequests.Add(leaveRequest);
                _V.SaveChanges();


                return RedirectToAction("Index");
            }


        }

    }