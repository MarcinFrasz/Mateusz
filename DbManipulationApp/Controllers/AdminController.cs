using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = userManager.Users;
            return View(users);
        }
    }
}
