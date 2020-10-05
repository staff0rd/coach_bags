using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html;
using coach_bags_selenium.Farfetch;
using Flurl.Http;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class ToryBurchCategory : ProductCategory
        {
            public ToryBurchCategory(int id, ProductType productType) : base(id, $"ToryBurch{productType.ToString()}", productType) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1770, 1770);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch { 
                ProductType.Bags => $"https://www.toryburch.com/handbags/view-all/?sort=newest",
                _ => throw new NotImplementedException(),
            };

            public override async Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                driver.Navigate().GoToUrl(GetProductsUrl(0));
                int loops = 0;
                while (true)
                {
                    await Task.Delay(1000);
                    var lastProduct = driver
                        .FindElementsByCssSelector("[data-id='ProductTile']")
                        .Last();
                    var scrollYBefore = (Int64)driver.ExecuteScript("return window.scrollY;");
                    driver.ExecuteScript("arguments[0].scrollIntoView()", lastProduct);
                    var scrollYAfter = (Int64)driver.ExecuteScript("return window.scrollY");
                    if (scrollYBefore == scrollYAfter)
                        break;
                    if (loops > 100)
                        throw new Exception("Too many loops");
                    Console.WriteLine($"Scroll: {scrollYAfter}");
                }
                
                var html = await ScrapeCommandHandler.GetHtml(driver.PageSource);
                var products = new List<Product>();
                foreach (var product in html.QuerySelectorAll("[data-id='ProductTile']"))
                {
                    var tb = new ToryBurchProduct(product);
                    var entity = tb.AsEntity(this);
                    products.Add(entity);
                }
                return products;
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                var html = await ScrapeCommandHandler.GetHtml(driver, product.Link, 2);
                var images = html.QuerySelectorAll("[data-id$=detailsGalleryThumbnails] img")
                    .Select(i => i.GetAttribute("src")
                        .Replace("60x68", "1556x1770")
                    ).ToArray();
                
                var tags = html.QuerySelectorAll("li[class^=desc]")
                    .Select(p => p.TextContent.Trim())
                    .ToArray();

                return new ProductMetadata
                {
                    Images = images,
                    Tags = tags,
                };
            }
        }
    }

}