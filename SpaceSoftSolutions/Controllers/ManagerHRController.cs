using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpaceSoftSolutions.Models;

namespace SpaceSoftSolutions.Controllers
{
    public class ManagerHRController : Controller
    {
        private readonly MyDbContext _context;

        public ManagerHRController(MyDbContext obj)
        {
            _context = obj;
        }
        public IActionResult Index()
        {

            return View();
        }


        public async Task<IActionResult> Manager() {

            var managers = _context.Managers.Include(m => m.Department).ToList();

            return View(managers);
        }
        public async Task<IActionResult> AddManager()
        {
            var department = await _context.Departments.ToListAsync();

            return View(department);
        }
        [HttpPost]
        public IActionResult AddManager(Manager manager)

        {
          
                _context.Managers.Add(manager);
                _context.SaveChanges();

            return RedirectToAction(nameof(AddManager));
        }

        public IActionResult Edit(int id)
        {
            var manager = _context.Managers.Include(m => m.Department).FirstOrDefault(m => m.Id == id); 

            if (manager == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _context.Departments.ToList();

            return View(manager);
        }
        [HttpPost]
        public IActionResult Edit(Manager manager)
        {

            _context.Managers.Update(manager);
            _context.SaveChanges();
            return RedirectToAction(nameof(Manager));
        }
        public IActionResult Delete(int id)
        {


            var man = _context.Managers.Include(m => m.Department).FirstOrDefault(m => m.Id == id);

            return View(man);
        }
        public IActionResult DeleteManager(int Id)
        {
            var m = _context.Managers.Find(Id);
            _context.Managers.Remove(m);
            _context.SaveChanges();

            return RedirectToAction(nameof(Manager));
        }

    }
}
