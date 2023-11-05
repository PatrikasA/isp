﻿using Faxai.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Faxai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult HomePage()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login()
        {
            return RedirectToAction("HomePage");
        }

        public IActionResult RegisterView()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register()
        {
            return RedirectToAction("HomePage");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}