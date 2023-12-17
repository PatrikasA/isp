using Faxai.Helper;
using Faxai.Models.ShoppingCartModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Faxai.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult ShoppingCart()
        {
            return View();
        }

        private object GetObject()
        {
            DataView dw = DataSource.ExecuteSelectSQL("SELECT Pavadinimas, Aprasymas, Kaina, Vertinimo_Vidurkis FROM Preke");
            List<ProductModel> products = new List<ProductModel>();
            foreach (DataRowView drv in dw)
            {
                DataRow dr = drv.Row; // Retrieve the DataRow from DataRowView
                ProductModel product = new ProductModel();
                product.Name = dr["Pavadinimas"].ToString();
                product.Description = dr["Aprasymas"].ToString();
                product.Price = Convert.ToDouble(dr["Kaina"]);
                product.Rating = Convert.ToDouble(dr["Vertinimo_Vidurkis"]);
                products.Add(product);
            }
            return products;
        }

        public IActionResult GetProductDetails(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                return BadRequest("Product name cannot be empty.");
            }

            try
            {
                using (var connection = new SqlConnection(Helper.Constants.ConnectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("SELECT Pavadinimas, Aprasymas, Kaina, Vertinimo_Vidurkis FROM Preke WHERE Pavadinimas = @productName", connection);
                    command.Parameters.AddWithValue("@productName", productName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Generate HTML for the product details
                            var productDetails = $@"
                            <tr>
                                <td>{reader["Pavadinimas"]}</td>
                                <td>{reader["Aprasymas"]}</td>
                                <td>{reader["Kaina"]}</td>
                                <td>{reader["Vertinimo_Vidurkis"]}</td>
                                <td><button class='btn btn-danger'>Ištrinti</button></td>
                            </tr>";

                            return Content(productDetails, "text/html");
                        }
                        else
                        {
                            return NotFound("Product not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
