using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{
    public class PatroniController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
