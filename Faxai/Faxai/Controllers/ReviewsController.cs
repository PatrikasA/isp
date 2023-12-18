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
        public IActionResult EditReviewForm(ReviewViewModel tupleModel, int id)
        {
            ReviewModel selectedReview = GetReviewById(id);
            tupleModel.NewReview = selectedReview;

            tupleModel.Product = GetProductById(selectedReview.ProductID);

            return View(tupleModel);
        }
        public IActionResult DeleteReviewForm(ReviewViewModel tupleModel, int id)
        {
            ReviewModel selectedReview = GetReviewById(id);
            tupleModel.NewReview = selectedReview;

            tupleModel.Product = GetProductById(selectedReview.ProductID);

            return View(tupleModel);
        }
        public IActionResult FullReview(ReviewViewModel tupleModel, int id)
        {
            ReviewModel selectedReview = GetReviewById(id);
            tupleModel.NewReview = selectedReview;

            tupleModel.Product = GetProductById(selectedReview.ProductID);

            return View(tupleModel);
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

        private ReviewModel GetReviewById(int id)
        {
            string sqlUzklausa = $@"SELECT A.*, CONCAT(N.Vardas, ' ', N.Pavarde) AS AutoriausVardas
                            FROM Atsiliepimas A
                            INNER JOIN Naudotojas N ON A.AutoriusID = N.ID
                            WHERE A.ID = {id}";
            DataView atsiliepimuDuomenys = DataSource.ExecuteSelectSQL(sqlUzklausa);

            var atsiliepimuSarasas = atsiliepimuDuomenys.ToTable().AsEnumerable().Select(row => new ReviewModel
            {
                ID = row.Field<int>("ID"),
                Rating = row.Field<int>("Ivertis"),
                Comment = row.Field<string>("Komentaras"),
                CreationDate = row.Field<DateTime>("Sukurimo_Data"),
                EditDate = row.Field<DateTime>("Redagavimo_Data"),
                AuthorID = row.Field<int>("AutoriusID"),
                ProductID = row.Field<int>("PrekeID"),
                AuthorName = row.Field<string>("AutoriausVardas")
            }).ToList();

            var atsiliepimas = atsiliepimuSarasas.FirstOrDefault();
            return atsiliepimas;
        }

        private ProductViewModel GetProductById(int id)
        {
            string sqlUzklausa = $"SELECT * FROM Preke WHERE ID = {id}";
            DataView prekiuDuomenys = DataSource.ExecuteSelectSQL(sqlUzklausa);

            var prekiuSarasas = prekiuDuomenys.ToTable().AsEnumerable().Select(row => new ProductViewModel
            {
                ID = row.Field<int>("ID"),
                Pavadinimas = row.Field<string>("Pavadinimas"),
                Kaina = row.Field<decimal>("Kaina"),
                Kiekis_Sandelyje = row.Field<int>("Kiekis_Sandelyje"),
                Aprasymas = row.Field<string>("Aprasymas"),
                Zemiausia_Kaina_Per_10d = row.Field<decimal>("Zemiausia_Kaina_Per_10d"),
                Kategorija = row.Field<string>("Kategorija")
            }).ToList();

            var preke = prekiuSarasas.FirstOrDefault();
            return preke;
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

        [HttpPost]
        public IActionResult EditReview(ReviewViewModel tupleModel, int id, int productID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Assuming GetReviewById is a method that retrieves a review by its ID
                    ReviewModel existingReview = GetReviewById(id);

                    if (existingReview != null)
                    {

                        existingReview.Rating = tupleModel.NewReview.Rating;
                        existingReview.Comment = tupleModel.NewReview.Comment;
                        existingReview.EditDate = DateTime.Today;

                        StringBuilder commandTextBuilder = new StringBuilder();
                        commandTextBuilder.Append("UPDATE Atsiliepimas SET ");
                        commandTextBuilder.AppendFormat("Ivertis = '{0}', ", tupleModel.NewReview.Rating);
                        commandTextBuilder.AppendFormat("Komentaras = '{0}', ", tupleModel.NewReview.Comment);
                        commandTextBuilder.AppendFormat("Redagavimo_Data = '{0}' ", existingReview.EditDate);
                        commandTextBuilder.AppendFormat("WHERE ID = {0}", id);

                        string commandText = commandTextBuilder.ToString();

                        bool success = DataSource.UpdateDataSQL(commandText);

                        if (success)
                        {
                            // Redirect to the "Details" action with the product ID
                            return RedirectToAction("Details", "Products", new { id = productID });
                        }
                        else
                        {
                            _logger.LogError("Failed to edit review: Database update failed.");
                            ModelState.AddModelError("", "Review editing failed due to a database error.");
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to edit review: Review not found.");
                        ModelState.AddModelError("", "Review not found.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred during review editing. Error: {0}", ex.Message);
                    ModelState.AddModelError("", "An error occurred during review editing.");
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid. Errors: {0}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ModelState.AddModelError("", "Please correct the errors and try again.");
            }

            // Return to the "Details" view with the product ID
            return RedirectToAction("Details", "Products", new { id = productID });
        }


        [HttpPost]
        public IActionResult DeleteReview(int id, int PrekesID)
        {
            try
            {
                string deleteUzklausa = $"DELETE FROM Atsiliepimas WHERE ID = {id}";
                if (DataSource.UpdateDataSQL(deleteUzklausa))
                {
                    return RedirectToAction("Details", "Products", new { id = PrekesID });
                }

                return RedirectToAction("Details", "Products", new { id = PrekesID });
                // Return an error response if the deletion fails
                //return StatusCode(500, "Failed to delete the review.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Serverio klaida: {ex.Message}");
            }
            //return RedirectToAction("Details", "Products", new { id = PrekesID });
        }

    }
}
