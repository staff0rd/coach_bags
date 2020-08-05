using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("tweet")]
    public class ScrapeAndTweetCommand : IRequest
    {
        [Option("-c|--category", CommandOptionType.SingleValue)]
        public Category Category { get; set; }

        [Option("--count", CommandOptionType.SingleOrNoValue)]
        public int Count { get; set; } = 30;

        [Option("-p|--prepare-only", CommandOptionType.NoValue)]
        public bool PrepareOnly { get; set; }
    }
}