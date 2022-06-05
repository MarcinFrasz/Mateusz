using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbManipulationApp.Controllers
{
    public class LekcjonarzController : Controller
    {
        private readonly DbManipulationAppContext _db_identity;
        private readonly czytaniaContext _db_czytania;
        public LekcjonarzController(DbManipulationAppContext db_identity, czytaniaContext db_czytania)
        {
            _db_identity = db_identity;
            _db_czytania = db_czytania;
        }
        public IActionResult Index()
        {
            ModelState.Clear();

            var querry_typczytania = _db_czytania.SlownikDnis.Select(m =>
            new { m.DzienLiturgiczny, m.NazwaDnia });
            List<string> dzienliturginczny_list = new List<string>();
            foreach (var item in querry_typczytania)
            {
                dzienliturginczny_list.Add(item.DzienLiturgiczny + "|" + item.NazwaDnia);
            }

            ViewData["dzienLiturgiczny_list"] = dzienliturginczny_list;
            LekcjonarzViewModel model = new();
            if (TempData["dzienliturgiczny"] != null)
            {
                model.DzienLiturgiczny = (string)TempData["dzienliturgiczny"];
                try
                {
                    model.Lekcjonarze = _db_czytania.Lekcjonarzs.Where(m => m.DzienLiturgiczny == model.DzienLiturgiczny);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                }
                return View(model);
            }
            try
            {
                model.Lekcjonarze = _db_czytania.Lekcjonarzs;
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDaySelected(string? day)
        {
            IEnumerable<Lekcjonarz> model;
            if(day!=null)
            {
                var str=day.Split('|');
                var picked_day = str[0];
                try
                {
                   model=_db_czytania.Lekcjonarzs.Where(m => m.DzienLiturgiczny == picked_day);
                }
                catch(Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas ładowania danych.";
                    return RedirectToAction("Index");
                }
                return PartialView("_LekcjonarzDisplay", model);
            }
            return PartialView("_LekcjonarzDisplay", Enumerable.Empty<Lekcjonarz>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDayPicked(string? day)
        {
            if (day != null)
            {
                IQueryable<Lekcjonarz>? querry_search;
                try
                {
                    querry_search =
                        from item in _db_czytania.Lekcjonarzs
                        where EF.Functions.Like(item.DzienLiturgiczny, day+"%")
                    select item;
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wyszukiwania.";
                    return PartialView("_LekcjonarzDisplay", Enumerable.Empty<SlownikDni>());
                }
                return PartialView("_LekcjonarzDisplay", querry_search);
            }
            return PartialView("_LekcjonarzDisplay", Enumerable.Empty<Lekcjonarz>());
        }
    }
}
