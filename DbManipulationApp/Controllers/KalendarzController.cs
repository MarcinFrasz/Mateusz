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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(KalendarzAddModel model)
        {
            if (ModelState.IsValid)
            {
                model.Kalendarz.IdKsiazka1 = Convert.ToInt32((model.BookId.Split('|')[0]));
                IQueryable<Kalendarz>? qValidateDate;
                try
                {
                    qValidateDate = _db_czytania.Kalendarzs.Where(m => m.Data == model.Kalendarz.Data);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                if (qValidateDate.Any() == false)
                {
                    DateTime Date = model.Kalendarz.Data;
                    string month = Date.Month.ToString();
                    string day = Date.Day.ToString();
                    if (month.Length < 2)
                        month = "0" + month;
                    if (day.Length < 2)
                        day = "0" + day;
                    var date_patroni = month + "-" + day;
                    Patroni? querry_patron;
                    try
                    {
                        querry_patron = _db_czytania.Patronis.Where(m => m.Data == date_patroni).First();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                        return RedirectToAction("Index");
                    }
                    if (querry_patron != null)
                    {
                        querry_patron.Wyswietl = model.ShowPatron;
                        querry_patron.RowVersion = DateTime.Now;
                        try
                        {
                            _db_czytania.Patronis.Update(querry_patron);
                            _db_czytania.SaveChanges();
                        }
                        catch (Exception)
                        {
                            TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                            return RedirectToAction("Index");
                        }
                    }
                    model.Kalendarz.RowVersion = DateTime.Now;
                    model.Kalendarz.DzienLiturgiczny = model.Kalendarz.DzienLiturgiczny.Substring(0, 8);
                    try
                    {
                        _db_czytania.Kalendarzs.Add(model.Kalendarz);
                        _db_czytania.SaveChanges();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił błąd podczas zapisu rekordu.";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Nowy rekord został  pomyślnie dodany.";
                    TempData["date"] = model.Kalendarz.Data.Date.ToString("dd/MM/yyyy");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Istnieje już rekord dla podanej daty!";
                    TempData["model"] = JsonConvert.SerializeObject(model);
                    return RedirectToAction("Add");
                }
            }


            TempData["error"] = "Wystąpił problem z walidacją.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string? id)
        {
            ModelState.Clear();
            if (id == null)
                return NotFound();
            Kalendarz model = new();
            id = Uri.UnescapeDataString(id);
            model.Data = Convert.ToDateTime(id);
            try
            {
                model = _db_czytania.Kalendarzs.Find(model.Data);
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
        public IActionResult Delete(Kalendarz model)
        {
            if (ModelState.IsValid)
            {
                Kalendarz? model_check;
                try
                {
                    model_check = _db_czytania.Kalendarzs.Find(model.Data);
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
                        _db_czytania.Kalendarzs.Remove(model_check);
                        _db_czytania.SaveChanges();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił błąd podczas usuwania rekordu.";
                        TempData["date"] = model.Data.Date.ToString("dd/MM/yyyy");
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Rekord został pomyślnie usunięty.";
                    TempData["date"] = model.Data.Date.ToString("dd/MM/yyyy");
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
            KalendarzEditModel model = new();
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<KalendarzEditModel>((string)TempData["model"]);
            }
            ModelState.Clear();
            if (id == null)
                return NotFound();
            id = Uri.UnescapeDataString(id);          
            try
            {
                model.MainRecord = _db_czytania.Kalendarzs.Find(Convert.ToDateTime(id));
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
                    try
                    {
                        var querry_ksiazka = _db_czytania.Ksiazkis.Find(model.MainRecord.IdKsiazka1);
                        model.BookId = querry_ksiazka.IdKsiazki.ToString() + '|' + querry_ksiazka.Tytul;
                        var querry_dzien = _db_czytania.SlownikDnis.Find(model.MainRecord.DzienLiturgiczny);
                        model.EditedRecord.DzienLiturgiczny = querry_dzien.DzienLiturgiczny + '|' + querry_dzien.NazwaDnia;
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                        return RedirectToAction("Index");
                    }
                    model.EditedRecord.Data = model.MainRecord.Data;
                    model.EditedRecord.NazwaDnia = model.MainRecord.NazwaDnia;
                    model.EditedRecord.KomZrodloD = model.MainRecord.KomZrodloD;
                    model.EditedRecord.KomZrodloM= model.MainRecord.KomZrodloM;
                    model.EditedRecord.IdKsiazka1 = model.MainRecord.IdKsiazka1;                   
                    model.EditedRecord.NumerTygodnia = model.MainRecord.NumerTygodnia;
                    model.EditedRecord.RowVersion = model.MainRecord.RowVersion;
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
                return View(model);
            }
            TempData["error"] = "Wystąpił błąd podczas ładowania danych.";
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KalendarzEditModel model)
        {
            if (ModelState.IsValid)
            {
                Kalendarz? check_querry;
                if (model.MainRecord.Data == model.EditedRecord.Data)
                {
                    try
                    {
                        check_querry = _db_czytania.Kalendarzs.Find(model.MainRecord.Data);
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                        return RedirectToAction("Index");
                    }
                    if (check_querry != null && check_querry.RowVersion == model.MainRecord.RowVersion)
                    {
                        check_querry.Data = model.EditedRecord.Data;
                        check_querry.DzienLiturgiczny=model.EditedRecord.DzienLiturgiczny.Substring(0,8);
                        check_querry.NazwaDnia = model.EditedRecord.NazwaDnia;
                        check_querry.KomZrodloD = model.EditedRecord.KomZrodloD;
                        check_querry.KomZrodloM = model.EditedRecord.KomZrodloM;
                        check_querry.IdKsiazka1 = Convert.ToInt32(model.BookId.Split('|')[0]);
                        check_querry.NumerTygodnia = model.EditedRecord.NumerTygodnia;
                        check_querry.RowVersion = DateTime.Now;
                        try
                        {
                            _db_czytania.Kalendarzs.Update(check_querry);
                            _db_czytania.SaveChanges();
                        }
                        catch (Exception)
                        {
                            TempData["error"] = "Wystąpił problem podczas zapisu zmian. Proszę spróbować ponownie";
                            return RedirectToAction("Index");
                        }
                        TempData["success"] = "Pomyślnie zmodyfikowano rekord";
                        TempData["date"] = model.EditedRecord.Data.Date.ToString("dd/MM/yyyy");
                        return RedirectToAction("Index");
                    }
                    TempData["error"] = "Rekord nie mógł zostać zmodyfikowany. Prosze spróbować ponownie.";
                    return RedirectToAction("Index");
                }
                try
                {
                    check_querry = _db_czytania.Kalendarzs.Find(model.MainRecord.Data);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                Kalendarz? check_existing;
                if (check_querry != null && check_querry.RowVersion == model.MainRecord.RowVersion)
                {
                    try
                    {
                        check_existing = _db_czytania.Kalendarzs.Find(model.EditedRecord.Data);
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił problem podczas wczytywania danych";
                        return RedirectToAction("Index");
                    }
                    if (check_existing != null)
                    {
                        TempData["error"] = "Istnieje już rekord dla tej daty";
                        TempData["model"] = JsonConvert.SerializeObject(model);
                        return RedirectToAction("Edit");
                    }
                    try
                    {
                        model.EditedRecord.RowVersion = DateTime.Now;
                        model.EditedRecord.DzienLiturgiczny = model.EditedRecord.DzienLiturgiczny.Substring(0, 8);
                        model.EditedRecord.IdKsiazka1 = Convert.ToInt32(model.BookId.Split('|')[0]);
                        _db_czytania.Kalendarzs.Add(model.EditedRecord);
                        _db_czytania.Remove(check_querry);
                        _db_czytania.SaveChanges();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Edycja rekordu nie powiodła się. Proszę spróbować ponownie";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Pomyślnie zmodyfikowano rekord";
                    TempData["date"] = model.EditedRecord.Data.Date.ToString("dd/MM/yyyy");
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Wystąpił problem podczas edycji. Proszę spróbować ponownie.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Wystąpił problem z walidacją.";
            return RedirectToAction("Index");
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
