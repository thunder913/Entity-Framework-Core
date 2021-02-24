using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class UserExportDto
    {
        [XmlAttribute("username")]
        public string UserName { get; set; }

        public PurchaseExportDto[] Purchases { get; set; }

        public decimal TotalSpent { get; set; }
    }

    [XmlType("Purchase")]
    public class PurchaseExportDto
    {
        public string Card { get; set; }

        public string Cvc { get; set; }

        public string Date { get; set; }

        public GameExportDto Game { get; set; }
        
    }

    [XmlType("Game")]
    public class GameExportDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }
        
        public string Genre { get; set; }

        public decimal Price { get; set; }
    }
}
