using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P03_FootballBetting.Data.Models
{
    public class Team
    {
        public Team() 
        {
            this.Players = new HashSet<Player>();
            this.HomeGames = new HashSet<Game>();
            this.AwayGames = new HashSet<Game>();
        }
        public int TeamId { get; set; }
        [Required]
        public decimal Budget { get; set; }
        [Required]
        public string Initials { get; set; }
        [Required]
        public string LogoUrl { get; set; }
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int PrimaryKitColorId { get; set; }
        [Required]
        public int SecondaryKitColorId { get; set; }
        [Required]
        public int TownId { get; set; }

        public ICollection<Player> Players { get; set; }
        [Required]
        public Color PrimaryKitColor { get; set; }
        [Required]
        public Color SecondaryKitColor { get; set; }
        public Town Town { get; set; }

        public ICollection<Game> HomeGames { get; set; }
        public ICollection<Game> AwayGames { get; set; }
    }
}
