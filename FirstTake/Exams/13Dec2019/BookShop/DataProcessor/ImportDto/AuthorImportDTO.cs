using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShop.DataProcessor.ImportDto
    {
    public class AuthorImportDTO
    {
        [MinLength(3),MaxLength(30)][Required]
        public string FirstName { get; set; }
        [MinLength(3), MaxLength(30)][Required]
        public string LastName { get; set; }
        [Required][RegularExpression(@"^[0-9]{3}-[0-9]{3}-[0-9]{4}$")]
        public string Phone { get; set; }
        [EmailAddress][Required]
        public string Email { get; set; }

        [JsonProperty("Books")]
        
        public List<BookIdDTO> Books { get; set; } = new List<BookIdDTO>();
    }

    public class BookIdDTO 
    {
        [Required]
        public int? Id { get; set; }
    }
    
}
