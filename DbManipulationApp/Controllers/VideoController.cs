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
            if(TempData["date"]!=null)
                model.Date= DateTime.ParseExact((string)TempData["date"], "dd/MM/yyyy", null);
            
            if (model.Date == DateTime.MinValue)
            {
                model.Date = DateTime.Now.Date;
            }
            var querry_videosSelect = _db_czytania.Videos.Where(m => m.Data.Date == model.Date.Date);
            model.Videos = querry_videosSelect;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime? date)
        {
            if (date != null)
            {
                Video model_send = new();
                model_send.Data = (DateTime)date;
                TempData["model"] = JsonConvert.SerializeObject(model_send);
                return RedirectToAction("Add");
            }
            return RedirectToAction("Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDatePicked(string date)
        {
            try
            {
                var currentDate = Convert.ToDateTime(date);
                var querry = from vid in _db_czytania.Videos
                             where vid.Data.Date == currentDate.Date
                             select vid;
                IEnumerable<Video> model= querry;
                return PartialView("_VideoDisplay", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("Data", "Format daty jest nieprawidłowy.");
                return PartialView("_VideoDisplay",Enumerable.Empty<Video>());
            }
        }       

        public IActionResult Add()
        {
            ModelState.Clear();
            Video model = new();
            if (TempData["model"] != null)
            {
                TempData["model"] = JsonConvert.DeserializeObject<Video>((string)TempData["model"]);
                model = (Video)TempData["model"];
            }
            if(TempData["error"]!=null)
            TempData["error"] = TempData["error"];
            var querry_typczytania =
                from typ in _db_czytania.STypCzytania
                select typ.STypCzytania;
            ViewData["typCzytania_list"] = querry_typczytania.ToList();
            return View("Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Video model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var qValidateTypCzytania = _db_czytania.Videos.Where(m => m.Data == model.Data && m.TypCzytania == model.TypCzytania);
                    if (qValidateTypCzytania.Any()==false)
                    {
                        _db_czytania.Videos.Add(model);
                        _db_czytania.SaveChanges();
                        TempData["success"] = "Nowy rekord został  pomyślnie dodany.";
                        TempData["model"] = JsonConvert.SerializeObject(model);
                        TempData["date"] = model.Data.Date.ToString("dd/MM/yyyy");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var querry_typczytania =
                        from typ in _db_czytania.STypCzytania
                        select typ.STypCzytania;
                        ViewData["typCzytania_list"] = querry_typczytania.ToList();
                        TempData["error"] = "Istnieje już rekord dla podanej daty i typu czytania!";
                        TempData["model"] = JsonConvert.SerializeObject(model);

                        return RedirectToAction("Add");
                    }
                }
                catch (Exception)
                {
                    var querry_typczytania =
                    from typ in _db_czytania.STypCzytania
                    select typ.STypCzytania;
                    ViewData["typCzytania_list"] = querry_typczytania.ToList();
                    ViewData["error"] = "Something went wrong please retry.";
                    return RedirectToAction("Add");
                }
            }
            else
            {
                var querry_typczytania =
                from typ in _db_czytania.STypCzytania
                select typ.STypCzytania;
                ViewData["typCzytania_list"] = querry_typczytania.ToList();
            }
            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            ModelState.Clear();
            if (id==null)
            {
                return NotFound();
            }
            var querry = _db_czytania.Videos.Find(id);
            Video model = querry;
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Video? model)
        {
            if(ModelState.IsValid)
            {
                var model_check=_db_czytania.Videos.Find(model.IdVideo);
                if(JsonConvert.SerializeObject(model) == JsonConvert.SerializeObject(model_check))
                {
                    try
                    {
                        _db_czytania.Videos.Remove(model_check);
                        _db_czytania.SaveChanges();
                        TempData["success"] = "Rekord został pomyślnie usunięty.";
                        TempData["date"] = model.Data.Date.ToString("dd/MM/yyyy");
                        return RedirectToAction("Index");
                    }
                    catch(Exception)
                    {
                        TempData["error"] = "Wystąpił błąd proszę spróbować ponownie.";
                        TempData["date"] = model.Data.Date.ToString("dd/MM/yyyy");
                        //return RedirectToAction("Index");
                        throw;
                    }
                    }
                TempData["error"] = "Wystąpił błąd, możliwe, że rekord został zmodyfikowany.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Wystąpił błąd, proszę spróbować ponownie.";
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            ModelState.Clear();
            if(TempData["id"]!=null)
                id=(int)TempData["id"];
            if (id == null)
                return NotFound();

            var querry_typczytania =
                            from typ in _db_czytania.STypCzytania
                            select typ.STypCzytania;
            ViewData["typCzytania_list"] = querry_typczytania.ToList();
            var querry = _db_czytania.Videos.Find(id);
            Video model = querry;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Video model)
        {
            if(ModelState.IsValid)
            {
                var model_check = _db_czytania.Videos.Find(model.IdVideo);
                if(model_check!=null && model_check.RowVersion==model.RowVersion)
                {
                    model_check.Data = model.Data;
                    if(model_check.TypCzytania!=model.TypCzytania)
                    {
                        var check_exists = _db_czytania.Videos.Where(m=>m.Data==model.Data && m.TypCzytania==model.TypCzytania);
                        if(check_exists.Any())
                        {

                            var querry_typczytania =
                            from typ in _db_czytania.STypCzytania
                            select typ.STypCzytania;
                            ViewData["typCzytania_list"] = querry_typczytania.ToList();
                            TempData["error"] = "Istnieje już rekord dla podanej daty i typu czytania!";
                            TempData["id"] = model.IdVideo;
                            return RedirectToAction("Edit");
                        }
                    }
                    model_check.TypCzytania = model.TypCzytania;
                    model_check.YoutubeId = model.YoutubeId;
                    model_check.RowVersion = DateTime.Now;
                    _db_czytania.Videos.Update(model_check);
                    _db_czytania.SaveChanges();
                    TempData["date"] = model.Data.ToString("dd/MM/yyyy");
                    TempData["success"] = "Pomyślnie zmodyfikowano rekord.";
                    return RedirectToAction("Index");
                }
                TempData["date"] = model.Data.ToString("dd/MM/yyyy");
                TempData["error"] = "Rekord uległ zmianie przed zakończeniem operacji, spróbuj ponownie.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Wystąpił błąd, proszę spróbować ponownie.";
            return RedirectToAction("Index");
        }
    }
}

