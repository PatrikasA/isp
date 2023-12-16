using Faxai.Helper;
using Faxai.Models;
using Faxai.Models.PageSystemModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;

namespace Faxai.Controllers
{
    public class PageSystemController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private enum DataType
        {
            PageLog,
            PageConfig,
            EmailTemplate
        }
        public PageSystemController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult PageConfig()
        {
            return View(GetObject(DataType.PageConfig));
        }
        public IActionResult PageAnalitics()
        {
            return View(GetObject(DataType.PageLog));
        }
        public IActionResult EmailTemplates()
        {
            return View(GetObject(DataType.EmailTemplate));
        }
        private object GetObject(DataType type)
        {
            DataView dw = DataSource.SelectData("testas", new[] { "@One", "1" });

            DataView dw2 = DataSource.ExecuteSelectSQL("SELECT * FROM Kategorija");

            bool IsValid = DataSource.UpdateData("UpdateTestas", new[] { "@testas", "WowwssProcedure" });

            bool isValid2 = DataSource.UpdateDataSQL("UPDATE Kategorija SET Pavadinimas = 'Wows'");

            // Simulate data retrieval from a database
            switch (type)
            {
                case DataType.PageConfig:
                    return new List<PageConfig>{
                        new PageConfig { ID = 1,Code="PrimaryColor",Value="1",Description="tai pavyzdinis kodas" },
                        new PageConfig { ID = 2,Code="MaxFailedLogins",Value="5",Description="tai pavyzdinis kodas" },
                        new PageConfig { ID = 3,Code="ConnectionStrg",Value="1",Description="tai pavyzdinis kodas" }};
                case DataType.EmailTemplate:
                    return new List<EmailTemplate> {
                        new EmailTemplate{ ID = 1, Code = "Main", Description ="testas", From ="testas@testas.com", Title="Laiskas", Text="su pagarba, testas" },
                        new EmailTemplate{ ID = 2, Code = "Alert", Description ="testas", From ="testas@testas.com", Title="Pranesimas", Text="su pagarba, testas" },
                        new EmailTemplate{ ID = 3, Code = "AlerWithLogin", Description ="testas", From ="testas@testas.com", Title="Ispejimas", Text="su pagarba, testas" }
                    };
                case DataType.PageLog:
                    return new List<PageLog> { 
                        new PageLog { PageLogID = 1, Date=DateTime.Today, Description = "testas", Url="http://localhost", Category = "testas", UserID = 1, } ,
                        new PageLog { PageLogID = 2, Date=DateTime.Today, Description = "testas", Url="http://localhost", Category = "testas", UserID = 1, },
                        new PageLog { PageLogID = 3, Date=DateTime.Today, Description = "testas", Url="http://localhost", Category = "testas", UserID = 1, }
                    };

            }
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
