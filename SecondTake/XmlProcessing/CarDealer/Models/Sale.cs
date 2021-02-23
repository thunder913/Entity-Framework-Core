using System.Xml.Serialization;

namespace CarDealer.Models
{
    public class Sale
    {
        [XmlIgnore]
        public int Id { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("carId")]
        public int CarId { get; set; }
        [XmlIgnore]
        public Car Car { get; set; }

        [XmlElement("customerId")]
        public int CustomerId { get; set; }
        [XmlIgnore]
        public Customer Customer { get; set; }
    }
}
