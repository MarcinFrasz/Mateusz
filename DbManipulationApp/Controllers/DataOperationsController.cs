using DbManipulationApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{
    public class DataOperationsController : Controller
    {
        public IActionResult Video(Video video)
        {
            if (video.YoutubeId == null)
            {
                video.YoutubeId = "simp";
            }
            else
            {
                video.YoutubeId = "bad";
            }
            ModelState.Clear();
            return View(video);
        }
        [HttpPost]
        public IActionResult VideoDatePicked(DateTime date)
        {
            
            Video video = new();
            video.Data = date;
            video.YoutubeId = "something";
           
            ModelState.Clear();
            return RedirectToAction("Video",video);
        }
    }
}
