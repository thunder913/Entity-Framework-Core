using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class Prisoner
    {
        public Prisoner() 
        {
            this.PrisonerOfficers = new HashSet<OfficerPrisoner>();
            this.Mails = new HashSet<Mail>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)][MinLength(3)]
        public string FullName { get; set; }

        [Required]
        public string Nickname { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public DateTime IncarcerationDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal? Bail { get; set; }
        [ForeignKey(nameof(Cell))]
        public int? CellId { get; set; }
        public Cell Cell { get; set; }
        [ForeignKey(nameof(Mail))]
        public ICollection<Mail> Mails { get; set; }
        
        public ICollection<OfficerPrisoner> PrisonerOfficers { get; set; }
       
    }
}