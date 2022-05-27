using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DbManipulationApp.Controllers
{
    public class DataOperationsController : Controller
    {
        private readonly DbManipulationAppContext _db_identity;
        private readonly czytaniaContext _db_czytania;
        public DataOperationsController(DbManipulationAppContext db_identity, czytaniaContext db_czytania)
        {
            _db_identity = db_identity;
            _db_czytania = db_czytania;
        }
        public IActionResult Video(VideoViewModel model)
        {
            ModelState.Clear();
            if (model.Date==DateTime.MinValue)
            {
                model.Date = DateTime.Now.Date;
            }
            var querry_videosSelect = _db_czytania.Videos.Where(m => m.Data == model.Date.Date);
            model.Videos = querry_videosSelect;
            return View(model);
        }
        public IActionResult VideoD(string date)
        {
            ModelState.Clear();
            VideoViewModel model = new VideoViewModel();
            //var date_in = Convert.ToDateTime(id);
            DateTime date_in;

            if (date != null)
                date_in = DateTime.ParseExact(date, "dd/MM/yyyy", null);
            else
                date_in = DateTime.Now;

            var querry_videosSelect = _db_czytania.Videos.Where(m => m.Data.Date == date_in.Date);
            model.Date = date_in.Date;
            model.Videos = querry_videosSelect;
            return View("Video",model);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult VideoDatePicked(string date)
        {
            ModelState.Clear();           
            try
            {
                var currentDate = Convert.ToDateTime(date);
                var querry = from vid in _db_czytania.Videos
                             where vid.Data == currentDate
                             select vid;
                return PartialView("_VideoDisplay", querry);
            }
            catch(Exception)
            {
                ModelState.AddModelError("Current_video.Data", "Format daty jest nieprawidłowy.");
            }         
           return PartialView("_VideoDisplay");
        }
        [HttpGet]
        public IActionResult VideoAdd()
        {
            var querry_typczytania =
                from typ in _db_czytania.STypCzytania
                select typ.STypCzytania;
            List<string> typCzytania_list = querry_typczytania.ToList();
            ViewData["typCzytania_list"] = typCzytania_list;
            return View();
        }
        [HttpPost]
        public IActionResult VideoAdd(Video model)
        {
            return RedirectToAction("VideoD", new { @date = model.Data.ToString("dd/MM/yyyy") }); 
        }
    }
}
