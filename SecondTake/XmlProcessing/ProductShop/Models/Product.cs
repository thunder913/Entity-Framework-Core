namespace ProductShop.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Product
    {
        public Product()
        {
            this.CategoryProducts = new List<CategoryProduct>();
        }

        [XmlIgnore]
        public int Id { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("sellerId")]
        public int SellerId { get; set; }
        public User Seller { get; set; }

        [XmlElement("buyerId")]
        public int? BuyerId { get; set; }
        public User Buyer { get; set; }

        [XmlIgnore]
        public ICollection<CategoryProduct> CategoryProducts { get; set; }
    }
}