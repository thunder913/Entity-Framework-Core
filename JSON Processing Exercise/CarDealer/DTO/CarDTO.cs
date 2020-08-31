using CarDealer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarDealer.DTO
{
    public class CarDTO
    {
        public int Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        public ICollection<Sale> Sales { get; set; }

        public ICollection<int> PartsId { get; set; } = new List<int>();
    }
}
