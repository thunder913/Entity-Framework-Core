using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class PurchaseImportDTO
    {
        [XmlAttribute("title")]
        [Required]
        public string Title { get; set; }
        [Required]
        [XmlElement("Type")]
        public string Type { get; set; }
        [XmlElement("Key")]
        [RegularExpression(@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        [Required]
        public string ProductKey { get; set; }
        [XmlElement("Card")]
        [Required]
        public string CardNumber { get; set; }
        [XmlElement("Date")]
        [Required]
        public string Date { get; set; }
    }
}
