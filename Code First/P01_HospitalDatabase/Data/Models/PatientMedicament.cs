﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_HospitalDatabase.Data.Models
{
    public class PatientMedicament
    {
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int MedicamentId { get; set; }

        public Patient Patient { get; set; }
        public Medicament Medicament { get; set; }

    }
}
