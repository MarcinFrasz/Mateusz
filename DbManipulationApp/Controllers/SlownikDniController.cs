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
            if (TempData["dzienliturgiczny"] != null)
            {
                model.Dzien_liturgiczny = (string)TempData["dzienliturgiczny"];
                try
                {
                    model.SlownikDni_list = _db_czytania.SlownikDnis.Where(m => m.DzienLiturgiczny == model.Dzien_liturgiczny);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                }
                return View(model);
            }
            try
            {
                model.SlownikDni_list = _db_czytania.SlownikDnis;
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
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

        public IActionResult Delete(string? id)
        {
            ModelState.Clear();
            if (id == null)
            {
                return NotFound();
            }
            SlownikDni? querry;
            try
            {
                querry = _db_czytania.SlownikDnis.Find(id);
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywani wartości.";
                return RedirectToAction("Index");
            }
            SlownikDni model = querry;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(SlownikDni? model)
        {
            if (ModelState.IsValid)
            {
                SlownikDni? model_check;
                try
                {
                    model_check = _db_czytania.SlownikDnis.Find(model.DzienLiturgiczny);
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
                        _db_czytania.SlownikDnis.Remove(model_check);
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

        public IActionResult Edit(string? id)
        {
            ModelState.Clear();
            SlownikDniEditModel model = new();
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<SlownikDniEditModel>((string)TempData["model"]);
            }

            if (id == null)
                return NotFound();
            try
            {
                model.MainRecord = _db_czytania.SlownikDnis.Find(id);
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                return RedirectToAction("Index");
            }
            if (model.MainRecord != null)
            {
                if (model.EditedRecord == null)
                {
                    model.EditedRecord = new();
                    model.EditedRecord.DzienLiturgiczny = model.MainRecord.DzienLiturgiczny;
                    model.EditedRecord.NazwaDnia = model.MainRecord.NazwaDnia;
                    model.EditedRecord.Swieto = model.MainRecord.Swieto;
                    model.EditedRecord.Timestamp = model.MainRecord.Timestamp;
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
        public IActionResult Edit(SlownikDniEditModel model)
        {
            if (ModelState.IsValid)
            {
                SlownikDni? check_querry;
                if (model.MainRecord.DzienLiturgiczny == model.EditedRecord.DzienLiturgiczny)
                {
                    try
                    {
                        check_querry = _db_czytania.SlownikDnis.Find(model.MainRecord.DzienLiturgiczny);
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                        return RedirectToAction("Index");
                    }
                    if (check_querry != null)
                    {
                        check_querry.DzienLiturgiczny = model.EditedRecord.DzienLiturgiczny;
                        check_querry.NazwaDnia = model.EditedRecord.NazwaDnia;
                        check_querry.Swieto = model.EditedRecord.Swieto;
                        check_querry.Timestamp = model.EditedRecord.Timestamp;
                        check_querry.RowVersion = model.EditedRecord.RowVersion;
                        try
                        {
                            _db_czytania.SlownikDnis.Update(check_querry);
                            _db_czytania.SaveChanges();
                        }
                        catch (Exception)
                        {
                            TempData["error"] = "Wystąpił problem podczas zapisu zmian. Proszę spróbować ponownie";
                            return RedirectToAction("Index");
                        }
                        TempData["success"] = "Pomyślnie zmodyfikowano rekord";
                        TempData["dzienliturgiczny"] = model.EditedRecord.DzienLiturgiczny;
                        return RedirectToAction("Index");
                    }
                    TempData["error"] = "Rekord nie mógł zostać zmodyfikowany. Prosze spróbować ponownie.";
                    return RedirectToAction("Index");
                }
                try
                {
                    check_querry = _db_czytania.SlownikDnis.Find(model.MainRecord.DzienLiturgiczny);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                SlownikDni? check_existing;
                if (check_querry != null)
                {
                    try
                    {
                        check_existing = _db_czytania.SlownikDnis.Find(model.EditedRecord.DzienLiturgiczny);
                    }
                    catch(Exception)
                    {
                        TempData["error"]= "Wystąpił problem podczas wczytywania danych";
                        return RedirectToAction("Index");
                    }
                    if(check_existing!=null)
                    {
                        TempData["error"] = "Istnieje już rekord dla podanego typu czytania";
                        TempData["model"] = JsonConvert.SerializeObject(model);
                        return RedirectToAction("Edit");
                    }
                    try 
                    { 
                    _db_czytania.SlownikDnis.Add(model.EditedRecord);
                    _db_czytania.Remove(check_querry);
                    _db_czytania.SaveChanges();
                    }
                    catch(Exception)
                    {
                        TempData["error"] = "Edycja rekordu nie powiodła się. Proszę spróbować ponownie";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Pomyślnie zmodyfikowano rekord";
                    TempData["dzienliturgiczny"] = model.EditedRecord.DzienLiturgiczny;
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

