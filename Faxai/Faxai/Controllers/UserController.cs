﻿using Faxai.Helper;
using Faxai.Models.UserModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace Faxai.Controllers
{
    public class UserController : Controller
    {

        public IActionResult UserInfo()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                // Handle the case where there is no logged-in user
                return RedirectToAction("Login"); // Redirect to login page or another appropriate page
            }

            // Fetch user information from the database
            var userInfo = GetUserById(userId);
            if (userInfo != null)
            {
                return View(userInfo);
            }
            else
            {
                // Handle the case where user data could not be retrieved
                return RedirectToAction("Error"); // Redirect to an error page or handle differently
            }
        }

        private UserModel GetUserById(string userId)
        {
            string commandText = $"SELECT * FROM Naudotojas WHERE ID = '{userId}'"; // Use parameterized query to prevent SQL injection
            DataView userData = DataSource.ExecuteSelectSQL(commandText);

            if (userData != null && userData.Table.Rows.Count > 0)
            {
                var row = userData.Table.Rows[0];
                return new UserModel
                {
                    UserID = Convert.ToInt32(row["ID"]),
                    Name = row["Vardas"].ToString(),
                    Surname = row["Pavarde"].ToString(),
                    Email = row["El_Pastas"].ToString(),
                    Phone = row["Telefonas"].ToString(),
                    BankAccount = row["Banko_Saskaita"].ToString(),
                    Newsletter = Convert.ToBoolean(row["Pasirinkimas_Gauti_Naujienlaiskius"]),
                    BirthDay = Convert.ToDateTime(row["Gimimo_data"]),
                    Password = row["Slaptazodis"].ToString(), // Va taip ir nulaužė KTU'na
                    TypeID = Convert.ToInt32(row["TipasID"])
                };
            }
            return null;
        }

        public IActionResult Orders() 
        {
            return View();
        }

        public IActionResult DeliveryForm(int? id)
        {
            if (id.HasValue)
            {
                // Fetch the specific delivery instruction
                var model = GetDeliveryInstructionById(id.Value);
                return View(model);
            }

            return View(new DeliveryInfoModel());
        }

        private DeliveryInfoModel GetDeliveryInstructionById(int id)
        {
            string commandText = $"SELECT * FROM Pristatymo_Instrukcijos WHERE ID = {id}";
            // Use parameterized queries to prevent SQL injection

            DataView dataView = DataSource.ExecuteSelectSQL(commandText);
            if (dataView != null && dataView.Table.Rows.Count > 0)
            {
                var row = dataView.Table.Rows[0];
                return new DeliveryInfoModel
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Address = row["Adresas"].ToString(),
                    CommentForCourier = row["Komentaras_Kurjeriui"].ToString(),
                    UserId = Convert.ToInt32(row["NaudotojasID"])
                };
            }
            return null;
        }


        public IActionResult DeliveryInfo()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login"); // Redirect to login if not logged in
            }

            var deliveryInstructions = GetDeliveryInstructionsForUser(userId);
            return View(deliveryInstructions);
        }
        [HttpPost]
        public IActionResult SubmitDeliveryInfo(DeliveryInfoModel model)
        {
            if (ModelState.IsValid)
            {
                string commandText;
                if (model.ID == 0)
                {
                    // Insert new delivery instruction
                    commandText = $"INSERT INTO Pristatymo_Instrukcijos (Adresas, Komentaras_Kurjeriui, NaudotojasID) VALUES ('{model.Address}', '{model.CommentForCourier}', '{HttpContext.Session.GetString("UserID")}')";
                }
                else
                {
                    // Update existing delivery instruction
                    commandText = $"UPDATE Pristatymo_Instrukcijos SET Adresas = '{model.Address}', Komentaras_Kurjeriui = '{model.CommentForCourier}' WHERE ID = {model.ID}";
                }

                // Note: Use parameterized queries to prevent SQL injection

                bool success = DataSource.UpdateDataSQL(commandText);

                if (success)
                {
                    return RedirectToAction("DeliveryInfo");
                }
                else
                {
                    // Handle failure
                }
            }

            return View("DeliveryForm", model);
        }

        [HttpPost]
        public IActionResult ConfirmDeleteUser(int userId)
        {
            try
            {
                // Construct SQL command to delete the user
                string commandText = $"DELETE FROM Naudotojas WHERE ID = {userId}";
                // Note: Use parameterized queries to prevent SQL injection

                bool success = DataSource.UpdateDataSQL(commandText);

                if (success)
                {
                    // Redirect to a confirmation page or user list
                    return RedirectToAction("Index"); // Replace with your user list page
                }
                else
                {
                    // Handle failure (e.g., display error message)
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the error and display an error message)
            }

            // Return back to the delete confirmation view if deletion fails
            return View("DeleteUser", userId);
        }

        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return RedirectToAction("Login"); // Redirect to login page if authentication failed

            var emailClaim = authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (emailClaim == null)
                return RedirectToAction("Login"); // No email claim found, redirect to login page

            // Search in the database for the user with this email
            string commandText = $"SELECT * FROM Naudotojas WHERE El_Pastas = '{emailClaim}'";
            // Note: Use parameterized queries to prevent SQL injection
            DataView userData = DataSource.ExecuteSelectSQL(commandText);

            if (userData != null && userData.Table.Rows.Count > 0)
            {
                // User is found in local database. Set session.
                var userRow = userData.Table.Rows[0];
                HttpContext.Session.SetString("UserID", userRow["ID"].ToString());
                HttpContext.Session.SetString("UserName", userRow["Vardas"].ToString());
                // Add other necessary session settings as needed

                return Redirect("/Home/HomePage"); // Redirect to home page or other secure page
            }
            else
            {
                // User not found, optionally create a new user record in your database
                // ...

                // After creating the user, set session variables as above
                // ...

                return Redirect("/Home/HomePage"); // Redirect to home page or other secure page
            }
        }



        [HttpPost]
        public IActionResult SubmitNewDeliveryInfo(DeliveryInfoModel model)
        {
            if (ModelState.IsValid)
            {
                // Construct SQL command to insert new delivery instruction
                string commandText = $"INSERT INTO Pristatymo_Instrukcijos (Adresas, Komentaras_Kurjeriui, NaudotojasID) VALUES ('{model.Address}', '{model.CommentForCourier}', '{HttpContext.Session.GetString("UserID")}')";
                // Note: Use parameterized queries to prevent SQL injection
                Console.WriteLine(commandText);
                bool success = DataSource.UpdateDataSQL(commandText);

                if (success)
                {
                    // Redirect to DeliveryInfo page after successful insertion
                    return RedirectToAction("DeliveryInfo");
                }
                else
                {
                    // Handle failure (e.g., display error message)
                }
            }

            // If model state is invalid or insertion fails, return to the same view
            return View("DeliveryForm", model);
        }

        private List<DeliveryInfoModel> GetDeliveryInstructionsForUser(string userId)
        {
            List<DeliveryInfoModel> instructions = new List<DeliveryInfoModel>();
            string commandText = $"SELECT * FROM Pristatymo_Instrukcijos WHERE NaudotojasID = '{userId}'"; // Use parameterized queries to prevent SQL injection

            DataView dataView = DataSource.ExecuteSelectSQL(commandText);
            if (dataView != null)
            {
                foreach (DataRow row in dataView.Table.Rows)
                {
                    var instruction = new DeliveryInfoModel
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        Address = row["Adresas"].ToString(),
                        CommentForCourier = row["Komentaras_Kurjeriui"].ToString(),
                        UserId = Convert.ToInt32(row["NaudotojasID"])
                    };
                    instructions.Add(instruction);
                }
            }
            return instructions;
        }
        public IActionResult SubmitDeliveryInfo()
        {
            return RedirectToAction("DeliveryInfo");

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

    }
}
