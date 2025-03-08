using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpaceSoftSolutions.Models;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class ManagersController : Controller
    {
        private readonly MyDbContext _context;
        private readonly EmailService _emailService;

        public ManagersController(MyDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
   

        public IActionResult ShowEmployees(int Id)
        {

            //var employees = _context.Employees.Where(e => e.ManagerId == Id).ToList();
            var employees = _context.Employees.Where(e => e.ManagerId == 1).ToList(); //test
            ViewBag.Departments = _context.Departments.Where(dep => employees.Select(e => e.DepartmentId).Contains(dep.Id)).ToList();
            return View(employees);
        }


        public IActionResult CreateEmp(int id)
        {
            ViewBag.ManagerId = id;
            //ViewBag.Department = new SelectList(_context.Departments.Where(Department => Department.Id == 1).ToList(), "Id", "Name");
            ViewBag.Department = new SelectList(_context.Departments.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        //public IActionResult CreateEmp(Employee employee, int ManagerId)
        //{
        //        if (ModelState.IsValid)
        //        {
        //            employee.Id = 0;  // Ensures it's a new record
        //        employee.ManagerId = 1;//test
        //        employee.ManagerId = ManagerId;  // Assigning ManagerId correctly

        //        // Check if ManagerId exists in Managers table before inserting
        //        var managerExists = _context.Managers.Any(m => m.Id == 2); //testing
        //        //var managerExists = _context.Managers.Any(m => m.Id == ManagerId);
        //        if (!managerExists)
        //            {
        //                ModelState.AddModelError("ManagerId", "Selected Manager does not exist.");
        //                return View(employee);
        //            }

        //            _context.Employees.Add(employee);
        //            _context.SaveChanges();
        //            return RedirectToAction("Index", "Home");
        //        }

        //    ViewBag.ManagerId = 1; //test
        //    //ViewBag.ManagerId = ManagerId;  // Maintain ManagerId in case of validation errors
        //        return View(employee);
        //    }


        //}
        //if (ModelState.IsValid)
        //{
        //    employee.Id = 0;
        //    _context.Employees.Add(employee);
        //    _context.SaveChanges();
        //    return RedirectToAction("Index", "Home");
        //}
        //else
        //{
        //    ViewBag.ManagerId = employee.Id;
        //    return View();
        //}

        [HttpPost]
        public IActionResult CreateEmp(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.Id = 0;  // Ensures it's a new record
                employee.ManagerId = 1;  // Set to a static ManagerId for testing

                // Ensure you have valid values for required fields
                if (employee.Name != null && employee.Email != null)
                {
                    _context.Employees.Add(employee);  // Add the employee to the DbContext
                    _context.SaveChanges();  // Commit the transaction to the database
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Optional: Add validation error if employee fields are invalid
                    ModelState.AddModelError("", "Please fill all required fields.");
                    return View(employee);
                }
            }

            return View(employee);  // Return the view if the model is not valid
        }



        public IActionResult ShowTasks(int id)
        {
            var tasks = _context.Tasks.Where(t => t.EmployeeId == id).ToList();
            return View(tasks);
        }



        public IActionResult ShowAttendance(int id)
        {
            var attendanceData = _context.Attendances.Where(a => a.EmployeeId == id).ToList();
            return View(attendanceData);
        }

        //public IActionResult DeleteTask(int id) 
        //{
        //    var task = _context.Tasks.Find(id);
        //    _context.Remove(task);
        //    _context.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        public IActionResult DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return RedirectToAction("ShowTasks", new { id = task.EmployeeId });
        }

        // Display leave request form


        //[HttpPost]
        //public async Task<IActionResult> ForgetPassword(string Email)
        //{
        //    var Employee = _context.Employees.FirstOrDefault(e => e.Email == Email);
        //    if (Employee != null)
        //    {
        //        await _emailService.SendEmailAsync(Email, "Reset your password", $"https://localhost:7283/Home/NewPassword/%7BEmployee.Id%7D");
        //        return View();
        //    }

        //    var Manager = _context.Managers.FirstOrDefault(e => e.Email == Email);
        //    if (Manager != null)
        //    {
        //        await _emailService.SendEmailAsync(Email, "Reset your password", $"https://localhost:7283/Home/NewPassword/%7BManager.Id%7D");
        //        return View();
        //    }

        //    var Hr = _context.Hrs.FirstOrDefault(e => e.Email == Email);
        //    if (Hr != null)
        //    {
        //        await _emailService.SendEmailAsync(Email, "Reset your password", $"https://localhost:7283/Home/NewPassword/%7BHr.Id%7D");
        //        return View();
        //    }

        //    return View();

        //}
        public IActionResult Index()
        {
            // قراءة بيانات المدراء وتجنب التكرار
            var managers = _context.Managers
                                      .GroupBy(d => d.Name)
                                      .Select(g => g.First())
                                      .ToList();

            // قراءة بيانات الأقسام وتجنب التكرار
            var departments = _context.Departments
                                      .GroupBy(d => d.Name)
                                      .Select(g => g.First())
                                      .ToList();

            ViewBag.Managers = managers;
            ViewBag.Departments = departments;



            var employees = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .ToList();

            return View(employees);
        }



        [HttpPost]
        public IActionResult AddEmployee(string Name, string Password, string Email, string Position, int DepartmentId, int? ManagerId)
        {
            if (ModelState.IsValid)
            {
                var newEmployee = new Employee
                {
                    Name = Name,
                    Email = Email,
                    PasswordHash = Password,
                    Position = Position,
                    DepartmentId = DepartmentId,
                    ManagerId = ManagerId

                };

                _context.Employees.Add(newEmployee);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Managers = _context.Employees.Where(e => e.Position == "Manager").ToList();
            return View();
        }

        public IActionResult Tasks()
        {
            var Employee = _context.Employees
                                      .GroupBy(d => d.Name)
                                      .Select(g => g.First())
                                      .ToList();
            ViewBag.Employees = Employee;
            var tasks = _context.Tasks
                                .Include(t => t.Employee)
                                .ToList();

            if (tasks == null || !tasks.Any())
            {
                return NotFound();
            }

            return View(tasks);
        }
        [HttpPost]
        public async Task <IActionResult> AddTask(string TaskName, string Description, DateOnly StartDate, DateOnly EndDate, string Status, int EmployeeId)
        {

            if (ModelState.IsValid)
            {
                var newTask = new SpaceSoftSolutions.Models.Task
                {
                    TaskName = TaskName,
                    Description = Description,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Status = Status,
                    EmployeeId = EmployeeId

                };

                _context.Tasks.Add(newTask);
                _context.SaveChanges();

                var emp = _context.Employees.Find(newTask.EmployeeId);
                await _emailService.SendEmailAsync(emp.Email, "New Task", $"<p><b>task title:</b> {newTask.TaskName}<br>" +
                        $"<b>task Discription:</b> {newTask.Description}<br></p>" +
                        $"<b>Start Date :</b> {newTask.StartDate}<br></p>" +
                        $"<b>End Date:</b> {newTask.EndDate}<br></p>");

                return RedirectToAction("Tasks");
            }

            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Managers = _context.Employees.Where(e => e.Position == "Manager").ToList();
            return View();
        }


        public IActionResult Leaves()
        {
            var leaveRequests = _context.LeaveRequests
                .Include(r => r.Employee)
                .ToList();
            return View(leaveRequests);
        }


        [HttpPost]
        public JsonResult UpdateStatus(int leaveRequestId, string decision)
        {
            var leaveRequest = _context.LeaveRequests.FirstOrDefault(lr => lr.Id == leaveRequestId);

            if (leaveRequest != null)
            {
                if (decision == "approve")
                {
                    leaveRequest.Status = "Approved";
                }
                else if (decision == "reject")
                {
                    leaveRequest.Status = "Rejected";
                }

                _context.SaveChanges();
            }

            return Json(new { success = true });
        }
        public IActionResult Evaluation()
        {
            var managers = _context.Managers
                                   .Select(m => new { m.Id, m.Name })
                                   .Distinct()
                                   .ToList();
            ViewBag.Managers = managers;


            var employees = _context.Employees
                        .Select(e => new { e.Id, e.Name })
                        .Distinct()
                        .ToList();

            ViewBag.Employees = employees;



            return View();
        }

        [HttpPost]
        public IActionResult SubmitEvaluation(EvaluationViewModel model)
        {


            var scores = new List<int>
    {
        ConvertScore(model.Question1),
        ConvertScore(model.Question2),
        ConvertScore(model.Question3),
        ConvertScore(model.Question4),
        ConvertScore(model.Question5),
        ConvertScore(model.Question6),
        ConvertScore(model.Question7),
        ConvertScore(model.Question8),
        ConvertScore(model.Question9),
        ConvertScore(model.Question10)
    };

            double averageScore = scores.Average();

            string scoreValue = "";
            if (averageScore >= 4)
            {
                scoreValue = "excellent";
            }
            else if (averageScore >= 3.5)
            {
                scoreValue = "very good";
            }
            else if (averageScore >= 3)
            {
                scoreValue = "good";
            }
            else
            {
                scoreValue = "fair";
            }

            var evaluation = new Evaluation
            {
                EmployeeId = model.EmployeeId,
                ManagerId = model.ManagerId,
                Score = scoreValue,
                Comments = model.Comments,
                DateEvaluated = DateOnly.FromDateTime(DateTime.Now)
            };
            _context.Evaluations.Add(evaluation);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Evaluation Submitted Successfully!";

            return RedirectToAction("ShowEvaluation");
        }
        [HttpPost]
        public IActionResult ClearSuccessMessage()
        {
            TempData.Remove("SuccessMessage");
            return Ok();
        }

        private int ConvertScore(string answer)
        {
            return answer switch
            {
                "excellent" => 4,
                "very_good" => 3,
                "good" => 2,
                "fair" => 1,
                _ => 0
            };
        }

        public IActionResult ShowEvaluation()
        {
            var evaluations = _context.Evaluations
                .Include(e => e.Employee)
                .Include(e => e.Employee.Department)
                .Include(e => e.Employee.Manager)
                .ToList();

            return View(evaluations);
        }
        public IActionResult ExportEvaluationsCSV()
        {
            var evaluations = _context.Evaluations
                .Include(e => e.Employee)
                .Include(e => e.Employee.Department)
                .Include(e => e.Employee.Manager)
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Employee Name,Manager Name,Score,Comments,Date Evaluated,Department");

            foreach (var eval in evaluations)
            {
                csv.AppendLine($"{eval.Employee?.Name ?? "N/A"},{eval.Manager?.Name ?? "N/A"},{eval.Score},{eval.Comments},{eval.DateEvaluated:yyyy-MM-dd},{eval.Employee?.Department?.Name ?? "N/A"}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "Evaluations.csv");
        }
        [HttpPost]
        public IActionResult SaveTask(int taskId, string taskName, string description, DateTime startDate, DateTime endDate)
        {
            var task = _context.Tasks.Find(taskId);
            if (task == null)
            {
                return NotFound();
            }

            //task.TaskName = taskName;
            //task.Description = description;


            _context.SaveChanges();

            return RedirectToAction("Tasks");
        }
        public IActionResult EditTask(int id)
        {
            var task = _context.Tasks
                               .Include(t => t.Employee)
                               .FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            ViewBag.Employees = new SelectList(_context.Employees, "Id", "Name", task.EmployeeId);
            return View(task);
        }
        [HttpPost]
        public IActionResult EditTask(SpaceSoftSolutions.Models.Task task)
        {
            var existingTask = _context.Tasks.FirstOrDefault(t => t.Id == task.Id);

            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.TaskName = task.TaskName;
            existingTask.Description = task.Description;
            existingTask.StartDate = task.StartDate;
            existingTask.EndDate = task.EndDate;
            existingTask.EmployeeId = task.EmployeeId;
            existingTask.Status = task.Status;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Task updated successfully!";
            return RedirectToAction("tasks");
        }

        public IActionResult DeleteTask1(int id)
        {
            var task = _context.Tasks.Find(id);
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Task deleted successfully!";
            return RedirectToAction("tasks");


        }
        public IActionResult TaskDetails(int id)
        {
            var employee = _context.Employees
                .Include(e => e.Tasks)
                .FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        public IActionResult Attendance(int id)
        {
            var attendance = _context.Attendances
                                      .Where(a => a.EmployeeId == id)
                                      .Include(a => a.Employee)
                                      .ToList();

            if (attendance == null || !attendance.Any())
            {
                return View("Attendance", new List<SpaceSoftSolutions.Models.Attendance>());
            }

            return View(attendance);
        }


    }

}