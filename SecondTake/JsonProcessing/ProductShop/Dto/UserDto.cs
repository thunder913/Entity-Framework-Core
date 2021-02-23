using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProductShop.Dto
{
    public class UserDto
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("age")]
        public int? Age { get; set; }
        [JsonProperty("soldProducts")]
        public SoldProductsWithCount SoldProducts { get; set; }
    }
}
