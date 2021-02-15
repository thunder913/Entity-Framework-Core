namespace CarDealer.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

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
        [XmlElement("TravelledDistance")]
        public long TravelledDistance { get; set; }
        [XmlIgnore]
        public ICollection<Sale> Sales { get; set; }
		[XmlArray("parts")]
        public ICollection<PartCar> PartCars { get; set; }
    }
}