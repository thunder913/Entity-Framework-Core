using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P01_HospitalDatabase.Data.Models
{
    public class Medicament
    {
        public Medicament() 
        {
            this.Prescriptions = new HashSet<PatientMedicament>();
        }
        [Required]
        public int MedicamentId { get; set; }

        [MaxLength(50)][Required]
        public string Name { get; set; }

        public ICollection<PatientMedicament> Prescriptions { get; set; }
    }
}
