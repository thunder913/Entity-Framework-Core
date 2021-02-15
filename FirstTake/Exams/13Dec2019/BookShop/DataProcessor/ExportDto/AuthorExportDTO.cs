using Newtonsoft.Json;
using System.Collections.Generic;

namespace BookShop.DataProcessor.ExportDto
{
    public class AuthorExportDTO
    {
        public string AuthorName { get; set; }

        [JsonProperty("Books")]
        public List<BookExportDTO> Books { get; set; } = new List<BookExportDTO>();
    }

    public class BookExportDTO
    { 
        public string BookName { get; set; }
        public string BookPrice { get; set; }
    }
}
