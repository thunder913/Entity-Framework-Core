using Newtonsoft.Json;
using System.Collections.Generic;

namespace VaporStore.DataProcessor.Dto.Export
{
    public class GenreExportDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("Genre")]
        public string Genre { get; set; }
        [JsonProperty("TotalPlayers")]
        public List<GameExportDTO> Games { get; set; } = new List<GameExportDTO>();
        public int TotalPurchases { get; set; }

    }
}
