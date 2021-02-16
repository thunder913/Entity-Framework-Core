
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class Town
    {
        public int TownId { get; set; }
        [Required]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();

    }
}
