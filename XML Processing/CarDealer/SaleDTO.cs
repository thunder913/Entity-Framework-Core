using CarDealer.DTOModels;
using System.Xml.Serialization;

namespace CarDealer
{
    [XmlType("sale")]
    public class SaleDTO
    {
        [XmlElement("car")]
        public SaleCarExportDTO Car { get; set; }
        
        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public string PriceAfterDiscount { get; set; }
    }

    [XmlType("car")]
    public class SaleCarExportDTO 
    {
        [XmlAttribute("make")]
        public string Make { get; set; }
        [XmlAttribute("model")]
        public string Model { get; set; }
        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
