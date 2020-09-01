namespace CarDealer.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Supplier
    {
        public Supplier()
        {
            this.Parts = new HashSet<Part>();
        }
        [XmlIgnore]
        public int Id { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("isImporter")]
        public bool IsImporter { get; set; }
        [XmlIgnore]
        public ICollection<Part> Parts { get; set; }
    }
}
