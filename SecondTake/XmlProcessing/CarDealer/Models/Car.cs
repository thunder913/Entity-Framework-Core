namespace CarDealer.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("car")]
    public class Car
    {
        public Car()
        {
            this.PartCars = new HashSet<PartCar>();
        }

        [XmlIgnore]
        public int Id { get; set; }

        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlIgnore]
        public ICollection<Sale> Sales { get; set; }
		
        [XmlIgnore]
        public ICollection<PartCar> PartCars { get; set; }
    }
}