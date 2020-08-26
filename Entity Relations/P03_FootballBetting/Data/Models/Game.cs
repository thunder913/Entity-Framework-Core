using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace P03_FootballBetting.Data.Models
{
    public class Game
    {
        public Game() 
        {
            this.PlayerStatistics = new HashSet<PlayerStatistic>();
            this.Bets = new HashSet<Bet>();
        }
        public int GameId { get; set; }
        [Required]
        public decimal AwayTeamBetRate { get; set; }
        [Required]
        public int AwayTeamGoals { get; set; }
        [Required]
        public int AwayTeamId { get; set; }
                 [NotMapped]
        public Team AwayTeam { get; set; }
        [Required]
        public decimal DrawBetRate { get; set; }
        [Required]
        public decimal HomeTeamBetRate { get; set; }
        [Required]
        public int HomeTeamGoals { get; set; }
        [Required]
        public int HomeTeamId { get; set; }
                 [NotMapped]
        public Team HomeTeam { get; set; }
        [Required]
        public string Result { get; set; }
        [Required]
        public DateTime DateTime { get; set; }

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; }

        public ICollection<Bet> Bets { get; set; }
    }
}
