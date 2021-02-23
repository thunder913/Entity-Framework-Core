using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProductShop.Dto
{
    public class SoldProductsWithCount
    {
        [JsonProperty("count")]
        public int Count => this.Products.Count;

        [JsonProperty("products")]
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
