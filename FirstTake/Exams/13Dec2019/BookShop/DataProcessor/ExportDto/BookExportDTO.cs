using System.Xml.Serialization;
using System.Xml;
using System;

namespace BookShop.DataProcessor.ExportDto
{
    [XmlType("Book")]
    public class BookXMLExportDTO
    {
        [XmlAttribute("Pages")]
        public int Pages { get; set; }
        
        public string Name { get; set; }

        public string Date { get; set; }

        
    
    }
}
