using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    
    [XmlType("Category")]
    public class CategoryDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("count")]
        public int ProductsCount { get; set; }

        [XmlElement("averagePrice")]
        public decimal AveragePrice { get; set; }

        [XmlElement("totalRevenue")]
        public decimal TotalRevenue { get; set; }
    }
}
