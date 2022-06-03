using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DbManipulationApp.Controllers
{
    public class VideoController : Controller
    {
        private readonly DbManipulationAppContext _db_identity;
        private readonly czytaniaContext _db_czytania;
        public VideoController(DbManipulationAppContext db_identity, czytaniaContext db_czytania)
        {
            _db_identity = db_identity;
            _db_czytania = db_czytania;
        }

        public IActionResult Index()
        {
            ModelState.Clear();
            VideoViewModel model = new();
            if (TempData["date"] != null)
                model.Date = DateTime.ParseExact((string)TempData["date"], "dd/MM/yyyy", null);

            if (model.Date == DateTime.MinValue)
                model.Date = DateTime.Now.Date;

            IQueryable<Video>? querry_videosSelect;
            try
            {
                querry_videosSelect = _db_czytania.Videos.Where(m => m.Data.Date == model.Date.Date);
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas ładowania danych.";
                return View(model);
            }
            if (querry_videosSelect.Any())
                model.Videos = querry_videosSelect;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime? date)
        {
            IQueryable<string>? querry_typczytania;
            try
            {
                querry_typczytania =
                from typ in _db_czytania.STypCzytania
                select typ.STypCzytania;
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                return RedirectToAction("Index");
            }
            ViewData["typCzytania_list"] = querry_typczytania.ToList();
            if (date != null)
            {
                DateTime ndate = (DateTime)date;
                return RedirectToAction("Add", new { id = ndate.Date });
            }
            return RedirectToAction("Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDatePicked(string date)
        {
            IQueryable<Video>? querry;
            try
            {
                var currentDate = Convert.ToDateTime(date);
                querry = from vid in _db_czytania.Videos
                         where vid.Data.Date == currentDate.Date.Date
                         select vid;
            }
            catch (Exception)
            {
                ModelState.AddModelError("Data", "Format daty jest nieprawidłowy.");
                return PartialView("_VideoDisplay", Enumerable.Empty<Video>());
            }
            IEnumerable<Video> model = querry;
            return PartialView("_VideoDisplay", model);
        }

        public IActionResult Add(string? id)
        {
            ModelState.Clear();
            Video model = new();
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<Video>((string)TempData["model"]);
            }
            else
            {
                if (id != null)
                {
                    id = Uri.UnescapeDataString(id);
                    model.Data = Convert.ToDateTime(id);
                }
            }
            if (TempData["error"] != null)
                TempData["error"] = TempData["error"];
            IQueryable<string>? querry_typczytania;
            try
            {
                querry_typczytania =
                    from typ in _db_czytania.STypCzytania
                    select typ.STypCzytania;
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywania wartości.";
                return RedirectToAction("Index");
            }
            ViewData["typCzytania_list"] = querry_typczytania.ToList();
            return View("Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Video model)
        {
            if (ModelState.IsValid)
            {
                IQueryable<Video>? qValidateTypCzytania;
                try
                {
                    qValidateTypCzytania = _db_czytania.Videos.Where(m => m.Data == model.Data && m.TypCzytania == model.TypCzytania);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                if (qValidateTypCzytania.Any() == false)
                {
                    model.RowVersion = DateTime.Now;
                    try
                    {
                        _db_czytania.Videos.Add(model);
                        _db_czytania.SaveChanges();
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił błąd podczas zapisu rekordu.";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Nowy rekord został  pomyślnie dodany.";
                    TempData["date"] = model.Data.Date.ToString("dd/MM/yyyy");
                    return RedirectToAction("Index");
                }
                else
                {
                    IQueryable<string>? querry_typczytania;
                    try
                    {
                        querry_typczytania =
                        from typ in _db_czytania.STypCzytania
                        select typ.STypCzytania;
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                        return RedirectToAction("Index");
                    }
                    ViewData["typCzytania_list"] = querry_typczytania.ToList();
                    TempData["error"] = "Istnieje już rekord dla podanej daty i typu czytania!";
                    TempData["model"] = JsonConvert.SerializeObject(model);
                    return RedirectToAction("Add");
                }
            }
            TempData["error"] = "Wystąpił problem z walidacją.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            ModelState.Clear();
            if (id == null)
                return NotFound();

            Video? querry;
            try
            {
                querry = _db_czytania.Videos.Find(id);
            }
            catch (Exception)
            {
                TempData["error"] = "Wystąpił problem podczas wczytywani wartości.";
                return RedirectToAction("Index");
            }
            Video model = new();
            if (querry != null)
                model = querry;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Video? model)
        {
            if (ModelState.IsValid)
            {
                Video? model_check;
                try
                {
                    model_check = _db_czytania.Videos.Find(model.IdVideo);
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
                        _db_czytania.Videos.Remove(model_check);
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

        public IActionResult Edit(int? id)
        {
            VideoEditModel model = new();
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<VideoEditModel>((string)TempData["model"]);
            }
            ModelState.Clear();
            if (id == null)
                return NotFound();
            try
            {
                model.MainRecord = _db_czytania.Videos.Find(id);
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
                    model.EditedRecord.IdVideo = model.MainRecord.IdVideo;
                    model.EditedRecord.Data = model.MainRecord.Data;
                    model.EditedRecord.TypCzytania = model.MainRecord.TypCzytania;
                    model.EditedRecord.YoutubeId = model.MainRecord.YoutubeId;
                    model.EditedRecord.RowVersion = model.MainRecord.RowVersion;
                }
                if (TempData["error"] != null)
                    TempData["error"] = TempData["error"];
                IQueryable<string>? querry_typczytania;
                try
                {
                    querry_typczytania =
                    from typ in _db_czytania.STypCzytania
                    select typ.STypCzytania;
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                ViewData["typCzytania_list"] = querry_typczytania.ToList();
                return View(model);
            }
            TempData["error"] = "Wystąpił błąd podczas ładowania danych.";
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(VideoEditModel model)
        {
            if (ModelState.IsValid)
            {
                IQueryable<string>? querry_typczytania;
                try
                {
                    querry_typczytania =
                    from typ in _db_czytania.STypCzytania
                    select typ.STypCzytania;
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas wczytywania danych.";
                    return RedirectToAction("Index");
                }
                Video? check_querry = new();
                try
                {
                    check_querry = _db_czytania.Videos.Find(model.MainRecord.IdVideo);
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił problem podczas ładowania danych.";
                }
                if (check_querry.IdVideo != model.MainRecord.IdVideo ||
                    model.EditedRecord.IdVideo != model.MainRecord.IdVideo ||
                    check_querry.RowVersion != model.MainRecord.RowVersion)
                {
                    TempData["error"] = "Wystąpił problem z walidacją rekrodu. " +
                        "Proszę spróbować ponownie.";
                    return RedirectToAction("Index");
                }
                if (model.EditedRecord.Data != model.MainRecord.Data || model.EditedRecord.TypCzytania != model.MainRecord.TypCzytania)
                {
                    IQueryable<Video>? querry;
                    try
                    {
                        querry = _db_czytania.Videos.Where(m => m.Data.Date == model.EditedRecord.Data.Date && m.TypCzytania == model.EditedRecord.TypCzytania);
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Wystąpił problem podczas ładowania danych.";
                        return RedirectToAction("Index");
                    }

                    if (querry.Any() != false)
                    {
                        ViewData["typCzytania_list"] = querry_typczytania.ToList();
                        TempData["error"] = "Istnieje już rekord dla podanej daty i typu czytania.";
                        TempData["model"] = JsonConvert.SerializeObject(model);
                        return RedirectToAction("Edit");
                    }
                }
                try
                {
                    check_querry.Data = model.EditedRecord.Data;
                    check_querry.TypCzytania = model.EditedRecord.TypCzytania;
                    check_querry.YoutubeId = model.EditedRecord.YoutubeId;
                    check_querry.RowVersion = DateTime.Now;
                    _db_czytania.Videos.Update(check_querry);
                    _db_czytania.SaveChanges();
                }
                catch (Exception)
                {
                    TempData["error"] = "Wystąpił błąd podczas zapisu zmian rekordu.";
                    return RedirectToAction("Index");
                }
                TempData["date"] = model.EditedRecord.Data.ToString("dd/MM/yyyy");
                TempData["success"] = "Pomyślnie zmodyfikowano rekord.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Coś poszło nie tak podczas walidacji.";
            return RedirectToAction("Index");
        }
    }
}

