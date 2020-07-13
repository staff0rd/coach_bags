using System;
using System.Linq;
using System.Text.Json;
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
                    driver.FindElementsByClassName("product-tile-card")
                    .Select(p => new Product(p));
                Console.WriteLine($"Found {products.Count()} products");
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                //Console.WriteLine(JsonSerializer.Serialize(products, jsonOptions));
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            finally {
                driver?.Quit();
            }
        }
    }
}
