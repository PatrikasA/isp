using Microsoft.AspNetCore.Mvc;

namespace Faxai.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult Product()
        {
            return View();
        }
    }
}
