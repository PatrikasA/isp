using Microsoft.AspNetCore.Mvc;

namespace Faxai.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult ShoppingCart()
        {
            return View();
        }
    }
}
