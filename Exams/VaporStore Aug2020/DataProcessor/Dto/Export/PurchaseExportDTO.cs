using System;
using System.Xml.Serialization;
namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("Purchase")]
    public class PurchaseExportDTO
    {
        [XmlElement("Card")]
        public string Card { get; set; }
        [XmlElement("Cvc")]
        public string Cvc { get; set; }
        [XmlElement("Date")]
        public string Date { get; set; }
        public GamePurchaseExportDTO Game { get; set; }
    }

    [XmlType("Game")]
    public class GamePurchaseExportDTO 
    {
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlElement("Genre")]
        public string Genre { get; set; }
        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
