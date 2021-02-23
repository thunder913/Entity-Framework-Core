using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("Product")]
    public class ProductDto
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("price")]
        public decimal? Price { get; set; }
    }
    [XmlType("SoldProducts")]
    public class SoldProducts
    {
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlArray("products")]
        public ProductDto[] Products { get; set; }
    }
    [XmlType("User")]
    public class UserProductDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }
        [XmlElement("lastName")]
        public string LastName { get; set; }
        [XmlElement("age")]
        public int? Age { get; set; }
        [XmlElement("SoldProducts")]
        public SoldProducts SoldProducts { get; set; }
    }
    public class UserCountDto
    {
        [XmlElement("count")]
        public int? Count { get; set; }
        [XmlArray("users")]
        public UserProductDto[] Users{ get; set; }
    }
}
