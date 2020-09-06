using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    class UserImportDTO
    {
        [RegularExpression(@"^[A-Z][a-z]* [A-Z][a-z]*$")]
        [MinLength(1)]
        public string FullName { get; set; }
        [MinLength(3),MaxLength(20)]
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Range(3,103)]
        public int Age { get; set; }

        public CardImportDTO[] Cards { get; set; }
    }
}
