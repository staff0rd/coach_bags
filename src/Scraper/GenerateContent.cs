using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace coach_bags_selenium
{
    [Command("generate")]
    public class GenerateContent {
        private readonly IConfiguration _config;

        public GenerateContent(IConfiguration config)
        {
            _config = config;
        }

        public void OnExecute(IConfiguration config)
        {
            var connectionString = _config.GetConnectionString("Postgres");
            var db = new coach_bags_selenium.Data.DatabaseContext(connectionString);

            db.Products
                .Where(p => p.LastPostedUtc.HasValue)
                .OrderByDescending(p => p.LastPostedUtc)
                .ToList()
                .GroupBy(g => $"/{g.LastPostedUtc.Value.Year}/{g.LastPostedUtc.Value.Month}/{g.LastPostedUtc.Value.Day}")
                .ToList()
                .ForEach(g => {
                    Console.WriteLine(g.Key);
                    g.ToList().ForEach(p => Console.WriteLine($"\t{p.Name}"));
                });


        }
    }
}
