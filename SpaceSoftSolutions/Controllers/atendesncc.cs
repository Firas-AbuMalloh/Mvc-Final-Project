using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class atendesncc : Controller
    {
        private readonly MyDbContext _a;
        public atendesncc(MyDbContext a)
        {
            _a = a;
        }


        public IActionResult Indexx()
        {
            int Id = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            var attendanceRecords = _a.Attendances
                .Where(a => a.EmployeeId == Id)
                .OrderByDescending(a => a.PunchIn)
                .ToList();

            ViewBag.EmployeeId = Id;
            return View(attendanceRecords);
        }
        public IActionResult PunchIn(int Id)
        {
            var attendance = new Attendance
            {
                EmployeeId = Id,
                PunchIn = DateTime.Now
            };
            _a.Attendances.Add(attendance);
            _a.SaveChanges();
            HttpContext.Session.SetInt32("PunchInEmployeeId", Id);


            return RedirectToAction("Indexx");
        }
        public IActionResult PunchOut(int Id)
        {

            var attendance = _a.Attendances
           .FirstOrDefault(a => a.EmployeeId == Id && a.PunchOut == null);

            if (attendance != null)
            {

                if (attendance.EmployeeId == Id)
                {

                    attendance.PunchOut = DateTime.Now;


                    _a.SaveChanges();
                }
            }


            return RedirectToAction("Indexx");
        }
        //public IActionResult PunchOut(int Id,int EmployeeId)
        //{
        //    var attendance = _a.Attendances
        //        .FirstOrDefault(a => a.EmployeeId == EmployeeId && a.Id==Id && a.PunchOut == null);

        //    if (attendance != null)
        //    {
        //        attendance.PunchOut = DateTime.Now;
        //        _a.SaveChanges();
        //    }

        //    return RedirectToAction("Indexx"); // العودة إلى قائمة الحضور
        //}

        //public IActionResult PunchOut(int EmployeeId)
        //{
        //    var attendance = _a.Attendances
        //        .Where(a => a.EmployeeId == EmployeeId && a.PunchOut == null)
        //          .FirstOrDefault();

        //    if (attendance != null)
        //    {
        //        attendance.PunchOut = DateTime.Now; // تعيين وقت الانصراف إلى الوقت الحالي
        //        _a.SaveChanges();  // حفظ التغييرات في قاعدة البيانات
        //    }

        //    return RedirectToAction("Dashboard", "Employee");
        //}



    }
}
