using McMaster.Extensions.CommandLineUtils;

namespace coach_bags_selenium
{

    [Command("scrapeurl")]
    public class ScrapeUrlCommand : Request
    {
        [Option("-u|--url", CommandOptionType.SingleValue)]
        public string Url { get; set; }
    }
}