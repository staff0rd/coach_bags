using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Chrome;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Models;

[assembly: UserSecretsIdAttribute("35c1247a-0256-4d98-b811-eb58b6162fd7")]
namespace coach_bags_selenium
{
    public class TwitterOptions
    {
        public string ConsumerSecret { get; set; }
        public string ConsumerKey { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }

    class Program
    {
        static Task Main(string[] args) => CreateHostBuilder().RunCommandLineApplicationAsync<Program>(args);
                
        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {

                });


        private IHostEnvironment _env;
        private IConfiguration _config;

        public Program(IHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public void OnExecute()
        {
            var twitterOptions = _config.GetSection("Twitter").Get<TwitterOptions>();
            var count = _config.GetValue<int>("Count");
            var connectionString = _config.GetConnectionString("Postgres");
            var db = new coach_bags_selenium.Data.DatabaseContext(connectionString);
            db.Database.Migrate();
            var driver = ConfigureDriver();

            try
            {
                IEnumerable<Product> products = GetProducts(driver, count);
                var now = Save(db, products.Select(p => p.AsEntity));

                Random rand = new Random();
                var pendingProducts = db.Products
                    .Where(p => p.LastUpdatedUtc >= now && p.LastPostedUtc == null)
                    .ToArray();

                int index = rand.Next(pendingProducts.Length);

                var productToPost = pendingProducts.ElementAt(index);
                productToPost.LastPostedUtc = now;

                var src = products.Single(p => p.Id == productToPost.Id).Image;
                var fileName = "image.jpg";
                var directory = "download";
                src.DownloadFileAsync(directory, fileName).Wait();

                Auth.SetUserCredentials(twitterOptions.ConsumerKey, twitterOptions.ConsumerSecret, twitterOptions.AccessToken, twitterOptions.AccessTokenSecret);

                var text = $"{productToPost.Name} - {productToPost.SavingsPercent}% off, was ${productToPost.Price}, now ${productToPost.SalePrice} - {productToPost.Link}";                

                byte[] file1 = File.ReadAllBytes(Path.Combine(directory, fileName));
                var media = Upload.UploadBinary(file1);
                var tweet = Tweet.PublishTweet(text, new PublishTweetOptionalParameters
                {
                    Medias = new List<IMedia> { media }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                driver?.Quit();
            }
        }

        private static IEnumerable<Product> GetProducts(ChromeDriver driver, int count)
        {
            var url = $"https://coachaustralia.com/on/demandware.store/Sites-au-coach-Site/en_AU/Search-UpdateGrid?cgid=sale-womens_sale-bags&start=0&sz={count}";
            driver.Navigate().GoToUrl(url);
            var products =
                driver.FindElementsByClassName("product")
                .Select(p => new Product(p));
            Console.WriteLine($"Found {products.Count()} products");
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return products;
        }

        private static ChromeDriver ConfigureDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("no-sandbox"); // needed to run inside container

            var driver = new ChromeDriver(options);
            driver.ExecuteChromeCommand("Network.setUserAgentOverride", new System.Collections.Generic.Dictionary<string, object> {
                { "userAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.53 Safari/537.36" }
            });
            driver.ExecuteChromeCommand("Page.addScriptToEvaluateOnNewDocument",
                new System.Collections.Generic.Dictionary<string, object> {
                    { "source", @"
                        Object.defineProperty(navigator, 'webdriver', {
                            get: () => undefined
                        })"
                    }
                 }
            );
            return driver;
        }

        private static DateTime Save(DatabaseContext db, IEnumerable<Data.Product> products)
        {
            var now = DateTime.UtcNow;

            foreach (var product in products)
            {
                product.LastUpdatedUtc = now;
                
                var existing = db.Products.FirstOrDefault(p => p.Id == product.Id);
                if (existing is null)
                {
                    product.CreatedUtc = now;
                    db.Products.Add(product);
                } else
                {
                    existing.Link = product.Link;
                    existing.Name = product.Name;
                    existing.SalePrice = product.SalePrice;
                    existing.Price = product.Price;
                    existing.Savings = product.Savings;
                    existing.LastUpdatedUtc = now;
                }
            }
            Console.WriteLine("Saving...");
            db.SaveChanges();
            return now;
        }
    }
}
