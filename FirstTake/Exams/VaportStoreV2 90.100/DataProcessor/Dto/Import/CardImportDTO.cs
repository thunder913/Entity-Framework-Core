using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class CardImportDTO
    {
        [Required]
        [RegularExpression(@"^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$")]
        public string Number { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{3}$")]
        public string Cvc { get; set; }
        [Required]
        public string Type { get; set; }
    }
}
