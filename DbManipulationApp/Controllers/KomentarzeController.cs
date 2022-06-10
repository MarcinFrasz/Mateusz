using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbManipulationApp.Controllers
{
    public class KomentarzeController : Controller
    {
        private readonly DbManipulationAppContext _db_identity;
        private readonly czytaniaContext _db_czytania;
        public KomentarzeController(DbManipulationAppContext db_identity, czytaniaContext db_czytania)
        {
            _db_identity = db_identity;
            _db_czytania = db_czytania;
        }
        public IActionResult Index()
        {

            ModelState.Clear();
            KomentarzeViewModel model = new();
            if (TempData["dzienlitrugiczny"] != null)
            {
                model.Komentarzes = _db_czytania.Komentarzes.Where(m => m.DzienLiturgiczny == (string)TempData["dzienliturgiczny"]);
            }
            IQueryable<dynamic>? querry_dzienliturgiczny;
            try
            {
                querry_dzienliturgiczny = _db_czytania.SlownikDnis.Select(m =>
                new { m.DzienLiturgiczny, m.NazwaDnia });
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                TempData["dzienLiturgiczny_list"] = new List<string>();
                return View(model);
            }
            List<string> dzienliturginczny_list = new();
            foreach (var item in querry_dzienliturgiczny)
            {
                dzienliturginczny_list.Add(item.DzienLiturgiczny + "|" + item.NazwaDnia);
            }

            ViewData["dzienLiturgiczny_list"] = dzienliturginczny_list;
            if (TempData["dzienliturgiczny"] != null)
            {
                model.DzienLiturgiczny = (string)TempData["dzienliturgiczny"];
                try
                {
                    model.Komentarzes = _db_czytania.Komentarzes.Where(m => m.DzienLiturgiczny == model.DzienLiturgiczny);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                }
                return View(model);
            }
            try
            {
                model.Komentarzes = _db_czytania.Komentarzes;
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
            IEnumerable<Komentarze> model;
            if (day != null)
            {
                var picked_day = day.Split('|')[0];
                try
                {
                    model = _db_czytania.Komentarzes.Where(m => m.DzienLiturgiczny == picked_day);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas ładowania danych.";
                    return RedirectToAction("Index");
                }
                return PartialView("_KomentarzeDisplay", model);
            }
            return PartialView("_KomentarzeDisplay", Enumerable.Empty<Komentarze>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDayPicked(string? day)
        {
            if (day != null)
            {
                IQueryable<Komentarze>? querry_search;
                try
                {
                    querry_search =
                        from item in _db_czytania.Komentarzes
                        where EF.Functions.Like(item.DzienLiturgiczny, day + "%")
                        select item;
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wyszukiwania.";
                    return PartialView("_KomentarzeDisplay", Enumerable.Empty<SlownikDni>());
                }
                return PartialView("_KomentarzeDisplay", querry_search);
            }
            return PartialView("_KomentarzeDisplay", Enumerable.Empty<Komentarze>());
        }
    }
}
