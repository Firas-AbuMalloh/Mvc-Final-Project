using Microsoft.AspNetCore.Mvc;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class TTTT : Controller
    {
        private readonly MyDbContext _context;

        public TTTT(MyDbContext context)
        {
            _context = context;
        }

        
        public IActionResult emptaskk()
        {
            var tasks = _context.Tasks.ToList(); 
            return View(tasks); 
        }

        
        [HttpPost]
        public IActionResult emptaskk(int Id, string Status)
        {
            
            var task = _context.Tasks.FirstOrDefault(t => t.Id == Id);

            if (task != null)
            {
                task.Status = Status;  
                _context.SaveChanges();  
            }

            return RedirectToAction("emptaskk");  
        }

        public IActionResult vieeval(int Id)

        {

            var evaluations = _context.Evaluations
            .Where(e => e.EmployeeId == Id)
            .ToList();

            return View(evaluations);

        }
    }
}
