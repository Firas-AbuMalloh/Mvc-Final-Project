using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SpaceSoftSolutions.Models;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SpaceSoftSolutions.Controllers
{
    public class HRController : Controller
    {
        private readonly MyDbContext _context;

        public HRController(MyDbContext context)
        {
            _context = context;
        }

        // ✅ دالة للتحقق مما إذا كان المستخدم مسجلاً للدخول
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }



        // ✅ تعيين مدير على قسم معين
        [HttpPost]
        public IActionResult AssignManager(int managerId, int departmentId)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var manager = _context.Managers.Find(managerId);
            if (manager != null)
            {
                manager.Id = departmentId;
                _context.SaveChanges();
            }
            return RedirectToAction("Managers");
        }

        // ✅ عرض جميع الأقسام
        public IActionResult Departments(string search)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var departments = _context.Departments
                                      .Include(d => d.Managers)
                                      .Include(d => d.Employees)
                                      .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                departments = departments.Where(d => d.Name.Contains(search));
            }

            return View(departments.ToList());
        }


        [HttpGet]
        public IActionResult AddDepartment()
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult AddDepartment(Department model)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Departments.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Departments");
            }
            return View(model);
        }


        // ✅ تعديل بيانات القسم
        [HttpGet]
        public IActionResult EditDepartment(int id)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var department = _context.Departments
                .Include(d => d.Managers) // ✅ تحميل المديرين
                .FirstOrDefault(d => d.Id == id);

            if (department == null) return NotFound();

            ViewBag.Managers = _context.Managers.ToList(); // ✅ تحميل قائمة المديرين في `ViewBag`

            return View(department);
        }


        [HttpPost]
        public IActionResult EditDepartment(int id, string name, string description, int? managerId)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var department = _context.Departments
                .Include(d => d.Managers) // ✅ تحميل المديرين الحاليين
                .FirstOrDefault(d => d.Id == id);

            if (department == null) return NotFound();

            department.Name = name;
            department.Description = description;

            if (managerId.HasValue)
            {
                var newManager = _context.Managers.FirstOrDefault(m => m.Id == managerId);
                if (newManager != null)
                {
                    department.Managers.Clear(); // ✅ حذف المدير الحالي (إفراغ القائمة)
                    department.Managers.Add(newManager); // ✅ تعيين المدير الجديد
                }
            }
            else
            {
                department.Managers.Clear(); // ✅ إزالة المدير إن لم يتم تحديد أي مدير جديد
            }

            _context.SaveChanges();
            return RedirectToAction("Departments");
        }


        // ✅ حذف قسم
        [HttpPost]
        public IActionResult DeleteDepartment(int id)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var department = _context.Departments.Find(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
            return RedirectToAction("Departments");
        }



        public IActionResult DepartmentDetails(int id)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var department = _context.Departments
                                     .Include(d => d.Managers)
                                     .Include(d => d.Employees)
                                     .FirstOrDefault(d => d.Id == id);

            if (department == null) return NotFound();

            return View(department);
        }



        public IActionResult Employees(string search, int? departmentId)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            // تحميل بيانات الموظفين مع معلومات القسم
            var employees = _context.Employees
                                    .Include(e => e.Department) // تضمين بيانات القسم
                                    .AsQueryable(); // تجهيز الاستعلام للفلترة

            // البحث عن الموظفين بالاسم أو البريد
            if (!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(e => e.Name.Contains(search) || e.Email.Contains(search));
            }

            // فلترة حسب القسم إذا تم تحديده
            if (departmentId.HasValue)
            {
                employees = employees.Where(e => e.DepartmentId == departmentId);
            }

            // جلب قائمة الأقسام بدون تكرار
            var departments = _context.Departments
                                      .OrderBy(d => d.Name)
                                      .ToList();

            // تمرير قائمة الأقسام إلى View
            ViewBag.Departments = departments;

            return View(employees.ToList());
        }



        public IActionResult Details(int employeeId)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var employee = _context.Employees
                .Include(e => e.Department) // إذا أردت تضمين قسم الموظف
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null)
            {
                return NotFound(); // إذا لم يتم العثور على الموظف
            }

            return View(employee);
        }


        public IActionResult ApprovedLeaves(string search)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var leaves = _context.LeaveRequests
                                 .Where(l => l.Status == "Approved")
                                 .Include(l => l.Employee)
                                 .AsQueryable();

            // ✅ إضافة بحث إذا كان هناك قيمة في الـ search
            if (!string.IsNullOrEmpty(search))
            {
                leaves = leaves.Where(l => l.Employee.Name.Contains(search) || l.Employee.Email.Contains(search));
            }

            ViewData["SearchQuery"] = search;

            return View(leaves.ToList());
        }






        [HttpPost]
        public IActionResult ViewLeaveDetails(int leaveId)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            // جلب بيانات طلب الإجازة
            var leave = _context.LeaveRequests
                                .Include(l => l.Employee) // التأكد من تحميل بيانات الموظف
                                .FirstOrDefault(l => l.Id == leaveId);

            // التحقق من وجود الطلب
            if (leave == null)
            {
                ViewBag.Message = "Leave request not found!";
                return RedirectToAction("ApprovedLeaves");
            }

            // عرض التفاصيل في صفحة منفصلة
            return View(leave);
        }






        // ✅ عرض جميع الرسائل من صفحة الفيدباك أو الكونتاكت
        public IActionResult FeedbackMessages()
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            var feedbacks = _context.Feedbacks.ToList();  // جلب كافة الرسائل من الـ DB
            return View(feedbacks);
        }


        public IActionResult ExportToPDF(string reportType, string search = "", int? departmentId = null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 30, 30, 30, 30); // تعيين هوامش محسنة
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // ✅ تخصيص الخطوط والألوان
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.WHITE);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE);
                BaseColor headerColor = new BaseColor(41, 128, 185); // أزرق فاتح
                BaseColor borderColor = new BaseColor(52, 73, 94);  // رمادي غامق
                BaseColor backgroundColor = new BaseColor(236, 240, 241); // رمادي فاتح

                // ✅ إضافة عنوان التقرير بشكل جذاب
                Paragraph title = new Paragraph($"{reportType.ToUpper()} REPORT\n\n", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 25
                };
                document.Add(title);

                PdfPTable table;

                switch (reportType.ToLower())
                {
                    case "employees":
                        var employees = _context.Employees.Include(e => e.Department).AsQueryable();
                        if (!string.IsNullOrEmpty(search))
                            employees = employees.Where(e => e.Name.Contains(search) || e.Email.Contains(search));
                        if (departmentId.HasValue)
                            employees = employees.Where(e => e.DepartmentId == departmentId);

                        table = CreateStyledTable(5);
                        AddTableHeader(table, new string[] { "Name", "Email", "Position", "Department", "Address" }, headerFont, headerColor);

                        foreach (var employee in employees.ToList())
                        {
                            AddTableRow(table, new string[]
                            {
                        employee.Name,
                        employee.Email,
                        employee.Position,
                        employee.Department?.Name ?? "No Department",
                        employee.Address ?? "Not Provided"
                            }, normalFont, borderColor);
                        }
                        document.Add(table);
                        break;


                    case "evaluations":
                        var evaluations = _context.Evaluations
                            .Include(e => e.Employee)
                            .Include(e => e.Manager)
                            .ToList();

                        if (!evaluations.Any()) // ✅ منع إنشاء ملف PDF فارغ
                        {
                            document.Add(new Paragraph("No evaluations available.", normalFont));
                            document.Close();
                            return File(stream.ToArray(), "application/pdf", "Evaluations_Report.pdf");
                        }

                        table = new PdfPTable(4)
                        {
                            WidthPercentage = 100
                        };
                        table.SetWidths(new float[] { 3, 3, 2, 4 });

                        // ✅ إضافة رؤوس الأعمدة مع تحسين التنسيق
                        AddTableHeader(table, new string[] { "Employee", "Manager", "Score", "Comments" }, headerFont, headerColor);

                        foreach (var evaluation in evaluations)
                        {
                            AddTableRow(table, new string[]
                            {
                        evaluation.Employee?.Name ?? "Unknown",
                        evaluation.Manager?.Name ?? "Unknown",
                        evaluation.Score,
                        evaluation.Comments ?? "No Comments"
                            }, normalFont, borderColor);
                        }

                        document.Add(table);
                        break;

                    case "departments":
                        document.Add(new Paragraph("Departments Report", titleFont));
                        document.Add(new Paragraph("\n"));

                        // ✅ إنشاء جدول للأقسام
                        PdfPTable departmentTable = CreateStyledTable(4);
                        AddTableHeader(departmentTable, new string[] { "Department Name", "Description", "Manager", "Employees" }, headerFont, headerColor);

                        var departments = _context.Departments.Include(d => d.Managers).Include(d => d.Employees).ToList();

                        foreach (var department in departments)
                        {
                            string managerName = department.Managers.Any() ? department.Managers.First().Name : "No Manager Assigned";
                            string employeesList = department.Employees.Any()
                                ? string.Join(", ", department.Employees.Select(e => e.Name))
                                : "No Employees";

                            AddTableRow(departmentTable, new string[]
                            {
                        department.Name,
                        department.Description ?? "No Description",
                        managerName,
                        employeesList
                            }, normalFont, borderColor);
                        }

                        document.Add(departmentTable);
                        break;

                    case "feedbacks":
                        var feedbacks = _context.Feedbacks.ToList();
                        table = CreateStyledTable(3);
                        AddTableHeader(table, new string[] { "Name", "Email", "Message" }, headerFont, headerColor);

                        foreach (var feedback in feedbacks)
                        {
                            AddTableRow(table, new string[] { feedback.Name, feedback.Email, feedback.Message }, normalFont, borderColor);
                        }
                        document.Add(table);
                        break;

                    case "leaves":
                        var leaves = _context.LeaveRequests.Include(l => l.Employee).Include(l => l.Employee.Department).ToList();
                        table = CreateStyledTable(5);
                        AddTableHeader(table, new string[] { "Employee", "Start Date", "End Date", "Reason", "Department" }, headerFont, headerColor);

                        foreach (var leave in leaves)
                        {
                            AddTableRow(table, new string[]
                            {
                        leave.Employee.Name,
                        leave.StartDate.ToString("yyyy-MM-dd"),
                        leave.EndDate.ToString("yyyy-MM-dd"),
                        leave.Reason ?? "No Reason",
                        leave.Employee.Department?.Name ?? "No Department"
                            }, normalFont, borderColor);
                        }
                        document.Add(table);
                        break;


                    default:
                        document.Add(new Paragraph("Invalid Report Type", titleFont));
                        break;
                }

                document.Close();
                return File(stream.ToArray(), "application/pdf", $"{reportType}_Report.pdf");
            }
        }

        // ✅ دالة لإنشاء الجداول بتنسيق محسّن
        private PdfPTable CreateStyledTable(int columnCount)
        {
            PdfPTable table = new PdfPTable(columnCount) { WidthPercentage = 100 };
            table.SetWidths(Enumerable.Repeat(4f, columnCount).ToArray());
            return table;
        }

        // ✅ دالة لإضافة رؤوس الجداول
        private void AddTableHeader(PdfPTable table, string[] headers, Font font, BaseColor backgroundColor)
        {
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, font))
                {
                    BackgroundColor = backgroundColor,
                    Padding = 8,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);
            }
        }

        // ✅ دالة لإضافة بيانات إلى الجدول
        private void AddTableRow(PdfPTable table, string[] values, Font font, BaseColor borderColor)
        {
            foreach (var value in values)
            {
                PdfPCell cell = new PdfPCell(new Phrase(value, font))
                {
                    Padding = 8,
                    BorderColor = borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);
            }
        }

        // ✅ عرض جميع التقييمات التي تمت من قبل المدراء
        public IActionResult Evaluations(string searchName, string scoreFilter)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            // جلب جميع التقييمات
            List<Evaluation> evaluations = _context.Evaluations
                                                   .Include(e => e.Employee)
                                                   .Include(e => e.Manager)
                                                   .ToList();

            // البحث عن التقييمات بناءً على اسم الموظف
            if (!string.IsNullOrEmpty(searchName))
            {
                evaluations = evaluations.Where(e => e.Employee.Name.Contains(searchName)).ToList();
            }

            // الفلترة حسب التقييم (Score) إذا تم اختياره
            if (!string.IsNullOrEmpty(scoreFilter))
            {
                evaluations = evaluations.Where(e => e.Score == scoreFilter).ToList();
            }

            return View(evaluations);
        }


    }
}
