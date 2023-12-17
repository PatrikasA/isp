using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Faxai.Models.ProductModels
{
    public class ProductViewModel
    {
        public int ID { get; set; }
        public string Pavadinimas { get; set; }
        public decimal Kaina { get; set; }
        public int Kiekis_Sandelyje { get; set; }
        public string Aprasymas { get; set; }
        public decimal Zemiausia_Kaina_Per_10d { get; set; }
    }
}
