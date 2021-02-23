using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.DataProcessor.ImportDto
{
    public class PrisonerDto
    {

        [Required]
        [MinLength(3), MaxLength(20)]
        public string FullName { get; set; }

        [RegularExpression(@"^The [A-Z][a-z]+$")]
        [Required]
        public string Nickname { get; set; }

        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public string IncarcerationDate { get; set; }

        public string ReleaseDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Bail { get; set; }
        public int? CellId { get; set; }
        
        public MailDto[] Mails { get; set; }

    }

    public class MailDto
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [RegularExpression(@"^[0-9 a-zA-Z]+str\.$")]
        [Required]
        public string Address { get; set; }
    }
}
