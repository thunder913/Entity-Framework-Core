namespace ProductShop.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Category
    {
        public Category()
        {
            this.CategoryProducts = new List<CategoryProduct>();
        }

        [XmlIgnore]
        public int Id { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlIgnore]
        public ICollection<CategoryProduct> CategoryProducts { get; set; }
    }
}
