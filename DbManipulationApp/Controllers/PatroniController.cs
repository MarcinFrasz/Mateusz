using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace DbManipulationApp.Controllers
{
    public class PatroniController : Controller
    {
        private readonly DbManipulationAppContext _db_identity;
        private readonly czytaniaContext _db_czytania;
        public PatroniController(DbManipulationAppContext db_identity, czytaniaContext db_czytania)
        {
            _db_identity = db_identity;
            _db_czytania = db_czytania;
        }

        public IActionResult Index()
        {
            ModelState.Clear();
            PatroniViewModel model = new();
            if (TempData["date"] != null)
            {
                model.Date = (string)TempData["date"];
                try
                {
                    model.Patroni = _db_czytania.Patronis.Where(m => m.Data == model.Date);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                }
                return View(model);
            }
            try
            {
                model.Patroni = _db_czytania.Patronis;
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDatePicked(string? date)
        {
            Regex regex=new Regex(@"^([0][1-9]|[1][0-2])[-]([0][1-9]|[1-2][0-9]|[3][0-1])$");
            if (regex.IsMatch(date))
            {
                IQueryable<Patroni>? querry_search;
                try
                {
                    querry_search =
                        from item in _db_czytania.Patronis
                        where item.Data.Contains(date)
                        select item;
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wyszukiwania.";
                    return PartialView("_PatroniDisplay", Enumerable.Empty<Patroni>());
                }
                return PartialView("_PatroniDisplay", querry_search);
            }
            return PartialView("_PatroniDisplay", Enumerable.Empty<Patroni>());
        }

        public IActionResult Add()
        {
            ModelState.Clear();
            Patroni model=new();
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Patroni model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    _db_czytania.Patronis.Add(model);
                    _db_czytania.SaveChanges();
                }
                catch(Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas zapisu danych. Proszę spróbować ponownie.";
                    //return RedirectToAction("Index");
                    throw;
                }
                TempData["success"] = "Pomyślnie dodano nowy rekord.";
                TempData["date"] = model.Data;
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
            Patroni? querry;
            try
            {
                querry = _db_czytania.Patronis.Find(id);
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywani wartości.";
                return RedirectToAction("Index");
            }
            Patroni model = new();
            if (querry != null)
                model = querry;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Patroni model)
        {
            if (ModelState.IsValid)
            {
                Patroni? model_check;
                try
                {
                    model_check = _db_czytania.Patronis.Find(model.Id);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych";
                    return RedirectToAction("Index");
                }

                if (JsonConvert.SerializeObject(model) == JsonConvert.SerializeObject(model_check))
                {
                    try
                    {
                        _db_czytania.Patronis.Remove(model_check);
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
            PatroniEditModel model = new();
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<PatroniEditModel>((string)TempData["model"]);
            }
            ModelState.Clear();
            if (id == null)
                return NotFound();
            try
            {
                model.MainRecord = _db_czytania.Patronis.Find(id);
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
                    model.EditedRecord.Id = model.MainRecord.Id;
                    model.EditedRecord.Data = model.MainRecord.Data;
                    model.EditedRecord.Patron = model.MainRecord.Patron;
                    model.EditedRecord.Glowny = model.MainRecord.Glowny;
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
        public IActionResult Edit(PatroniEditModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.EditedRecord.Id!=model.MainRecord.Id)
                {
                    TempData["error"] = "Wystąpił problem podczas ładowania danych. Proszę spróbować ponownie.";
                    return RedirectToAction("Index");
                }
                Patroni? check_querry;
                try
                {
                    check_querry = _db_czytania.Patronis.Find(model.MainRecord.Id);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                if (check_querry != null && check_querry.RowVersion==model.MainRecord.RowVersion)
                { 
                    try
                    {
                        check_querry.Data = model.EditedRecord.Data;
                        check_querry.Patron = model.EditedRecord.Patron;
                        check_querry.Glowny = model.EditedRecord.Glowny;
                        check_querry.Opis = model.EditedRecord.Opis;
                        check_querry.RowVersion = DateTime.Now;
                        _db_czytania.Patronis.Update(check_querry);
                        _db_czytania.SaveChanges();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Edycja rekordu nie powiodła się. Proszę spróbować ponownie";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Pomyślnie zmodyfikowano rekord";
                    TempData["date"] = model.EditedRecord.Data;
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Wystąpił problem podczas edycji. Proszę spróbować ponownie.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Wystąpił problem z walidacją.";
            return RedirectToAction("Index");
        }
    }
}
