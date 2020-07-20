using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace coach_bags_selenium.Data
{

    public class Product
    {
        public string Link { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Price { get; set; }
        public decimal Savings { get; set; }
        public string Id { get; set; }
        [NotMapped]
        public int SavingsPercent => 100 - (int)Math.Round(SalePrice / Price * 100, 0);
        public DateTime? LastPostedUtc { get; set; }
        public DateTime? LastUpdatedUtc { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Image { get; set; }
        public Category Category { get; set; }
    }
}