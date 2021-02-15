using System;
using System.Xml.Serialization;
namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("Purchase")]
    public class PurchaseExportDTO
    {
        public string Card { get; set; }
        public string Cvc { get; set; }
        public string Date { get; set; }
        public GamePurchaseDTO Game { get; set; }
    }

    [XmlType("Game")]
    public class GamePurchaseDTO 
    {
        [XmlAttribute("title")]
        public string Title { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
    }
}
