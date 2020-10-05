using System;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("tweetonly")]
    public class TweetRandomProductCommand : Request
    {
        [Option("-c|--category", CommandOptionType.SingleValue)]
        public string CategoryName
        {
            set { Category = Enumeration.FromDisplayName<ProductCategory>(value); }
            get { return Category.DisplayName; }
        }
        public ProductCategory Category { get; set; }
        [Option("-s|--since", CommandOptionType.SingleValue)]
        public DateTime Since { get; set; }
        [Option("-p|--prepare-only", CommandOptionType.NoValue)]
        public bool PrepareOnly { get; set; }
    }
}