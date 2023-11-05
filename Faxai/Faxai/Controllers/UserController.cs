using Microsoft.AspNetCore.Mvc;

namespace Faxai.Controllers
{
    public class UserController : Controller
    {
        public IActionResult UserInfo()
        {

            return View();
        }

        public IActionResult Orders() 
        {
            return View();
        }

        public IActionResult DeliveryInfo()
        {
            return View();
        }
        public IActionResult SubmitDeliveryInfo()
        {
            return RedirectToAction("DeliveryInfo");

        }
        public IActionResult DeliveryForm()
        {
            return View();
        }

        public IActionResult BlockUser()
        {

            return View();
        }

        [HttpPost]
        public IActionResult ConfirmBlockUser()
        {
            return RedirectToAction("UserInfo");
        }

        public IActionResult DeleteUser()
        {

            return View();
        }

        [HttpPost]
        public IActionResult ConfirmDeleteUser()
        {
            return RedirectToAction("Index", "Home"); // Assuming you want to redirect to the HomePage after deletion
        }
    }
}
