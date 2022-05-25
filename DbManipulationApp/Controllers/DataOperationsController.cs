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
        public DataOperationsController(DbManipulationAppContext db_identity,czytaniaContext db_czytania)
            {
                _db_identity=db_identity;
                _db_czytania = db_czytania;
            }
        [HttpGet]
        public IActionResult Video(VideoViewModel model)
        {
            ModelState.Clear();
            model.Current_video.Data = DateTime.Now;
            var querry_videosSelect = _db_czytania.Videos.Where(m => m.Data == model.Current_video.Data);
            var querry_typczytaniaSelect =
                from typ in _db_czytania.STypCzytania
                select typ.STypCzytania;
            model.Videos_list = querry_videosSelect.ToList<Video>();
            model.Czytania_list = querry_typczytaniaSelect.ToList<string>();
            return View(model); 
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult VideoDatePicked(string jsonString, string date)
        {
            ModelState.Clear();
            VideoViewModel model = JsonConvert.DeserializeObject<VideoViewModel>(jsonString);
                model.Current_video.Data = Convert.ToDateTime(date);

            var querry = from vid in _db_czytania.Videos
                         where vid.Data == model.Current_video.Data
                         select vid;
            model.Videos_list = querry.ToList<Video>();
         
           return PartialView("_VideoDisplay",model);
        }
    }
}
