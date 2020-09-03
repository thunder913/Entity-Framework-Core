
using System;
using System.Collections.Generic;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class GameDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ReleaseDate { get; set; }
        public string Developer { get; set; }
        public string Genre { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
