using System.Collections.Generic;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class UserDTO
    {
        public UserDTO() 
        {
            Cards = new HashSet<CardDTO>();
        }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public virtual ICollection<CardDTO> Cards { get; set; }
    }
}
