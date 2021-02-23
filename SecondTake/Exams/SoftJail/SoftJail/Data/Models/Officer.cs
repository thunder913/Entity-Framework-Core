using SoftJail.Data.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Officer
    {
        public int Id { get; set; }

        [MinLength(3), MaxLength(30)]
        [Required]
        public string FullName { get; set; }
    
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }
    
        [Required]
        public Position Position { get; set; }
    
        [Required]
        public Weapon Weapon { get; set; }

        public int DepartmentId { get; set; }
    
        public Department Department { get; set; }

        public ICollection<OfficerPrisoner> OfficerPrisoners { get; set; } = new HashSet<OfficerPrisoner>();

    }
}
