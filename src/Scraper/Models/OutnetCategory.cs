using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using PlaywrightSharp;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class OutnetCategory : ProductCategory
        {
            public OutnetCategory(int id, ProductType productType) : base(id, $"Outnet{productType.ToString()}", productType, Edit.BagsOnSale) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1440, 1440);
                return base.GetTwitterImageSize(count);
            }
            
            public async override Task<ProductMetadata> GetProductMetadataFromUrl(Browser browser, Product product)
            {
                using var playwright = await Playwright.CreateAsync(debug: "pw:api");
                await using var browserInstance = await playwright.Chromium.LaunchAsync(headless: browser.Headless);
                var page = await browserInstance.NewPageAsync();

                await page.GoToAsync(product.Link);
                var json = await page.EvaluateAsync<string>("JSON.stringify(window.state.pdp.detailsState.response.body.products)");
                var images = coach_bags_selenium.Outnet.OutnetProduct.FromJson(json)
                    .SelectMany(p => p.ToEntity(this).Images)
                    .ToArray();

                var html = await browser.GetDocumentFromSource(await page.GetContentAsync());

                var tags = html.QuerySelectorAll("#TECHNICAL_DESCRIPTION p")
                    .Select(p => p.TextContent.Trim())
                    .ToArray();
                
                return new ProductMetadata
                {
                    Images = images,
                    Tags = tags,
                };
            }

            protected override string GetProductsUrl(int pageNumber) => ProductType switch {
                ProductType.Coats => $"https://www.theoutnet.com/en-au/shop/clothing/coats?pageNumber={pageNumber}",
                ProductType.Shoes => $"https://www.theoutnet.com/en-au/shop/shoes/heels?pageNumber={pageNumber}",
                _ => throw new NotImplementedException(),
            };

            public async override Task<IEnumerable<Product>> GetProducts(Browser browser, int maxCount)
            {
                using var playwright = await Playwright.CreateAsync(debug: "pw:api");
                await using var browserInstance = await playwright.Chromium.LaunchAsync(headless: browser.Headless);
                var page = await browserInstance.NewPageAsync();
                var products = new List<Product>();
                var pageNumber = 0;
                while (products.Count < maxCount)
                {
                    var url = GetProductsUrl(++pageNumber);
                    await page.GoToAsync(url);
                    var json = await page.EvaluateAsync<string>("JSON.stringify(window.state ? window.state.plp.listing.visibleProducts[0].products : [])");
                    
                    var p = coach_bags_selenium.Outnet.OutnetProduct.FromJson(json)
                        .Where(p => p.Price.WasPrice != null)
                        .Select(p => p.ToEntity(this));
                    
                    if (p.Count() == 0)
                        break;

                    products.AddRange(p);
                }

                await Task.Delay(0);

                return products;
            }
        }
    }

}