using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DbManipulationApp.Controllers
{
    public class KalendarzController : Controller
    {
        private readonly DbManipulationAppContext _db_identity;
        private readonly czytaniaContext _db_czytania;
        public KalendarzController(DbManipulationAppContext db_identity, czytaniaContext db_czytania)
        {
            _db_identity = db_identity;
            _db_czytania = db_czytania;
        }
        public IActionResult Index()
        {
            ModelState.Clear();
            KalendarzViewModel model = new();
            if (TempData["date"] != null)
                model.Date = DateTime.ParseExact((string)TempData["date"], "dd/MM/yyyy", null);

            if (model.Date == DateTime.MinValue)
            {
                try
                {
                    model.Kalendarz = _db_czytania.Kalendarzs;
                }
                catch(Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                }
                model.Date = DateTime.Now.Date;
                return View(model);
            }

            try
            {
                model.Kalendarz = _db_czytania.Kalendarzs.Where(m => m.Data.Date == model.Date.Date);
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas ładowania danych.";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime? date)
        {
            if (date != null)
            {
                DateTime ndate = (DateTime)date;
                return RedirectToAction("Add", new { id = ndate.Date });
            }
            return RedirectToAction("Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDatePicked(string? date)
        {
            IQueryable<Kalendarz>? querry;
            try
            {
                var currentDate = Convert.ToDateTime(date);
                querry = from kal in _db_czytania.Kalendarzs
                         where kal.Data.Date == currentDate.Date.Date
                         select kal;
            }
            catch (Exception)
            {
                ModelState.AddModelError("Data", "Format daty jest nieprawidłowy.");
                return PartialView("_KalendarzDisplay", Enumerable.Empty<Kalendarz>());
            }
            return PartialView("_KalendarzDisplay", querry);
        }

        public IActionResult Add(string? id)
        {
            ModelState.Clear();
            KalendarzAddModel model = new();
            if (TempData["model"] != null)
                model = JsonConvert.DeserializeObject<KalendarzAddModel>((string)TempData["model"]);
            else
            {
                if (id != null)
                {
                    id = Uri.UnescapeDataString(id);
                    model.Kalendarz.Data = Convert.ToDateTime(id);
                }
            }
            if (TempData["error"] != null)
                TempData["error"] = TempData["error"];
            IQueryable<dynamic>? querry_ksiazki;
            IQueryable<dynamic>? querry_dzienliturgiczny;
            try
            {
                ViewData["komentarz_list"] =
                    (from item in _db_czytania.SKomentarzeZrodlas
                    select item.SZrodla).ToList();
                    //_db_czytania.SKomentarzeZrodlas.Select(m => m.SZrodla).ToList();
                querry_ksiazki = _db_czytania.Ksiazkis.Select(m =>
                new { m.IdKsiazki, m.Tytul });
                querry_dzienliturgiczny = _db_czytania.SlownikDnis.Select(m =>
                new { m.DzienLiturgiczny, m.NazwaDnia });
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                ViewData["ksiazki_list"] = new List<string>();
                ViewData["komentarz_list"] = new List<string>();
                ViewData["dzienLiturgiczny_list"] = new List<string>();
                return View(model);
            }
            List<string> ksiazki_list = new();
            foreach (var item in querry_ksiazki)
            {
                ksiazki_list.Add(item.IdKsiazki + "|" + item.Tytul);
            }
            ViewData["ksiazki_list"] = ksiazki_list;
            List<string> dzienliturginczny_list = new();
            foreach (var item in querry_dzienliturgiczny)
            {
                dzienliturginczny_list.Add(item.DzienLiturgiczny + "|" + item.NazwaDnia);
            }

            ViewData["dzienLiturgiczny_list"] = dzienliturginczny_list;
            return View("Add", model);
        }

        public IActionResult DayChanged(string? day)
        {
            if(day!=null)
            {
                var name=day.Substring(9);
                return Json(new { Success = "true", Name = name });
            }
            else
            {
                return Json(new { Success = "false" });
            }
        }
    }
}
