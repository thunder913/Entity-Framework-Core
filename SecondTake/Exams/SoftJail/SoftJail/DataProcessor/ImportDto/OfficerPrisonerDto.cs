using SoftJail.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class OfficerDto
    {

        [MinLength(3), MaxLength(30)]
        [Required]
        [XmlElement("Name")]
        public string FullName { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [XmlElement("Money")]
        public decimal Salary { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public string Weapon { get; set; }

        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public PrisonerIdDto[] Prisoners { get; set; }
    }

    [XmlType("Prisoner")]
    public class PrisonerIdDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
