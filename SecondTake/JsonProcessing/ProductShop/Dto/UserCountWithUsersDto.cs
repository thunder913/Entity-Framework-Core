using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProductShop.Dto
{
    public class UserCountWithUsersDto
    {
        [JsonProperty("usersCount")]
        public int UsersCount => this.Users.Count;
        [JsonProperty("users")]
        public List<UserDto> Users { get; set; } = new List<UserDto>();

    }
}
