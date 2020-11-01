using System;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;

namespace coach_bags_selenium
{
    [Command("twitterimages")]
    public class GetTwitterImagesCommand : Request<GetTwitterImagesCommandResult>
    {
        [Option("--source", CommandOptionType.MultipleValue)]
        public string[] Sources { get; set; }
        [Option("--now|-n", CommandOptionType.SingleValue)]
        public DateTime Now { get; set; }
        [Option("-c|--category", CommandOptionType.SingleValue)]
        public string CategoryName
        {
            set { Category = Enumeration.FromDisplayName<ProductCategory>(value); }
            get { return Category.DisplayName; }
        }
        public ProductCategory Category { get; set; }
    }
}