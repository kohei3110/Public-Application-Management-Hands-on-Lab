using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web.Models
{
    public class Product
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("ListPrice")]
        public decimal ListPrice { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("subCategory")]
        public string SubCategory { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("sellStartDate")]
        public DateTime SellStartDate { get; set; }
    }
}
