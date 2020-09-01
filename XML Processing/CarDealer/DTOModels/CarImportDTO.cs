using System.Collections.Generic;
using System.Xml.Serialization;

namespace CarDealer.DTOModels
{
    [XmlType("Car")]
    public class CarImportDTO
    {

        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }

    }
    
}


