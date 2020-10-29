using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using coach_bags_selenium.Data;
using OpenQA.Selenium.Chrome;

public static class HtmlHelpers
{
    public static async Task<IEnumerable<Product>> LoopPages(int maxPages, Func<int, string> getUrl, Func<string, Task<IEnumerable<Product>>> getPage)
    {
        var loop = 0;
        var pageNumber = 0;
        var products = new List<Product>();
        do
        {
            loop++;
            var productCountPrior = products.Count;
            var url = getUrl(++pageNumber);
            var pageProducts = await getPage(url);
            foreach (var product in pageProducts)
            {
                if (!products.Select(p => p.Id).Contains(product.Id))
                {
                    products.Add(product);
                }
            }
            if (productCountPrior == products.Count || loop > maxPages)
                break;
        } while (true);
        return products;
    }

    public static async Task<IDocument> GetHtml(ChromeDriver driver, string url, int waitSeconds = 0)
        {
            driver.Navigate().GoToUrl(url);
            await Task.Delay(waitSeconds * 1000);
            return await GetDocumentFromSource(driver.PageSource);
        }

        public static async Task<IDocument> GetDocumentFromSource(string pageSource)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(pageSource));
            return document;
        }

        public static async Task<IEnumerable<Product>> GetProductsFromInfiniteScroll(ChromeDriver driver, string productSelector, Func<AngleSharp.Dom.IElement, Product> converter, string url, int delaySeconds = 0)
        {
            driver.Navigate().GoToUrl(url);
            int loops = 0;
            while (true)
            {
                await Task.Delay(1000);
                var lastProduct = driver
                    .FindElementsByCssSelector(productSelector)
                    .Last();
                var scrollYBefore = (Int64)driver.ExecuteScript("return window.scrollY;");
                driver.ExecuteScript("arguments[0].scrollIntoView()", lastProduct);
                var scrollYAfter = (Int64)driver.ExecuteScript("return window.scrollY");
                if (scrollYBefore == scrollYAfter)
                    break;
                if (loops > 50)
                    throw new Exception("Too many loops");
                Console.WriteLine($"Scroll: {scrollYAfter}");
            }

            await Task.Delay(1000*delaySeconds); // because of bfx-price on https://www.rebeccaminkoff.com/collections/handbags

            var html = await GetDocumentFromSource(driver.PageSource);
            var products = new List<Product>();
            foreach (var product in html.QuerySelectorAll(productSelector))
            {
                products.Add(converter(product));
            }
            return products;
        }
}