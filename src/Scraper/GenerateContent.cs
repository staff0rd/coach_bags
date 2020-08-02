using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace coach_bags_selenium
{
    [Command("generate")]
    public class GenerateContent {
        private readonly IConfiguration _config;

        public GenerateContent(IConfiguration config)
        {
            _config = config;
        }

        class Linked : Data.Product {
            public DateTime NextPostedUtc { get; set; }
        }

        private string QUERY = @"
        with cte as (
            SELECT * FROM products
            WHERE last_posted_utc IS NOT NULL
            ORDER BY last_posted_utc
        )
        SELECT *,
        LEAD(last_posted_utc,1) OVER (
                ORDER BY last_posted_utc
            ) next_posted_utc
        FROM cte
        ORDER BY last_posted_utc DESC";

        public void OnExecute(IConfiguration config)
        {
            var connectionString = _config.GetConnectionString("Postgres");
            var db = new coach_bags_selenium.Data.DatabaseContext(connectionString);
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Query<Linked>(QUERY)
                    .ToList()
                    .ForEach(p => Console.WriteLine($"\t{p.Name}"));
            }
        }
    }
}
