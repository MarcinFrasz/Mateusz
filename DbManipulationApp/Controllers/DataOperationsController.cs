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
            var querry_typczytaniaSelect = _db_czytania.STypCzytania.Select(m=>m.STypCzytania);

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
        [HttpPost]
        public IActionResult VideoUpdateSelected(string Id)
        {
            int index = Convert.ToInt32(Id);
            var querry_getvid = _db_czytania.Videos.Where(m=>m.IdVideo==index);
            VideoViewModel model = new();
            model.Videos_list = querry_getvid.ToList();
            if(model.Videos_list.Count==1)
            {
                model.Current_video.IdVideo = model.Videos_list[0].IdVideo;
                model.Current_video.Data = model.Videos_list[0].Data;
                model.Current_video.TypCzytania = model.Videos_list[0].TypCzytania;
                model.Current_video.YoutubeId = model.Videos_list[0].YoutubeId;
            }
            model.Czytania_list = _db_czytania.STypCzytania.Select(m => m.STypCzytania).ToList();
            return PartialView("_VideoDisplay",model);
        }
    }
}
