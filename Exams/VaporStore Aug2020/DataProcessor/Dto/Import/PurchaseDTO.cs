using System.Xml.Serialization;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class PurchaseDTO
    {
        [XmlAttribute("title")]
        public string GameName { get; set; }
        [XmlElement("Type")]
        public PurchaseType Type { get; set; }
        [XmlElement("Key")]
        public string ProductKey { get; set; }
        [XmlElement("Date")]
        public string Date { get; set; }
        [XmlElement("Card")]
        public string Card { get; set; }
    }
}
