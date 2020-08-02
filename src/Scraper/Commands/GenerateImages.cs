using System;
using System.Data.SqlClient;
using System.Linq;
using coach_bags_selenium.Data;
using Dapper;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace coach_bags_selenium
{

    [Command("images")]
    public class GenerateImages {
        private readonly DataFactory _data;

        public GenerateImages(DataFactory data)
        {
            _data = data;
        }

        class Linked : Data.Product {
            public DateTime NextPostedUtc { get; set; }
        }

        public void OnExecute(IConfiguration config)
        {
            var product = _data.GetDatabaseContext().ChooseProductToTweet(Category.FwrdBags, DateTime.Today.AddDays(-1).Date);
            Console.WriteLine(product.Image);
            Console.WriteLine(product.Link);
        }
    }
}
