﻿using Microsoft.AspNetCore.Mvc;
using Faxai.Models.ProductModels;
using System.Data.SqlClient;
using Faxai.Helper;
using Faxai.Models;
using Faxai.Models.UserModels;
using Faxai.Models.ReviewModels;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Faxai.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Product(ReviewViewModel tupleModel)
        {
            return View("Product", tupleModel);
        }
        public IActionResult CreateProduct()
        {
            return View();
        }

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
                    commandTextBuilder.Append("INSERT INTO Preke (Pavadinimas, Kaina, Kiekis_Sandelyje, Aprasymas, Vertinimo_Vidurkis, Zemiausia_Kaina_Per_10d, NaudotojasID, Kategorija) ");
                    commandTextBuilder.AppendFormat("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')",
                        productModel.Pavadinimas, productModel.Kaina, productModel.Kiekis_Sandelyje, productModel.Aprasymas, 0, productModel.Kaina, userId, productModel.Kategorija);

                    string commandText = commandTextBuilder.ToString();
                    bool success = DataSource.UpdateDataSQL(commandText);

                    if (success)
                    {
                        return View("DeleteProduct");
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
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            DataView prekesDuomenys = DataSource.ExecuteSelectSQL($"SELECT * FROM Preke WHERE ID = {id}");

            if (prekesDuomenys != null && prekesDuomenys.Count > 0)
            {
                DataRow prekesEilute = prekesDuomenys.ToTable().Rows[0];

                ProductViewModel preke = new ProductViewModel
                {
                    ID = Convert.ToInt32(prekesEilute["ID"]),
                    Pavadinimas = Convert.ToString(prekesEilute["Pavadinimas"]),
                    Kaina = Convert.ToDecimal(prekesEilute["Kaina"]),
                    Kiekis_Sandelyje = Convert.ToInt32(prekesEilute["Kiekis_Sandelyje"]),
                    Aprasymas = Convert.ToString(prekesEilute["Aprasymas"]),
                    Zemiausia_Kaina_Per_10d = Convert.ToDecimal(prekesEilute["Zemiausia_Kaina_Per_10d"]),
                    Kategorija = Convert.ToString(prekesEilute["Kategorija"]),
                };

                return View(preke);
            }
            else
            {
                // Jei prekė nerasta, grąžiname 404 Not Found
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult EditProduct(ProductViewModel preke)
        {
            if (ModelState.IsValid)
            {

                StringBuilder commandTextBuilder = new StringBuilder();
                commandTextBuilder.AppendFormat("UPDATE Preke SET Pavadinimas = '{0}', Kaina = '{1}', Kiekis_Sandelyje = '{2}', Aprasymas = '{3}' WHERE ID = {4}", preke.Pavadinimas, preke.Kaina, preke.Kiekis_Sandelyje, preke.Aprasymas, preke.ID);
                string commandText = commandTextBuilder.ToString();
                bool success = DataSource.UpdateDataSQL(commandText);

                if (success)
                {
                    if (preke.Kaina < preke.Zemiausia_Kaina_Per_10d)
                    {
                        StringBuilder commandTextBuilder1 = new StringBuilder();
                        commandTextBuilder1.AppendFormat("UPDATE Preke SET Zemiausia_Kaina_Per_10d = '{0}' WHERE ID = {1}", preke.Kaina, preke.ID);
                        string commandText1 = commandTextBuilder1.ToString();
                        var flag = DataSource.UpdateDataSQL(commandText1);
                        if (flag)
                        {

                            string sqlUzklausa = $"SELECT * FROM Naudotojas WHERE Pasirinkimas_Gauti_Naujienlaiskius = 1";
                            DataView prekiuDuomenys = DataSource.ExecuteSelectSQL(sqlUzklausa);

                            var emailList = prekiuDuomenys.ToTable().AsEnumerable().Select(row => new string (row.Field<string>("El_Pastas"))).ToList();

                            foreach(var email in emailList)
                            {
                                Dictionary<string, string> dataToChange = new Dictionary<string, string>();
                                string price = preke.Kaina.ToString();
                                dataToChange.Add("Prekes nauja kaina", price);
                                EmailHelper.SendMail(email, "Kaina", dataToChange);
                            }

                            return View("DeleteProduct");
                        }
                        else
                        {
                            return BadRequest("Nepavyko atnaujinti mažiasuios kainos");

                        }

                    }
                    return View("DeleteProduct");
                }
                else
                {
                    // Jei atnaujinimas nepavyksta, grąžiname klaidą
                    ModelState.AddModelError("", "Klaida atnaujinant prekę.");
                    return View(preke);
                }
            }
            else
            {
                return View(preke);
            }
        }
        public IActionResult DeleteProduct()
        {
            // Gauti naudotojo ID iš sesijos
            var userId = HttpContext.Session.GetString("UserID");

            if (userId == null)
            {
                return View("DeleteProduct");
            }
            else
            {
                string sqlUzklausa = $"SELECT * FROM Preke WHERE NaudotojasID = {userId}";
                DataView prekiuDuomenys = DataSource.ExecuteSelectSQL(sqlUzklausa);

                var prekiuSarasas = prekiuDuomenys.ToTable().AsEnumerable().Select(row => new ProductViewModel
                {
                    ID = row.Field<int>("ID"),
                    Pavadinimas = row.Field<string>("Pavadinimas"),
                    Kaina = row.Field<decimal>("Kaina"),
                    Kiekis_Sandelyje = row.Field<int>("Kiekis_Sandelyje"),
                    Aprasymas = row.Field<string>("Aprasymas"),
                    Zemiausia_Kaina_Per_10d = row.Field<decimal>("Zemiausia_Kaina_Per_10d")
                }).ToList();
                return View("DeleteProduct", prekiuSarasas);
            }

        }
        public IActionResult DeleteByID(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserID");

                string patikrinimoUzklausa = $"SELECT COUNT(*) FROM Preke WHERE ID = {id} AND NaudotojasID = {userId}";
                int prekesKiekis = Convert.ToInt32(DataSource.ExecuteSelectSQL(patikrinimoUzklausa)[0]["Column1"]);

                if (prekesKiekis > 0)
                {
                    string deleteUzklausa = $"DELETE FROM Preke WHERE ID = {id}";
                    if (DataSource.UpdateDataSQL(deleteUzklausa))
                    {
                        return View("DeleteProduct");
                    }

                    return Ok();
                }
                else
                {
                    // Jei prekė nepriklauso šiam naudotojui
                    return BadRequest("Ši prekė neegzistuoja arba nepriklauso jums.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Serverio klaida: {ex.Message}");
            }
        }
        public IActionResult Details(int id)
        {
            ProductViewModel preke = GetProductById(id);

            if (preke == null)
            {
                return RedirectToAction("Index");
            }

            ReviewModel reviewModel = new ReviewModel();

            List<ReviewModel> reviews = GetReviewsForProduct(id);

            ReviewViewModel tupleModel = new ReviewViewModel
            {
                Product = preke,
                Reviews = reviews,
                NewReview = reviewModel
            };

            return View("Product", tupleModel);
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

        private List<ReviewModel> GetReviewsForProduct(int productId)
        {

            // Use a SELECT query to retrieve reviews for the specified product ID
            string commandText = $"SELECT * FROM Atsiliepimas WHERE PrekeID = {productId}";

            // Execute the query and get the result
            DataView reviewsDataView = DataSource.ExecuteSelectSQL(commandText);

            // Convert the DataView to a List<ReviewModel>
            List<ReviewModel> reviews = reviewsDataView.ToTable().AsEnumerable().Select(row => new ReviewModel
            {
                // Map database columns to ReviewModel properties
                ID = Convert.ToInt32(row["ID"]),
                Rating = Convert.ToInt32(row["Ivertis"]),
                Comment = Convert.ToString(row["Komentaras"]),
                CreationDate = Convert.ToDateTime(row["Sukurimo_Data"]),
                EditDate = Convert.ToDateTime(row["Redagavimo_Data"]),
                AuthorID = Convert.ToInt32(row["AutoriusID"]),
                ProductID = Convert.ToInt32(row["PrekeID"]),
            }).ToList();

            return reviews;
        }

        public ActionResult Filter(string kategorija)
        {
            string kat = HttpContext.Request.Query["kategorija"];

            string sqlUzklausa = $"SELECT * FROM Preke WHERE Kategorija = '{kat}'";
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

            return View("Filter", prekiuSarasas);
        }
    }
}
