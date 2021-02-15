using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class GameImportDTO
    {
        [JsonProperty("Name")]
        [Required]
        public string GameName { get; set; }
        [Range(typeof(decimal),"0", "79228162514264337593543950335")]
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string ReleaseDate { get; set; }
        [JsonProperty("Developer")]
        [Required]
        public string DeveloperName { get; set; }
        
        [JsonProperty("Genre")]
        [Required]
        public string GenreName { get; set; }
        [JsonProperty("Tags")]
        [Required]
        public List<string> Tags { get; set; }
    }
}
