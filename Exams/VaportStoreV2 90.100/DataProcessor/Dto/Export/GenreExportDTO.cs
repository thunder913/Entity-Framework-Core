using System.Collections.Generic;

namespace VaporStore.DataProcessor.Dto.Export
{
    public class GenreExportDTO
    {
        public int Id { get; set; }
        public string Genre { get; set; }
        public List<GameExportDTO> Games { get; set; } = new List<GameExportDTO>();

        public int TotalPlayers { get; set; }
    }
}
