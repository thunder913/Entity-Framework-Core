using System.Collections.Generic;

namespace CarDealer.DTO
{
    public class CarImportDto
    {
        public string Make { get; set; }

        public string Model { get; set; }

        public int TravelledDistance { get; set; }

        public HashSet<int> PartsId { get; set; }
    }
}
