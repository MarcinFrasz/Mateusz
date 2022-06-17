using DbManipulationApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{

    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = userManager.Users;
            return View(users);
        }

        public async Task<IActionResult> DeleteAsync(string? id)
        {
            if (id == null)
                return NotFound();
            var user = await userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string? id)
        {

            if (id == null)
            {
                TempData["error"] = $"User with Id = {id} cannot be found";
                return RedirectToAction("Index");
            }

            var user = await userManager.FindByIdAsync(id);

                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                TempData["success"] = "Pomyślnie usunięto rekord.";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                TempData["error"] = "Wystąpił błąd podczas próby usunięcia rekordu.";
                return RedirectToAction("Index");
                }

                return View("Index");

        }


        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["error"] = $"Użytkownik o Id = {id} nie został odnaleziony.";
                return NotFound();
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber=user.PhoneNumber,
                EmailConfirmed=user.EmailConfirmed,
               // Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EditUserModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                TempData["error"] = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.PhoneNumber = model.PhoneNumber;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["success"] = "Pomyślnie zmodyfikowano rekord.";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    TempData["error"] = "Wystąpił problem podczas zapisywania zmian.";
                    //return RedirectToAction("Index");
                    
                }

                return View(model);
            }
        }

        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["error"] = $"Użytkownik o Id = {userId} nie został odnaleziony.";
                return NotFound();
            }

            var model = new List<UserRolesModel>();

            foreach (var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult>
            ManageUserRoles(List<UserRolesModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["error"] = $"Użytkownik o Id = {userId} nie został odnaleziony.";
                return NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Nie można usunąć istniejących ról użytkownika.");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Nie można dodać wybranych roli użytkownikowi.");
                return View(model);
            }
            TempData["success"] = "Pomyślnie zmodyfikowano rekord.";
            return RedirectToAction("Index");
        }


    }
}
