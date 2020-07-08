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
                var count = 10;
                var url= $"https://coachaustralia.com/on/demandware.store/Sites-au-coach-Site/en_AU/Search-UpdateGrid?cgid=sale-womens_sale-bags&start=0&sz={count}";

                driver.Navigate().GoToUrl(url);
                var products = 
                    driver.FindElementsByClassName("product-tile-card")
                    .Select(p => new Product(p));
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                Console.WriteLine(JsonSerializer.Serialize(products, jsonOptions));
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
