namespace Faxai.Models.ReviewModels
{
    public class ReviewModel
    {
        public int? ID { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }
        public int AuthorID { get; set; }
        public int ProductID { get; set; }

        public string? AuthorName { get; set; }

    }
}

