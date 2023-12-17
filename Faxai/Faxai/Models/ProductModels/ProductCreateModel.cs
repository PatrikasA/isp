using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Faxai.Models.ProductModels
{
    public class ProductCreateModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Pavadinimas yra privalomas")]
        public string Pavadinimas { get; set; }

        [Required(ErrorMessage = "Kaina yra privaloma")]
        public decimal Kaina { get; set; }

        [Display(Name = "Kiekis Sandelyje")]
        public int Kiekis_Sandelyje { get; set; }

        public string Aprasymas { get; set; }

        [Display(Name = "Zemiausia Kaina Per 10d")]
        public decimal Zemiausia_Kaina_Per_10d { get; set; }

        public string Kategorija { get; set; }
    }

}

