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
        public IActionResult EmailTemplates(EmailTemplate template = null)
        {
            if (template != null)
                ViewData["ActiveEmailTemplate"] = template;
            else
                ViewData["ActiveEmailTemplate"] = new EmailTemplate() { ID = 0, Code = "testas", Text = "testas", Description = "testas", From = "te", Title = "tess" };

            return View(GetObject(DataType.EmailTemplate));
        }
        private object GetObject(DataType type)
        {
            //Dictionary<string, string> dataToChange = new Dictionary<string, string>();
            //dataToChange.Add("pavadinimas", "UZS1111");
            //try
            //{
            //    EmailHelper.SendMail("vidmantaszuk@gmail.com", "EmailTemplateCode", dataToChange);
            //}
            //catch (Exception e) 
            //{
            //    PageLog log = new PageLog();
            //    log.UserID = int.Parse((string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")) ? "0": HttpContext.Session.GetString("UserID")));
            //    log.Description = e.Message;
            //    log.Category = "SystemError";
            //    log.Url = "PageSystemController.GetObject";
            //    log.Date = DateTime.Now;
            //    log.SaveToDataBase();
            //}

            switch (type)
            {
                case DataType.PageConfig:
                    return ReturnPageConfigs();
                case DataType.EmailTemplate:
                    return ReturnEmailTemplates();
                case DataType.PageLog:
                    return ReturnPageLog();
                default:
                    return null;
            }
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

        protected List<PageLog> ReturnPageLog()
        {
            DataView dw = DataSource.SelectData("Puslapio_Statistika_SelectAll", new string[] { });
            List<PageLog> logList = new List<PageLog>();
            foreach (DataRow row in dw.Table.Rows)
            {
                PageLog log = new PageLog();
                log.PageLogID = Convert.ToInt32(row["StatistikaID"].ToString());
                log.UserID = Convert.ToInt32((string.IsNullOrEmpty(row["NaudotojasID"].ToString())?"0": row["NaudotojasID"].ToString()));
                log.Description = row["Aprasymas"].ToString();
                log.Category = row["Kategorija"].ToString();
                log.Url = row["Nuoroda"].ToString();
                log.Date = DateTime.Parse(row["Data"].ToString());
                logList.Add(log);
            }
            return logList;
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


        public IActionResult UpdateEmail(EmailTemplate template)
        {
            ViewData["ActiveEmailTemplate"] = template;
            return RedirectToAction("EmailTemplates", ViewData["ActiveEmailTemplate"]);
        }
    }
}
