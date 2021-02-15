using Newtonsoft.Json;
using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.Export
{
    public class UserSoldProductsDTO
    {
        public UserSoldProductsDTO() 
        { 
            this.ProductsSold = new HashSet<Products>(); 
        }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("soldProducts")]
        public ICollection<Products> ProductsSold { get; set; }
    }

    public class Products 
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("buyerFirstName")]
        public string BuyerFirstName { get; set; }
        [JsonProperty("buyerLastName")]
        public string BuyerLastName { get; set; }
    }
}
