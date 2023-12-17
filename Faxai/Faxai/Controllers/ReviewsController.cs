using Faxai.Helper;
using Faxai.Models;
using Faxai.Models.ReviewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Faxai.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ReviewsController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

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

        [HttpPost]
        public IActionResult CreateReview(ReviewModel reviewModel) // prekes id dinamiskai
        {
            if (ModelState.IsValid)
            {
                try
                {
                    reviewModel.AuthorID = int.Parse(HttpContext.Session.GetString("UserID"));
                    reviewModel.CreationDate = DateTime.Today;
                    reviewModel.ProductID = 1; ///

                    StringBuilder commandTextBuilder = new StringBuilder();
                    commandTextBuilder.Append("INSERT INTO Atsiliepimas (Ivertis, Komentaras, Sukurimo_Data, Redagavimo_Data, AutoriusID, PrekeID) ");
                    commandTextBuilder.AppendFormat("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                        reviewModel.Rating, reviewModel.Comment, reviewModel.CreationDate.ToString("yyyy-MM-dd"),
                        reviewModel.CreationDate.ToString("yyyy-MM-dd"), reviewModel.AuthorID, reviewModel.ProductID);

                    string commandText = commandTextBuilder.ToString();

                    bool success = DataSource.UpdateDataSQL(commandText);

                    if (success)
                    {
                        return RedirectToAction("Product", "Products");
                    }
                    else
                    {
                        _logger.LogError("Failed to register user: SQL command execution failed.");
                        ModelState.AddModelError("", "Registration failed due to a database error.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred during registration");
                    ModelState.AddModelError("", "An error occurred during registration.");
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid. Errors: {0}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ModelState.AddModelError("", "Please correct the errors and try again.");
            }

            return RedirectToAction("Product", "Products");
        }

    }
}
