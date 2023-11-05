using Microsoft.AspNetCore.Mvc;

namespace Faxai.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Product()
        {
            return View();
        }
    }
}
