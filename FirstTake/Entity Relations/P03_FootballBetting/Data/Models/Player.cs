using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class Player
    {
        public Player()
        {
            this.PlayerStatistics = new HashSet<PlayerStatistic>();
        }
        public int PlayerId { get; set; }
        [Required]
        public bool IsInjured { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int PositionId { get; set; }
        [Required]
        public int SquadNumber { get; set; }
        [Required]
        public int TeamId { get; set; }
        public Team Team { get; set; }

        public Position Position { get; set; }

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; }
    }
}
