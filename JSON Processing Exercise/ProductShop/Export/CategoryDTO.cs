using Newtonsoft.Json;

namespace ProductShop.Export
{
    public class CategoryDTO
    {
        [JsonProperty("category")]
        public string Name { get; set; }
        [JsonProperty("productsCount")]
        public int ProductsCount { get; set; }
        
        [JsonProperty("averagePrice")]
        public string AveragePrice { get; set; }

        [JsonProperty("totalRevenue")]
        public string Revenue { get; set; }

    }
}
