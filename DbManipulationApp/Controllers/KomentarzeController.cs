using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        public IActionResult Add()
        {
            ModelState.Clear();
            Komentarze model = new();
            if (TempData["model"] != null)
                model = JsonConvert.DeserializeObject<Komentarze>((string)TempData["model"]);
            IQueryable<dynamic>? querry_dzienliturgiczny;
            try
            {
                querry_dzienliturgiczny = _db_czytania.SlownikDnis.Select(m =>
                new { m.DzienLiturgiczny, m.NazwaDnia });
                ViewData["komentarz_list"] =
                    (from item in _db_czytania.SKomentarzeZrodlas
                     select item.SZrodla).ToList();
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                return RedirectToAction("Index");
            }
            List<string> dzienliturginczny_list = new();
            foreach (var item in querry_dzienliturgiczny)
            {
                dzienliturginczny_list.Add(item.DzienLiturgiczny + "|" + item.NazwaDnia);
            }

            ViewData["dzienLiturgiczny_list"] = dzienliturginczny_list;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Komentarze model)
        {
            IQueryable<Komentarze>? querry_check;
            if (ModelState.IsValid)
            {
                model.DzienLiturgiczny = model.DzienLiturgiczny.Substring(0, 8);
                try
                {
                    _db_czytania.Komentarzes.Add(model);
                    _db_czytania.SaveChanges();
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas zapisu rekordu.";
                    TempData["dzienliturgiczny"] = model.DzienLiturgiczny;
                    return RedirectToAction("Index");
                }
                TempData["success"] = "Pomyślnie dodano nowy rekord.";
                TempData["dzienliturgiczny"] = model.DzienLiturgiczny;
                return RedirectToAction("Index");

            }
            TempData["error"] = "Wystąpił problem podczas walidacji.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            ModelState.Clear();
            if (id == null)
                return NotFound();

            Komentarze model = new();
            try
            {
                model = _db_czytania.Komentarzes.Find(id);
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywani wartości.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Komentarze model)
        {
            if (ModelState.IsValid)
            {
                Komentarze? model_check;
                try
                {
                    model_check = _db_czytania.Komentarzes.Find(model.Idkom);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych";
                    return RedirectToAction("Index");
                }

                if (model_check != null && JsonConvert.SerializeObject(model) == JsonConvert.SerializeObject(model_check))
                {
                    try
                    {
                        _db_czytania.Komentarzes.Remove(model_check);
                        _db_czytania.SaveChanges();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił błąd podczas usuwania rekordu.";
                        TempData["dzienliturgiczny"] = model.DzienLiturgiczny;
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Rekord został pomyślnie usunięty.";
                    TempData["dzienliturgiczny"] = model.DzienLiturgiczny;
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Rekord uległ zmianie przed zakończeniem operacji, spróbuj ponownie.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Wystąpił błąd podczas walidacji.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            KomentarzeEditModel model = new();
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<KomentarzeEditModel>((string)TempData["model"]);
            }
            ModelState.Clear();
            if (id == null)
                return NotFound();
            try
            {
                model.MainRecord = _db_czytania.Komentarzes.Find(id);
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił błąd podczas ładowania danych.";
                return RedirectToAction("Index");
            }
            if (model.MainRecord != null)
            {
                if (model.EditedRecord == null)
                {
                    model.EditedRecord = new();
                    model.EditedRecord.Idkom = model.MainRecord.Idkom;
                    try
                    {
                        var temp = _db_czytania.SlownikDnis.Find(model.MainRecord.DzienLiturgiczny);
                        model.EditedRecord.DzienLiturgiczny = temp.DzienLiturgiczny + "|" + temp.NazwaDnia;
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                        return RedirectToAction("Index");
                    }

                    model.EditedRecord.KomZrodlo = model.MainRecord.KomZrodlo;
                    model.EditedRecord.Tekst = model.MainRecord.Tekst;
                    model.EditedRecord.RowVersion = model.MainRecord.RowVersion;
                }
                if (TempData["error"] != null)
                    TempData["error"] = TempData["error"];
                IQueryable<dynamic>? querry_dzienliturgiczny;
                try
                {
                    querry_dzienliturgiczny = _db_czytania.SlownikDnis.Select(m =>
                    new { m.DzienLiturgiczny, m.NazwaDnia });
                    ViewData["komentarz_list"] =
                    (from item in _db_czytania.SKomentarzeZrodlas
                     select item.SZrodla).ToList();
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return View(model);
                }
                List<string> dzienliturginczny_list = new();
                foreach (var item in querry_dzienliturgiczny)
                {
                    dzienliturginczny_list.Add(item.DzienLiturgiczny + "|" + item.NazwaDnia);
                }

                ViewData["dzienLiturgiczny_list"] = dzienliturginczny_list;
                try
                {
                    ViewData["typCzytania_list"] = _db_czytania.STypCzytania.Select(m => m.STypCzytania).ToList();
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            TempData["error"] = "Wystąpił błąd podczas ładowania danych.";
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KomentarzeEditModel model)
        {
            if (ModelState.IsValid)
            {

                Komentarze? model_check;
                try
                {
                    model_check = _db_czytania.Komentarzes.Find(model.MainRecord.Idkom);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem dopczas wczytywania danych";
                    return RedirectToAction("Index");
                }
                if (model_check != null)
                {
                    model.EditedRecord.DzienLiturgiczny = model.EditedRecord.DzienLiturgiczny.Substring(0, 8);
                    if (model_check.RowVersion == model.MainRecord.RowVersion &&
                           model.MainRecord.Idkom == model.EditedRecord.Idkom)
                    {
                        
                        model_check.DzienLiturgiczny = model.EditedRecord.DzienLiturgiczny;
                        model_check.KomZrodlo = model.EditedRecord.KomZrodlo;
                        model_check.Tekst = model.EditedRecord.Tekst;
                        model_check.RowVersion = DateTime.Now;
                        try
                        {
                            _db_czytania.Komentarzes.Update(model_check);
                            _db_czytania.SaveChanges();
                        }
                        catch (Exception)
                        {
                            TempData["error"] = "Wystąpił problem podczas zapisu zmian.";
                            return RedirectToAction("Index");
                        }
                        TempData["success"] = "Pomyślnie dodano nowy rekord";
                        TempData["dzienliturgiczny"] = model.EditedRecord.DzienLiturgiczny;
                        return RedirectToAction("Index");
                    }
                    TempData["error"] = "Wystąpił problem podczas walidacji danych.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Wystąpił problem podczas wczytywania danych proszę spróbować ponownie";
                TempData["dzienliturgiczny"] = model.MainRecord.DzienLiturgiczny;
                return RedirectToAction("Index");
            }
            TempData["error"] = "Wystąpił problem podczas walidacji.";
            return RedirectToAction("Index");
        }
    }
}
