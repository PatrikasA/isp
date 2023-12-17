namespace Faxai.Models.ShoppingCartModels
{
    public class ProductModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public double LowestPrice { get; set; }
        public int CreatorID { get; set; }
    }
}
