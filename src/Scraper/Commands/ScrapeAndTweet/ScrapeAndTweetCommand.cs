using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;

namespace coach_bags_selenium
{
    [Command("tweet")]
    public class ScrapeAndTweetCommand : Request
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

        [Option("-p|--prepare-only", CommandOptionType.NoValue)]
        public bool PrepareOnly { get; set; }
    }
}