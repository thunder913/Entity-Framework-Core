using System.Xml.Serialization;

namespace CarDealer.DTOModels
{
    [XmlType("car")]
    public class BMWCarExport
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("model")]
        public string Model { get; set; }
        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
