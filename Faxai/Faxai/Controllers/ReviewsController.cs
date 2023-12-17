using Faxai.Helper;
using Faxai.Models;
using Faxai.Models.ReviewModels;
using Faxai.Models.ProductModels;
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

        public List<ReviewModel> GetReviewsForProduct(int productId)
        {
            try
            {
                // Use a SELECT query to retrieve reviews for the specified product ID
                string commandText = $"SELECT * FROM Atsiliepimas WHERE PrekeID = {productId}";

                // Execute the query and get the result
                DataView reviewsDataView = DataSource.ExecuteSelectSQL(commandText);

                // Convert the DataView to a List<ReviewModel>
                List<ReviewModel> reviews = reviewsDataView.ToTable().AsEnumerable().Select(row => new ReviewModel
                {
                    // Map database columns to ReviewModel properties
                    Rating = Convert.ToInt32(row["Rating"]),
                    Comment = Convert.ToString(row["Comment"]),
                    // Add other properties as needed
                }).ToList();

                return reviews;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"Failed to retrieve reviews for product {productId}: {ex.Message}");
                // Handle the error, such as returning an empty list or rethrowing the exception
                throw;
            }
        }


        [HttpPost]
        public IActionResult CreateReview(ReviewViewModel tupleModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tupleModel.NewReview.AuthorID = int.Parse(HttpContext.Session.GetString("UserID"));
                    tupleModel.NewReview.CreationDate = DateTime.Today;
                    tupleModel.NewReview.ProductID = tupleModel.Product.ID;

                    StringBuilder commandTextBuilder = new StringBuilder();
                    commandTextBuilder.Append("INSERT INTO Atsiliepimas (Ivertis, Komentaras, Sukurimo_Data, Redagavimo_Data, AutoriusID, PrekeID) ");
                    commandTextBuilder.AppendFormat("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                        tupleModel.NewReview.Rating, tupleModel.NewReview.Comment, tupleModel.NewReview.CreationDate.ToString("yyyy-MM-dd"),
                        tupleModel.NewReview.CreationDate.ToString("yyyy-MM-dd"), tupleModel.NewReview.AuthorID, tupleModel.NewReview.ProductID);

                    // Add parameters to the SqlCommand or use a parameterized query with your ORM.

                    string commandText = commandTextBuilder.ToString();

                    bool success = DataSource.UpdateDataSQL(commandText);

                    if (success)
                    {
                        // Redirect to the "Details" action with the product ID
                        return RedirectToAction("Details", "Products", new { id = tupleModel.Product.ID });
                    }
                    else
                    {
                        _logger.LogError("Failed to create review: SQL command execution failed.");
                        ModelState.AddModelError("", "Review creation failed due to a database error.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred during review creation. SQL error: {0}", ex.InnerException?.Message);
                    ModelState.AddModelError("", "An error occurred during review creation.");
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid. Errors: {0}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ModelState.AddModelError("", "Please correct the errors and try again.");
            }

            // Return to the "Details" view with the product ID
            return RedirectToAction("Details", "Products", new { id = tupleModel.Product.ID });
        }

    }
}
