using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{
    public class KomentarzeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
