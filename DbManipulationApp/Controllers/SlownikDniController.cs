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
            if (TempData["dzienliturgiczny"] == null)
            {
                var querry = _db_czytania.SlownikDnis.ToList();
                if (querry.Any())
                    model.SlownikDni_list = querry;
            }
            else
            {
                model.Dzien_liturgiczny = TempData["dzienliturgiczny"].ToString();
                var querry = _db_czytania.SlownikDnis.Where(m=>m.DzienLiturgiczny==model.Dzien_liturgiczny);
                model.SlownikDni_list = (IEnumerable<SlownikDni>)querry;
            }            
           return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexDzienLiturgicznyPicked(string dzien_liturgiczny)
        {
            try
            {
                var querry_search =
                    from item in _db_czytania.SlownikDnis
                    where item.DzienLiturgiczny.Contains(dzien_liturgiczny)
                    select item;
                 return PartialView("_SlownikDniDisplay", querry_search);
            }
            catch(Exception)
            {
                TempData["error"] = "Wystąpił błąd podczas wyszukiwania.";
                return PartialView("_SlownikDniDisplay",Enumerable.Empty<SlownikDni>());
            }
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
                try
                {
                    var querry_check = _db_czytania.SlownikDnis.Where(m=>m.DzienLiturgiczny==model.DzienLiturgiczny);
                    if (querry_check.Any()==false)
                    {
                        model.RowVersion = DateTime.Now;
                        _db_czytania.SlownikDnis.Add(model);
                        _db_czytania.SaveChanges();
                        TempData["success"] = "Pomyślnie dodano nowy rekord.";
                        TempData["dzienliturgiczny"] = model.DzienLiturgiczny;
                        return RedirectToAction("Index");
                    }
                    TempData["error"] = "Podany dzień liturgiczny już istnieje!";
                    TempData["model"] = JsonConvert.SerializeObject(model);
                    return RedirectToAction("Add");
                }
                catch(Exception)
                {
                    TempData["error"] = "Wystąpił błąd, proszę spróbować ponownie.";
                    //return RedirectToAction("Index");
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(SlownikDni model)
        {
            if (ModelState.IsValid)
            {
                try 
                { 
                    
                }
                catch(Exception)
                {

                }
            }
            return RedirectToAction("Index");
        }
    }
}
