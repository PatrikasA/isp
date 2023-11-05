namespace Faxai.Models.PageSystemModels
{
    public class PageLog
    {
        public int PageLogID { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public int UserID { get; set; }
    }
}
