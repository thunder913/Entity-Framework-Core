using System.Xml.Serialization;

namespace BookShop.DataProcessor.ExportDto
{
    [XmlType("Book")]
    public class BookExportDto
    {
        [XmlAttribute("Pages")]
        public int Pages { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    }
}
