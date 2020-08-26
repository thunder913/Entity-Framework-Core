using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class User
    {
        public User() 
        {
            this.Bets = new HashSet<Bet>();
        }
        public int UserId { get; set; }
        [Required]
        public decimal Balance { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Username { get; set; }

        public ICollection<Bet> Bets { get; set; }
    }
}
