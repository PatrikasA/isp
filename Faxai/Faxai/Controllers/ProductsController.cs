using Microsoft.AspNetCore.Mvc;
using Faxai.Models.ProductModels;
using System.Data.SqlClient;
using Faxai.Helper;
using Faxai.Models;
using Faxai.Models.UserModels;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Faxai.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Product()
        {
            return View();
        }
        public IActionResult CreateProduct()
        {
            return View();
        }

        // POST: Products/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(ProductCreateModel productModel)
        {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var userId = HttpContext.Session.GetString("UserID");

                    StringBuilder commandTextBuilder = new StringBuilder();
                        commandTextBuilder.Append("INSERT INTO Preke (Pavadinimas, Kaina, Kiekis_Sandelyje, Aprasymas, Vertinimo_Vidurkis, Zemiausia_Kaina_Per_10d, NaudotojasID) ");
                        commandTextBuilder.AppendFormat("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                            productModel.Pavadinimas, productModel.Kaina, productModel.Kiekis_Sandelyje, productModel.Aprasymas, 0, productModel.Kaina, userId);

                        string commandText = commandTextBuilder.ToString();
                        // Note: It's important to use parameterized queries to prevent SQL injection

                        bool success = DataSource.UpdateDataSQL(commandText);

                        if (success)
                        {
                            return View("Product");
                        }
                        else
                        {
                            ModelState.AddModelError("", "PREKES NEPRIDEJO due to a database error.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred during registration.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please correct the errors and try again.");
                }
                return View("CreateProduct", productModel);
        }

        public IActionResult EditProduct()
        {
            return View();
        }
        public IActionResult DeleteProduct()
        {
            return View();
        }
        public IActionResult FullPoductReview()
        {
            return View();
        }
    }
}
