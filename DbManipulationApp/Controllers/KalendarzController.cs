using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{
    public class KalendarzController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
