using Faxai.Helper;

namespace Faxai.Models.PageSystemModels
{
    public class PageLog
    {
        private string DataBaseName = "Puslapio_Statistika";
        public int PageLogID { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public int UserID { get; set; }
        public PageLog() 
        {
            PageLogID = 0;
            Date = DateTime.Now;
            Description = string.Empty;
            Url = string.Empty;
            Category = string.Empty;
            UserID = 0;
        }

        public bool SaveToDataBase()
        {
            return DataSource.UpdateData(DataBaseName + Constants.SQLInset,
                new[]
                {
                "@Data",Date.ToString(),
                "@Aprasymas", Description,
                "@Nuoroda", Url,
                "@Kategorija", Category,
                "@NaudotojasID", UserID.ToString(),
                }
                );
        }

    }

}
