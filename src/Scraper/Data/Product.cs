using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace coach_bags_selenium.Data
{

    public class Product
    {
        public string Link { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Price { get; set; }
        public decimal Savings { get; set; }
        
        [JsonIgnore]
        public string Id { get; set; }
        [NotMapped]
        public int SavingsPercent => 100 - (int)Math.Round(SalePrice / Price * 100, 0);

        [JsonPropertyName("postedUtc")]
        public DateTime? LastPostedUtc { get; set; }

        [JsonIgnore]
        public DateTime? LastUpdatedUtc { get; set; }

        [JsonIgnore]
        public DateTime CreatedUtc { get; set; }

        [JsonIgnore]
        public string Image { get; set; }
        public Category Category { get; set; }
        public string[] Images {get; set;}
    }
}