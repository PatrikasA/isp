﻿using Faxai.Helper;
using Faxai.Models;
using Faxai.Models.ProductModels;
using Faxai.Models.UserModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace Faxai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult HomePage()
        {
            // Gaukite prekių sąrašą iš duomenų bazės
            DataView prekiuDuomenys = DataSource.ExecuteSelectSQL("SELECT * FROM Preke");

            // Paverčiame duomenis modeliu, kad galėtume juos perduoti į peržiūrą
            var prekiuSarasas = prekiuDuomenys.ToTable().AsEnumerable().Select(row => new ProductViewModel
            {
                ID = row.Field<int>("ID"),
                Pavadinimas = row.Field<string>("Pavadinimas"),
                Kaina = row.Field<decimal>("Kaina"),
                Kiekis_Sandelyje = row.Field<int>("Kiekis_Sandelyje"),
                Aprasymas = row.Field<string>("Aprasymas"),
                Zemiausia_Kaina_Per_10d = row.Field<decimal>("Zemiausia_Kaina_Per_10d")
                // Papildomai pridėkite kitus stulpelius pagal poreikį
            }).ToList();

            return View("HomePage", prekiuSarasas); 
        }


        [HttpPost]
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            try
            {
                // Construct the SQL command to retrieve user data
                // Remember to use parameterized queries to prevent SQL injection
                string commandText = $"SELECT * FROM Naudotojas WHERE El_Pastas = '{email}' AND Slaptazodis = '{password}'";

                DataView userData = DataSource.ExecuteSelectSQL(commandText);
                bool isBanned = LoginHelper.IsBannedUser(email);
                if (userData != null && userData.Table.Rows.Count > 0 && !isBanned)
                {
                    // User is found. Create session and redirect to a secure page
                    var userRow = userData.Table.Rows[0];
                    if (Convert.ToInt32(userRow["KrepselisID"]) == -1)
                    {
                        string insertCartQuery = $"INSERT INTO Krepselis (Apmoketas, Bendras_Kiekis, Bendra_Kaina, Issaugotas) VALUES (0, 0, 0, 0); SELECT SCOPE_IDENTITY();";

                        int krepselisID = 0;
                        try
                        {
                            using (var connection = new SqlConnection(Helper.Constants.ConnectionString))
                            {
                                connection.Open();
                                var command = new SqlCommand(insertCartQuery, connection);
                                krepselisID = Convert.ToInt32(command.ExecuteScalar());
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception occurred during Cart Creation");
                            ModelState.AddModelError("", "An error occurred during Cart Creation");
                        }

                        int userID = Convert.ToInt32(userRow["ID"]);

                        // Update Naudotojas to associate the user with the newly generated Krepselis.ID
                        string updateUserCartID = $"UPDATE Naudotojas SET KrepselisID = {krepselisID} WHERE ID = {userID}";
                        DataSource.UpdateDataSQL(updateUserCartID);
                    }

                    HttpContext.Session.SetString("UserID", userRow["ID"].ToString());
                    HttpContext.Session.SetString("UserName", userRow["Vardas"].ToString());
                    // Store other necessary details in the session as needed

                    return RedirectToAction("HomePage"); // Redirect to a secure page
                }
                else
                {
                    // User not found or password is incorrect
                    if(!isBanned) LoginHelper.SendToUserWarning($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}", email); //
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during login");
                ModelState.AddModelError("", "An error occurred during login.");
                return View("Index");
            }
        }


        public IActionResult RegisterView()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    StringBuilder commandTextBuilder = new StringBuilder();
                    commandTextBuilder.Append("INSERT INTO Naudotojas (Vardas, Pavarde, Gimimo_data, El_Pastas, Telefonas, Banko_Saskaita, Pasirinkimas_Gauti_Naujienlaiskius, Slaptazodis, TipasID, KrepselisID) ");
                    commandTextBuilder.AppendFormat("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, {9})",
                        userModel.Name, userModel.Surname, userModel.BirthDay.ToString("yyyy-MM-dd"),
                        userModel.Email, userModel.Phone, userModel.BankAccount,
                        userModel.Newsletter ? 1 : 0, userModel.Password, 1, -1);

                    string commandText = commandTextBuilder.ToString();
                    // Note: It's important to use parameterized queries to prevent SQL injection

                    bool success = DataSource.UpdateDataSQL(commandText);

                    if (success)
                    {
                        return View("Index");
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

            return View("RegisterView", userModel);
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}