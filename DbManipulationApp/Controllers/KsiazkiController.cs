using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DbManipulationApp.Controllers
{
    [Authorize]
    public class KsiazkiController : Controller
    {
        private readonly DbManipulationAppContext _db_identity;
        private readonly czytaniaContext _db_czytania;
        public KsiazkiController(DbManipulationAppContext db_identity, czytaniaContext db_czytania)
        {
            _db_identity = db_identity;
            _db_czytania = db_czytania;
        }
        public IActionResult Index()
        {
            ModelState.Clear();
            KsiazkiViewModel model = new();
            try
            {
                ViewData["title_list"] = _db_czytania.Ksiazkis.Select(m => m.Tytul).ToList();
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                ViewData["title_list"] = new List<string>();
                return View(model);
            }
            if (TempData["title"] != null)
            {
                model.Title = (string)TempData["title"];
                try
                {
                    model.Books = _db_czytania.Ksiazkis.Where(m => m.Tytul == model.Title);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                }
                return View(model);
            }
            try
            {
                model.Books = _db_czytania.Ksiazkis;
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexTytulPicked(string? title)
        {
            IQueryable<Ksiazki>? querry;
            try
            {
                querry = from book in _db_czytania.Ksiazkis
                         where book.Tytul == title
                         select book;
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                return PartialView("_KsiazkiDisplay", Enumerable.Empty<Video>());
            }
            return PartialView("_KsiazkiDisplay", querry);
        }

        public IActionResult Add()
        {
            ModelState.Clear();
            Ksiazki model = new();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Ksiazki model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    _db_czytania.Ksiazkis.Add(model);
                    _db_czytania.SaveChanges();
                }
                catch(Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                TempData["success"] = "Pomyślnie dodano rekord.";
                TempData["title"] = model.Tytul;
                return RedirectToAction("Index");
            }
            TempData["error"] = "Wystąpił problem z walidacją danych.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            ModelState.Clear();
            if (id == null)
                return NotFound();

            Ksiazki model = new();
            try
            {
                model = _db_czytania.Ksiazkis.Find(id);
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
        public IActionResult Delete(Ksiazki model)
        {
            if (ModelState.IsValid)
            {
                Ksiazki? model_check;
                try
                {
                    model_check = _db_czytania.Ksiazkis.Find(model.IdKsiazki);
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
                        _db_czytania.Ksiazkis.Remove(model_check);
                        _db_czytania.SaveChanges();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił błąd podczas usuwania rekordu.";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Rekord został pomyślnie usunięty.";
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
            KsiazkiEditModel model = new();
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<KsiazkiEditModel>((string)TempData["model"]);
            }
            ModelState.Clear();
            if (id == null)
                return NotFound();
            try
            {
                model.MainRecord = _db_czytania.Ksiazkis.Find(id);
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
                    model.EditedRecord.IdKsiazki = model.MainRecord.IdKsiazki;
                    model.EditedRecord.IdKmt = model.MainRecord.IdKmt;
                    model.EditedRecord.Tytul = model.MainRecord.Tytul;
                    model.EditedRecord.Autor = model.MainRecord.Autor;
                    model.EditedRecord.Foto = model.MainRecord.Foto;
                    model.EditedRecord.Opis = model.MainRecord.Opis;
                    model.EditedRecord.RowVersion = model.MainRecord.RowVersion;
                }
                if (TempData["error"] != null)
                    TempData["error"] = TempData["error"];
                return View(model);
            }
            TempData["error"] = "Wystąpił błąd podczas ładowania danych.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KsiazkiEditModel model)
        {
            if (ModelState.IsValid)
            {
                Ksiazki? check_querry = new();
                try
                {
                    check_querry = _db_czytania.Ksiazkis.Find(model.MainRecord.IdKsiazki);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas ładowania danych.";
                }
                if(check_querry==null)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                if (check_querry.IdKsiazki != model.MainRecord.IdKsiazki ||
                    model.EditedRecord.IdKsiazki != model.MainRecord.IdKsiazki ||
                    check_querry.RowVersion != model.MainRecord.RowVersion)
                {
                    TempData["error"] = "Wystąpił problem z walidacją rekrodu. " +
                        "Proszę spróbować ponownie.";
                    return RedirectToAction("Index");
                }
                try
                {
                    check_querry.IdKmt = model.EditedRecord.IdKmt;
                    check_querry.Tytul = model.EditedRecord.Tytul;
                    check_querry.Autor = model.EditedRecord.Autor;
                    check_querry.Foto = model.EditedRecord.Foto;
                    check_querry.Opis = model.EditedRecord.Opis;
                    _db_czytania.Ksiazkis.Update(check_querry);
                    _db_czytania.SaveChanges();
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas zapisu zmian rekordu.";
                    return RedirectToAction("Index");
                }
                TempData["title"] = model.EditedRecord.Tytul;
                TempData["success"] = "Pomyślnie zmodyfikowano rekord.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Coś poszło nie tak podczas walidacji.";
            return RedirectToAction("Index");
        }
    }
}
