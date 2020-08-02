using System;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace coach_bags_selenium
{
    [Command("scrape")]
    public class ScrapeCommand : IRequest
    {
        public int Count { get; set; }
        public Category Category { get; set; }  

        public void OnExecute(IConfiguration config, IMediator mediator)
        {
            this.Category = Enum.Parse<Category>(config.GetValue<string>("Category"));
            this.Count = config.GetValue<int>("Count");
            mediator.Send(this);
        }
    }
}