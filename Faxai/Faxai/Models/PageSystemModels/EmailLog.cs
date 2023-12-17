using Faxai.Helper;
using static System.Net.Mime.MediaTypeNames;

namespace Faxai.Models.PageSystemModels
{
    public class EmailLog
    {
        private string DataBaseName = "Laisku_Istorija";
        public int ID { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public bool IsSent { get; set; }
        public string Response { get; set; }
        public DateTime DateSent { get; set; }
        public int UserID { get; set; }
        public EmailLog()
        {
            ID = 0;
            Body = string.Empty;
            From = string.Empty;
            IsSent = false;
            Response = string.Empty;
            UserID = 0;
        }

        public bool SaveToDataBase()
        {
            return DataSource.UpdateData(DataBaseName + Constants.SQLInset,
                new[]
                {
                    "@Turinys",Body,
                    "@Nuo", From,
                    "@Issiustas", (IsSent? "1":"0"),
                    "@Atsakas", Response,
                    "@Siuntimo_Data", DateSent.ToString(),
                    "@NaudotojasID",UserID.ToString()
                });
        }
    }
}
