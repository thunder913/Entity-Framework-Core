using BookShop.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
namespace BookShop.DataProcessor.ImportDto
{
    [XmlType("Book")]
    public class BookImportDTO
    {
        [MinLength(3),MaxLength(30)][Required]
        public string Name { get; set; }
        [Range(1,3)][Required]
        public int Genre { get; set; }
        [Range(typeof(decimal),"0.01", "79228162514264337593543950335")][Required]
        public decimal Price { get; set; }
        [Range(50,5000)][Required]
        public int Pages { get; set; }
        [Required]
        public string PublishedOn { get; set; }
    }
}
