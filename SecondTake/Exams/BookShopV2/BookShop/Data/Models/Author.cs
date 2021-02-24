using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Data.Models
{
    public class Author
    {
        public int Id { get; set; }
        
        [Required]
        [MinLength(3), MaxLength(30)]
        public string FirstName { get; set; }
        
        [Required]
        [MinLength(3), MaxLength(30)]
        public string LastName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }
    
        [Required]
        [RegularExpression(@"^[0-9]{3}-[0-9]{3}-[0-9]{4}$")]
        public string Phone { get; set; }

        public ICollection<AuthorBook> AuthorsBooks { get; set; } = new HashSet<AuthorBook>();

    }
}
