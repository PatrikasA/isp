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
            if(ViewData["ActiveEmailTemplate"] == null)
                ViewData["ActiveEmailTemplate"] = new EmailTemplate() { ID = 0, Code = "testas", Text = "testas", Description = "testas", From = "te", Title = "tess" };

            return View(GetObject(DataType.EmailTemplate));
        }
        private object GetObject(DataType type)
        {
            //DataView dw = DataSource.SelectData("testas", new[] { "@One", "1" });

            //DataView dw2 = DataSource.ExecuteSelectSQL("SELECT * FROM Kategorija");

            //bool IsValid = DataSource.UpdateData("UpdateTestas", new[] { "@testas", "WowwssProcedure" });

            //bool isValid2 = DataSource.UpdateDataSQL("UPDATE Kategorija SET Pavadinimas = 'Wows'");

            // Simulate data retrieval from a database
            switch (type)
            {
                case DataType.PageConfig:
                    return ReturnPageConfigs();
                case DataType.EmailTemplate:
                    return ReturnEmailTemplates();
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
        protected List<PageConfig> ReturnPageConfigs()
        {
            DataView dw = DataSource.SelectData("Puslaptio_Nustatymai_SelectAll", new string[] { });
            List<PageConfig> configList = new List<PageConfig>();
            foreach (DataRow row in dw.Table.Rows)
            {
                PageConfig config = new PageConfig();
                config.ID = Convert.ToInt32(row["ID"]);
                config.Code = row["Kodas"].ToString();
                config.Value = row["Reiksme"].ToString();
                config.Description = row["Aprasymas"].ToString();
                configList.Add(config);
            }
            return configList;
        }
        protected List<EmailTemplate> ReturnEmailTemplates()
        {
            DataView dw = DataSource.SelectData("Laisko_Sablonas_SelectAll", new string[] { });
            List<EmailTemplate> EmailList = new List<EmailTemplate>();
            foreach (DataRow row in dw.Table.Rows)
            {
                EmailTemplate config = new EmailTemplate();
                config.ID = Convert.ToInt32(row["ID"]);
                config.Description = row["Aprasymas"].ToString();
                config.Code = row["Kodas"].ToString();
                config.Title = row["Antraste"].ToString();
                config.Text = row["Turinys"].ToString();
                config.From = row["Nuo"].ToString();
                EmailList.Add(config);
            }
            return EmailList;
        }


        public IActionResult SavePageConfig(PageConfig config)
        {
            if (config.SaveToDataBase())
            {
                //Teigiamas pranešimas
            }
            else
            {
                //Neigiamas
            }
            return RedirectToAction("PageConfig");
        }
        public IActionResult SaveEmailTemplate(EmailTemplate template)
        {
            if (template.SaveToDataBase())
            {
                //Teigiamas pranešimas
            }
            else
            {
                //Neigiamas
            }
            return RedirectToAction("EmailTemplates");
        }

        [HttpPost]
        public IActionResult DeleteConfig(int id)
        {
            PageConfig conf = new PageConfig();conf.ID = id;
            if (conf.DeleteFromDataBase())
            {

            }
            else
            {

            }
            return RedirectToAction("PageConfig");
        }

        [HttpPost]
        public IActionResult DeleteEmail(int id)
        {
            EmailTemplate emailTemplate = new EmailTemplate(); emailTemplate.ID = id;
            if (emailTemplate.DeleteFromDataBase())
            {

            }
            else
            {

            }
            return RedirectToAction("EmailTemplates");
        }


        public IActionResult UpdateEmail(object test)
        {
            ViewData["ActiveEmailTemplate"] = new EmailTemplate() { ID = 1, Code = "testasjameeee", Text = "tesdasdasdasatas", Description = "tesaszadasdasdtas", From = "tasasse", Title = "tess" };
            return RedirectToAction("EmailTemplates");
        }
    }
}
