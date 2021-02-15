using Newtonsoft.Json;

namespace ProductShop.Export
{
    public class ProductDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("seller")]
        public string SellerName { get; set; }
    }
}
