using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DbManipulationApp.Controllers
{
    [Authorize]
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
            LekcjonarzViewModel model = new();
            if (TempData["dzienlitrugiczny"] != null)
            {
                model.Lekcjonarze = _db_czytania.Lekcjonarzs.Where(m => m.DzienLiturgiczny == (string)TempData["dzienliturgiczny"]);
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
            if (day != null)
            {
                var str = day.Split('|');
                var picked_day = str[0];
                try
                {
                    model = _db_czytania.Lekcjonarzs.Where(m => m.DzienLiturgiczny == picked_day);
                }
                catch (Exception)
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
                        where EF.Functions.Like(item.DzienLiturgiczny, day + "%")
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

        public IActionResult Add()
        {
            ModelState.Clear();
            Lekcjonarz model = new();
            if (TempData["model"] != null)
                model = JsonConvert.DeserializeObject<Lekcjonarz>((string)TempData["model"]);
            IQueryable<dynamic>? querry_dzienliturgiczny;
            try
            {
                querry_dzienliturgiczny = _db_czytania.SlownikDnis.Select(m =>
                new { m.DzienLiturgiczny, m.NazwaDnia });
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                return RedirectToAction("Index");
            }
            List<string> dzienliturginczny_list = new ();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Lekcjonarz model)
        {
            IQueryable<Lekcjonarz>? querry_check;
            if (ModelState.IsValid)
            {
                model.DzienLiturgiczny = model.DzienLiturgiczny.Substring(0, 8);
                
                try
                {
                    querry_check = _db_czytania.Lekcjonarzs.Where(m => m.DzienLiturgiczny == model.DzienLiturgiczny && m.TypCzytania == model.TypCzytania);
                }
                catch(Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                if(querry_check.Any())
                {
                    try
                    {
                        var temp = _db_czytania.SlownikDnis.Find(model.DzienLiturgiczny);
                        model.DzienLiturgiczny = temp.DzienLiturgiczny + "|" + temp.NazwaDnia;
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                        return RedirectToAction("Index");
                    }
                    TempData["error"] = "Istnieje już rekord dla podanego dnia liturgicznego i typu czytania.";
                    TempData["model"] = JsonConvert.SerializeObject(model);
                    return RedirectToAction("Add");
                }
                try
                {
                    _db_czytania.Lekcjonarzs.Add(model);
                    _db_czytania.SaveChanges();
                }
                catch(Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas zapisu rekordu.";
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

            Lekcjonarz model = new();
            try
            {
                model = _db_czytania.Lekcjonarzs.Find(id);
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
        public IActionResult Delete(Lekcjonarz model)
        {
            if (ModelState.IsValid)
            {
                Lekcjonarz? model_check;
                try
                {
                    model_check = _db_czytania.Lekcjonarzs.Find(model.IdLlekcjonarz);
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
                        _db_czytania.Lekcjonarzs.Remove(model_check);
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
            LekcjonarzEditModel model = new();
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<LekcjonarzEditModel>((string)TempData["model"]);
            }
            ModelState.Clear();
            if (id == null)
                return NotFound();
            try
            {
                model.MainRecord = _db_czytania.Lekcjonarzs.Find(id);
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
                    model.EditedRecord.IdLlekcjonarz = model.MainRecord.IdLlekcjonarz;
                    try
                    {
                        var temp = _db_czytania.SlownikDnis.Find(model.MainRecord.DzienLiturgiczny);
                        model.EditedRecord.DzienLiturgiczny = temp.DzienLiturgiczny + "|" + temp.NazwaDnia;
                    }
                    catch(Exception)
                    {
                        TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                        return RedirectToAction("Index");
                    }
                  
                    model.EditedRecord.TypCzytania = model.MainRecord.TypCzytania;
                    model.EditedRecord.Siglum = model.MainRecord.Siglum;
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
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return View(model);
                }
                List<string> dzienliturginczny_list = new ();
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
        public IActionResult Edit(LekcjonarzEditModel model)
        {
            if (ModelState.IsValid)
            {

                Lekcjonarz? model_check;
                try
                {
                   model_check= _db_czytania.Lekcjonarzs.Find(model.MainRecord.IdLlekcjonarz);
                }
                catch(Exception)
                {
                    TempData["error"] = "Wystąpił problem dopczas wczytywania danych";
                    return RedirectToAction("Index");
                }
                if(model_check!=null)
                {
                    model.EditedRecord.DzienLiturgiczny=model.EditedRecord.DzienLiturgiczny.Substring(0, 8);
                 if(model_check.RowVersion==model.MainRecord.RowVersion &&
                        model.MainRecord.IdLlekcjonarz==model.EditedRecord.IdLlekcjonarz)
                    {
                        if(model.MainRecord.DzienLiturgiczny!=model.EditedRecord.DzienLiturgiczny || 
                            model.MainRecord.TypCzytania!=model.EditedRecord.TypCzytania)
                        {
                            IQueryable<Lekcjonarz>? check;
                            try
                            {
                                check=_db_czytania.Lekcjonarzs.Where(m => m.DzienLiturgiczny == model.EditedRecord.DzienLiturgiczny
                                && m.TypCzytania == model.EditedRecord.TypCzytania);
                            }
                            catch(Exception)
                            {
                                TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                                return RedirectToAction("Index");
                            }
                            if(check.Any()==false)
                            {
                                model_check.DzienLiturgiczny = model.EditedRecord.DzienLiturgiczny;
                                model_check.TypCzytania = model.EditedRecord.TypCzytania;
                                model_check.Siglum = model.EditedRecord.Siglum;
                                model_check.Tekst = model.EditedRecord.Tekst;
                                model_check.RowVersion = DateTime.Now;
                                try
                                {
                                    _db_czytania.Lekcjonarzs.Update(model_check);
                                    _db_czytania.SaveChanges();
                                }
                                catch(Exception)
                                {
                                    TempData["error"] = "Wystąpił problem podczas zapisu zmian.";
                                    return RedirectToAction("Index");
                                }
                                TempData["success"] = "Pomyślnie dodano nowy rekord";
                                TempData["dzienliturgiczny"] = model.EditedRecord.DzienLiturgiczny;
                                return RedirectToAction("Index");
                            }
                            try
                            {
                                var temp = _db_czytania.SlownikDnis.Find(model.EditedRecord.DzienLiturgiczny);
                                model.EditedRecord.DzienLiturgiczny = temp.DzienLiturgiczny + "|" + temp.NazwaDnia;
                            }
                            catch (Exception)
                            {
                                TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                                return RedirectToAction("Index");
                            }
                            TempData["error"] = "Istnieje już rekord dla podanego dnia liturgicznego i typu czytania.";
                            TempData["model"] = JsonConvert.SerializeObject(model);
                            return RedirectToAction("Edit");
                        }
                        model_check.DzienLiturgiczny = model.EditedRecord.DzienLiturgiczny;
                        model_check.TypCzytania = model.EditedRecord.TypCzytania;
                        model_check.Siglum = model.EditedRecord.Siglum;
                        model_check.Tekst = model.EditedRecord.Tekst;
                        model_check.RowVersion = DateTime.Now;
                        try
                        {
                            _db_czytania.Lekcjonarzs.Update(model_check);
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
