using Faxai.Models.ProductModels;

namespace Faxai.Models.ReviewModels
{
    public class ReviewViewModel
    {
        public ProductViewModel Product { get; set; }
        public List<ReviewModel> Reviews { get; set; }
        public ReviewModel NewReview { get; set; }

    }
}
