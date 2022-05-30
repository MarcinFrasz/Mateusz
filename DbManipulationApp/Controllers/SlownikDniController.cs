using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{
    public class SlownikDniController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
