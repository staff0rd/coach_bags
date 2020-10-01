using System;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace coach_bags_selenium
{

    [Command("scrape")]
    public class ScrapeCommand : Request
    {
        [Option("-c|--category", CommandOptionType.SingleValue)]
        public string CategoryName
        {
            set { Category = Enumeration.FromDisplayName<ProductCategory>(value); }
            get { return Category.DisplayName; }
        }
        public ProductCategory Category { get; set; }
        

        [Option("--count", CommandOptionType.SingleOrNoValue)]
        public int Count { get; set; } = 30;   
    }
}