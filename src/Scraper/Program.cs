using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using coach_bags_selenium.Data;
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace coach_bags_selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("no-sandbox"); // need to run inside container
            
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

            try {
                var count = 300;
                var url= $"https://coachaustralia.com/on/demandware.store/Sites-au-coach-Site/en_AU/Search-UpdateGrid?cgid=sale-womens_sale-bags&start=0&sz={count}";

                driver.Navigate().GoToUrl(url);
                var products = 
                    driver.FindElementsByClassName("product")
                    .Select(p => new Product(p));
                Console.WriteLine($"Found {products.Count()} products");
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var db = new coach_bags_selenium.Data.DatabaseContext(args[0]);    
                db.Database.Migrate();

                var now = Save(db, products.Select(p => p.AsEntity));
                Random rand = new Random();
                var pendingProducts = db.Products
                    .Where(p => p.LastUpdatedUtc >= now && p.LastPostedUtc == null)
                    .ToArray();

                int index = rand.Next(pendingProducts.Length);

                var productToPost = pendingProducts.ElementAt(index);
                productToPost.LastPostedUtc = now;
                
                var src = products.Single(p => p.Id == productToPost.Id).Image;
                src.DownloadFileAsync("download", "image.jpg").Wait();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            finally {
                driver?.Quit();
            }
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
