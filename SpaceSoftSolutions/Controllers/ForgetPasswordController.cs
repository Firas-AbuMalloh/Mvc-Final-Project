using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class ForgetPasswordController : Controller
    {
        private readonly MyDbContext _context;
        private readonly EmailService _emailService;
        public ForgetPasswordController(MyDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]

        public IActionResult ResetPassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            string userType = HttpContext.Session.GetString("UserType");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                ViewBag.Error = "Session expired. Please log in again.";
                return View();
            }

            if (userType == "Employee")
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Id == userId);

                if (employee == null || employee.PasswordHash != OldPassword)
                {
                    ViewBag.Error = "Invalid old password.";
                    return View();
                }
                if (NewPassword != ConfirmPassword)
                {
                    ViewBag.Error = "New password and confirm password do not match.";
                    return View();
                }

                employee.PasswordHash = NewPassword;
                _context.Employees.Update(employee);
                _context.SaveChanges();

                ViewBag.Success = "Password reset successfully!";
                return View();
            }

            if (userType == "Manager")
            {
                var manager = _context.Managers.FirstOrDefault(m => m.Id == userId);

                if (manager == null || manager.PasswordHash != OldPassword)
                {
                    ViewBag.Error = "Invalid old password.";
                    return View();
                }
                if (NewPassword != ConfirmPassword)
                {
                    ViewBag.Error = "New password and confirm password do not match.";
                    return View();
                }

                manager.PasswordHash = NewPassword;
                _context.Managers.Update(manager);
                _context.SaveChanges();

                ViewBag.Success = "Password reset successfully!";
                return View();
            }

            // HR User Type
            var hr = _context.Hrs.FirstOrDefault(h => h.Id == userId);

            if (hr == null || hr.PasswordHash != OldPassword)
            {
                ViewBag.Error = "Invalid old password.";
                return View();
            }
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "New password and confirm password do not match.";
                return View();
            }

            hr.PasswordHash = NewPassword;
            _context.Hrs.Update(hr);
            _context.SaveChanges();

            ViewBag.Success = "Password reset successfully!";
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string Email)
        {
            var Employee = _context.Employees.FirstOrDefault(e => e.Email == Email);
            if (Employee != null)
            {
                await _emailService.SendEmailAsync(Email, "Reset your password", $"https://localhost:44336/ForgetPassword/NewPassword/{Employee.Email}");
                return View();
            }

            var Manager = _context.Managers.FirstOrDefault(e => e.Email == Email);
            if (Manager != null)
            {
                await _emailService.SendEmailAsync(Email, "Reset your password", $"https://localhost:44336/ForgetPassword/NewPassword/{Manager.Email}");
                return View();
            }

            var Hr = _context.Hrs.FirstOrDefault(e => e.Email == Email);
            if (Hr != null)
            {
                await _emailService.SendEmailAsync(Email, "Reset your password", $"https://localhost:44336/ForgetPassword/NewPassword/{Hr.Email}");
                return View();
            }

            return View();

        }

        public IActionResult NewPassword()
        {

            return View();
        }

        // update the password after forget
        [HttpPost]
        [Route("Home/NewPassword/{id}")]
        public IActionResult NewPassword(string Password, string? id)
        {
            var Employee = _context.Employees.FirstOrDefault(e => e.Email == id );
            if (Employee != null)
            {
                Employee.PasswordHash = Password;
                _context.Employees.Update(Employee);
                _context.SaveChanges();
                return View();
            }

            var Manager = _context.Managers.FirstOrDefault(e => e.Email == id);
            if (Manager != null)
            {
                Manager.PasswordHash = Password;
                _context.Managers.Update(Manager);
                _context.SaveChanges();
                return View();
            }

            var Hr = _context.Hrs.FirstOrDefault(e => e.Email == id);
            if (Hr != null)
            {
                Hr.PasswordHash = Password;
                _context.Hrs.Update(Hr);
                _context.SaveChanges();
                return View();
            }
            return View();
        }

        public async Task<IActionResult> Department()
        {
            var deps = await _context.Departments.ToListAsync();
            return View(deps);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
