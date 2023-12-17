using Faxai.Models.PageSystemModels;
using System.Net;
using System.Net.Mail;

namespace Faxai.Helper
{
    public static class EmailHelper
    {
        public static void SendMail(string toUserEmail, string EmailTemplateCode, Dictionary<string, string> dataToChange)
        {
            SendMail(toUserEmail, Constants.SMTPUserEmail, EmailTemplateCode, dataToChange);
        }
        public static void SendMail(string toUserEmail, string fromUserEmail, string EmailTemplateCode,Dictionary<string,string> dataToChange)
        {
            EmailTemplate template = new EmailTemplate();
            template.FillDataByCode(EmailTemplateCode);

            if (template.ID == 0)
                throw new Exception(message: "Laiško šablonas nerastas, prašome peržiūrėti ar šablonas egzistuoja!!!");
            string body = template.Text;
            foreach (var pair in dataToChange)
            {
                string placeholder = $"[{pair.Key}]";
                if (body.Contains(placeholder))
                {
                    body = body.Replace(placeholder, pair.Value);
                }
            }

            SendMail(toUserEmail, fromUserEmail, template.Title, body);
        }

        public static void SendMail(string toUserEmail, string fromUserEmail, string title,string body)
        {
            SmtpClient smtpClient = new SmtpClient(Constants.SMTPServer, Constants.SMTPPort); 
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(Constants.SMTPUserEmail , Constants.SMTPUserPassword); 
            smtpClient.EnableSsl = true; 
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromUserEmail);
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(toUserEmail); 
            mailMessage.Subject = title;
            mailMessage.Body = body;

            EmailLog emailLog = new EmailLog();
            emailLog.From = fromUserEmail;
            emailLog.Body = body;
            emailLog.DateSent = DateTime.Now;
            emailLog.IsSent = false;

            try
            {
                smtpClient.Send(mailMessage);
                emailLog.IsSent = true;
                emailLog.Response = string.Empty;

            }
            catch (Exception ex)
            {
                emailLog.IsSent = false;
                emailLog.Response = ex.Message;
            }

            emailLog.SaveToDataBase();
        }
    }
}
