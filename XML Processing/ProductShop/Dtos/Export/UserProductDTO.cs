using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class UserProductDTO
    {
        [XmlIgnore]
        public List<UserDTO> AllUsers { get; set; } = new List<UserDTO>();

        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public List<UserDTO> Users { get; set; } = new List<UserDTO>();
    }

    [XmlType("User")]
    public class UserDTO 
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }
        [XmlElement("lastElement")]
        public string LastName { get; set; }
        [XmlElement("age")]
        public int? Age { get; set; }

        public SoldProducts SoldProducts { get; set; }
    }

    [XmlType("SoldProducts")]
    public class SoldProducts 
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public List<ProductsDTO> Products { get; set; } = new List<ProductsDTO>();
    }


    [XmlType("Product")]
    public class ProductsDTO 
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("price")]

        public decimal Price { get; set; }
    }
}
