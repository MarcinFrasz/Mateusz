using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DbManipulationApp.Controllers
{
    public class SlownikDniController : Controller
    {

        private readonly DbManipulationAppContext _db_identity;
        private readonly czytaniaContext _db_czytania;
        public SlownikDniController(DbManipulationAppContext db_identity, czytaniaContext db_czytania)
        {
            _db_identity = db_identity;
            _db_czytania = db_czytania;
        }

        public IActionResult Index()
        {
            SlownikDniViewModel model = new();
            if (TempData["dzienliturgiczny"] == null)
            {
                List<SlownikDni>? querry = new();
                try
                {
                    querry = _db_czytania.SlownikDnis.ToList();
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych";
                    return View(model);
                }
                if (querry.Any())
                    model.SlownikDni_list = querry;
            }
            else
            {
                IQueryable<SlownikDni>? querry;
                model.Dzien_liturgiczny = TempData["dzienliturgiczny"].ToString();
                try
                {
                    querry = _db_czytania.SlownikDnis.Where(m => m.DzienLiturgiczny == model.Dzien_liturgiczny);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                    return View(model);
                }
                model.SlownikDni_list = (IEnumerable<SlownikDni>)querry;
            }
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDzienLiturgicznyPicked(string dzien_liturgiczny)
        {
            IQueryable<SlownikDni>? querry_search;
            try
            {
                querry_search =
                    from item in _db_czytania.SlownikDnis
                    where item.DzienLiturgiczny.Contains(dzien_liturgiczny)
                    select item;    
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił błąd podczas wyszukiwania.";
                return PartialView("_SlownikDniDisplay", Enumerable.Empty<SlownikDni>());
            }
            return PartialView("_SlownikDniDisplay", querry_search);
        }

        public IActionResult Add()
        {
            SlownikDni model = new();
            if (TempData["model"] != null)
                model = JsonConvert.DeserializeObject<SlownikDni>((string)TempData["model"]);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(SlownikDni model)
        {
            if (ModelState.IsValid)
            {
                IQueryable<SlownikDni>? querry_check;
                try
                {
                    querry_check = _db_czytania.SlownikDnis.Where(m => m.DzienLiturgiczny == model.DzienLiturgiczny);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                if (querry_check.Any() == false)
                {
                    model.RowVersion = DateTime.Now;
                    try
                    {
                        _db_czytania.SlownikDnis.Add(model);
                        _db_czytania.SaveChanges();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił błąd podczas zapisu danych.";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Pomyślnie dodano nowy rekord.";
                    TempData["dzienliturgiczny"] = model.DzienLiturgiczny;
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Podany dzień liturgiczny już istnieje!";
                TempData["model"] = JsonConvert.SerializeObject(model);
                return RedirectToAction("Add");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(SlownikDni model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                }
                catch (Exception)
                {

                }
            }
            return RedirectToAction("Index");
        }
    }
}
