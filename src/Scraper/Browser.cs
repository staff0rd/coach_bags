using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using coach_bags_selenium.Data;
using PlaywrightSharp;
using Microsoft.Extensions.Configuration;

public class Browser
{
    public bool Headless { get; private set; }
    public Browser(Microsoft.Extensions.Configuration.IConfiguration config)
    {
        Headless = config.GetValue<bool>("Headless", true);
    }
    public async Task<IEnumerable<Product>> LoopPages(int maxPages, Func<int, string> getUrl, Func<Browser, string, Task<IEnumerable<Product>>> getPage)
    {
        var loop = 0;
        var pageNumber = 0;
        var products = new List<Product>();
        do
        {
            loop++;
            var productCountPrior = products.Count;
            var url = getUrl(++pageNumber);
            var pageProducts = await getPage(this, url);
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

    public async Task<T> ExecuteJavascript<T>(string url, string javascript)
    {
        using var playwright = await Playwright.CreateAsync(debug: "pw:api");
        await using var browser = await playwright.Webkit.LaunchAsync(headless: Headless);
        var page = await browser.NewPageAsync();
        await page.GoToAsync(url);
        return await page.EvaluateAsync<T>(javascript);
    }

    public async Task<string> GetSource(string url, int waitSeconds = 0)
    {
        using var playwright = await Playwright.CreateAsync(debug: "pw:api");
        await using var browser = await playwright.Webkit.LaunchAsync(headless: Headless);
        var page = await browser.NewPageAsync();
        await page.GoToAsync(url);
        await Task.Delay(waitSeconds * 1000);
        var source = await page.GetContentAsync();
        await browser.CloseAsync();
        return source;
    }

    public async Task<IDocument> GetHtml(string url, int waitSeconds = 0)
    {
        var source = await GetSource(url, waitSeconds);
        var document = await GetDocumentFromSource(source);
        return document;
    }

    public async Task<IDocument> GetDocumentFromSource(string pageSource)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(pageSource));
        return document;
    }

    public async Task<IEnumerable<Product>> GetProductsFromInfiniteScroll(string productSelector, Func<AngleSharp.Dom.IElement, Product> converter, string url, int delaySeconds = 0)
    {
        using var playwright = await Playwright.CreateAsync(debug: "pw:api");
        await using var browser = await playwright.Webkit.LaunchAsync(headless: Headless);
        var page = await browser.NewPageAsync();
        await page.GoToAsync(url);
        int loops = 0;
        while (true)
        {
            await Task.Delay(1000);
            var lastProduct = (await page
                .QuerySelectorAllAsync(productSelector))
                .Last();
            var scrollYBefore = await page.EvaluateAsync<Int64>("return window.scrollY;");
            await page.EvaluateAsync("arguments[0].scrollIntoView()", lastProduct);
            var scrollYAfter = await page.EvaluateAsync<Int64>("return window.scrollY");
            if (scrollYBefore == scrollYAfter)
                break;
            if (loops > 50)
                throw new Exception("Too many loops");
            Console.WriteLine($"Scroll: {scrollYAfter}");
        }

        await Task.Delay(1000*delaySeconds); // because of bfx-price on https://www.rebeccaminkoff.com/collections/handbags

        var html = await GetDocumentFromSource(await page.GetContentAsync());
        var products = new List<Product>();
        foreach (var product in html.QuerySelectorAll(productSelector))
        {
            products.Add(converter(product));
        }
        return products;
    }
}