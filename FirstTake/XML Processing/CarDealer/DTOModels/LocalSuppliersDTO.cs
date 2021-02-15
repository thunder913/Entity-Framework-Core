using System.Xml.Serialization;

namespace CarDealer.DTOModels
{
    [XmlType("suplier")]
    public class LocalSuppliersDTO
    {
        [XmlAttribute("id")]
        public int Id{ get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("parts-count")]
        public int PartsCount { get; set; }
        
    }
}
