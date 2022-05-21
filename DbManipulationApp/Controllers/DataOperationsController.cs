using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;

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
            //model.Videos_list  = _db_czytania.Videos.ToList<Video>();
            var querry = from vid in _db_czytania.Videos
                         where vid.IdVideo == 81
                         select vid;
            model.Videos_list = querry.ToList<Video>();
            return View(model); 
        }
        [HttpPost]
        public IActionResult Video(VideoViewModel model,string submit)
        {
            model.Current_video.YoutubeId = "post";
            ModelState.Clear();
            return View(model);
        }
        [HttpPost]
        public IActionResult VideoDatePicked(DateTime date)
        {           
            VideoViewModel model = new();
            model.Current_video.Data = date;
            ModelState.Remove("YoutubeId");
            model.Current_video.YoutubeId = date.ToString();          
            ModelState.Clear();
            return PartialView("_VideoDisplay",model);
        }
    }
}
