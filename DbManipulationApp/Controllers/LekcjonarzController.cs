﻿using Microsoft.AspNetCore.Mvc;

namespace DbManipulationApp.Controllers
{
    public class LekcjonarzController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}