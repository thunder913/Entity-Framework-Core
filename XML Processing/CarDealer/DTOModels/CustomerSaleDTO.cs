using System.Xml.Serialization;

namespace CarDealer.DTOModels
{
    [XmlType("customer")]
    public class CustomerSaleDTO
    {
        [XmlAttribute("full-name")]
        public string Name { get; set; }
        [XmlAttribute("bought-cars")]
        public int BoughtCarsCount { get; set; }
        [XmlAttribute("spent-money")]
        public decimal MoneySpent { get; set; }
    }
}
