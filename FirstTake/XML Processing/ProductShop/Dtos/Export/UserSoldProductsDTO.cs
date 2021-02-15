using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class UserSoldProductsDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public List<SoldProduct> SoldProducts { get; set; } = new List<SoldProduct>();
    }

    [XmlType("Product")]
    public class SoldProduct 
    {
        [XmlElement("name")]
        public string Name { get; set; }
    
        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
