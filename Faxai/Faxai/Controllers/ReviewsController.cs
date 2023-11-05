using Microsoft.AspNetCore.Mvc;

namespace Faxai.Controllers
{
    public class ReviewsController : Controller
    {
        public IActionResult CreateReviewForm()
        {
            return View();
        }
        public IActionResult EditReviewForm()
        {
            return View();
        }
        public IActionResult DeleteReviewForm()
        {
            return View();
        }
        public IActionResult FullReview()
        {
            return View();
        }
    }
}
