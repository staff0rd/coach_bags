

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using coach_bags_selenium.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using AngleSharp.Dom;

namespace coach_bags_selenium
{
    public class ScrapeCommandHandler : IRequestHandler<ScrapeCommand>
    {
        private readonly DataFactory _data;
        private readonly ChromeDriver _driver;
        private readonly ILogger<ScrapeCommandHandler> _logger;

        public ScrapeCommandHandler(
            ILogger<ScrapeCommandHandler> logger,
            DataFactory data,
            ChromeDriver driver)
        {
            _data = data;
            _driver = driver;
            _logger = logger;
        }

        public async Task<Unit> Handle(ScrapeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var products = request.Category switch
                {
                    Category.CoachBags => GetCoachBags(request.Count),
                    Category.OutnetCoats => await GetOutnetCoats(request.Count),
                    _ => await GetFwrdProducts(request.Category),
                };

                _data.GetDatabaseContext().Save(products);
            }
            finally
            {
                _driver?.Quit();
            }

            return Unit.Value;
        }


        private string GetOutnetUrl(int pageNumber) => $"https://www.theoutnet.com/en-au/shop/clothing/coats?pageNumber={pageNumber}";

        private async Task<IEnumerable<Product>> GetOutnetCoats(int count)
        {
            var products = new List<Product>();
            var pageNumber = 0;
            while (products.Count < count)
            {
                _driver.Navigate().GoToUrl(GetOutnetUrl(++pageNumber));
                var json = _driver.ExecuteScript("return JSON.stringify(window.state.plp.listing.visibleProducts[0].products)").ToString();
                
                var p = coach_bags_selenium.Outnet.OutnetProduct.FromJson(json)
                    .Where(p => p.Price.WasPrice != null)
                    .Select(p => p.ToEntity);

                products.AddRange(p);
            }

            return products;
        }

        private List<Product> GetOutnetCoats(IDocument document)
        {
            var ps = document.QuerySelectorAll("[id^=pid]");
            var products = ps
                .Select(p => new OutnetProduct(p))
                .Where(p => p.HasPrice)
                .Select(p => p.AsEntity(Category.OutnetCoats))
                .ToList();

            _logger.LogInformation($"Found {products.Count()} products");
            return products;
        }

        public static async Task<IDocument> GetHtml(ChromeDriver driver, string url)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            driver.Navigate().GoToUrl(url);
            var document = await context.OpenAsync(req => req.Content(driver.PageSource));
            return document;
        }

        private IEnumerable<Product> GetCoachBags(int count)
        {
            var url = $"https://coachaustralia.com/on/demandware.store/Sites-au-coach-Site/en_AU/Search-UpdateGrid?cgid=sale-womens_sale-bags&start=0&sz={count}";
            _driver.Navigate().GoToUrl(url);

            var products =
                _driver.FindElementsByClassName("product")
                .Select(p => new CoachBag(p).AsEntity);

            _logger.LogInformation($"Found {products.Count()} products");

            return products;
        }

        private async Task<IEnumerable<Product>> GetFwrdProducts(Category category)
        {
            string url = GetFwrdUrl(category);
            _driver.Navigate().GoToUrl(url);

            var text = _driver.PageSource;
            var pre = _driver.FindElementByCssSelector("pre").Text;
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(pre));
            var ps = document.QuerySelectorAll(".products-grid__item");

            _logger.LogInformation("Parsing");

            var products = ps
                .Select(p => new ForwardProduct(p))
                .Where(p => p.SalePrice.HasValue)
                .Select(p => p.AsEntity(category))
                .Where(p => p.SalePrice < 1000)
                .ToList();

            _logger.LogInformation($"Found {products.Count()} products");
            return products;
        }

        private static string GetFwrdUrl(Category category)
        {
            return category switch {
                Category.FwrdShoes => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3FpageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-shoes%2Fba4e3d%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DShoes&currentPageSortBy=newestMarkdown&useLargerImages=true&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                Category.FwrdDresses => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3Fnavsrc%3Dleft%26pageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-dresses%2F28a4b1%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DDresses&currentPageSortBy=newestMarkdown&useLargerImages=false&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                Category.FwrdBags => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3Fnavsrc%3Dleft%26pageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-bags%2F01ef40%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DBags&currentPageSortBy=newestMarkdown&useLargerImages=true&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                _ => throw new ArgumentException(nameof(category))
            };
        }
    }
}