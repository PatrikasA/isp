namespace Faxai.Models.ShoppingCartModels
{
    public class ShoppingCartM
    {
        public int ID { get; set; }
        public bool IsPaid { get; set; }
        public int TotalCount { get; set; }
        public double TotalPrice { get; set; }
        public bool IsSaved { get; set; }
    }
}
