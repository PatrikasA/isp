using Faxai.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Faxai.Controllers
{
    public class PageSystemController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public PageSystemController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult PageConfig()
        {
            return View();
        }
        public IActionResult PageAnalitics()
        {
            return View();
        }
        public IActionResult EmailTemplates()
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
