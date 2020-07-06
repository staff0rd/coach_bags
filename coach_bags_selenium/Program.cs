using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace coach_bags_selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            var driver = new ChromeDriver(options);
            driver.ExecuteChromeCommand("Network.setUserAgentOverride", new System.Collections.Generic.Dictionary<string, object> {
                { "userAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.53 Safari/537.36" }
            });
            driver.ExecuteChromeCommand("Page.addScriptToEvaluateOnNewDocument", new System.Collections.Generic.Dictionary<string, object> {
                { "source", @"
                    Object.defineProperty(navigator, 'webdriver', {
                    get: () => undefined
                    })" 
                }
            });

            WebDriverWait _wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            try {
                var url= "https://coachaustralia.com/on/demandware.store/Sites-au-coach-Site/en_AU/Search-UpdateGrid?cgid=sale-womens_sale-bags&start=0&sz=10";
                //var url = "https://coachaustralia.com/catalog/sale/womens-sale/bags/";
                //var url ="https://shop.coles.com.au/search/resources/store/20501/productview/2364711P";
                driver.Navigate().GoToUrl(url);
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
