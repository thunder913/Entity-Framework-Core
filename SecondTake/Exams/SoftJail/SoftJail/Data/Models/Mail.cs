using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
   public class Mail
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
    
        [Required]
        public string Sender { get; set; }
    
        [RegularExpression(@"^[0-9 a-zA-Z]+str\.$")]
        [Required]
        public string Address { get; set; }
    
        public int PrisonerId { get; set; }
    
        public Prisoner Prisoner { get; set; }

    }
}
