using Microsoft.AspNetCore.Mvc;

namespace SpaceSoftSolutions.Controllers
{
    public class HomeffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
