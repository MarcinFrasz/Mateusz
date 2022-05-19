using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{
    public class DataOperationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
