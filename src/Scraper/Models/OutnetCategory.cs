using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class OutnetCategory : ProductCategory
        {
            public OutnetCategory(int id, ProductType productType) : base(id, $"Outnet{productType.ToString()}", productType) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1440, 1440);
                return base.GetTwitterImageSize(count);
            }
            
            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                driver.Navigate().GoToUrl(product.Link);
                var json = driver.ExecuteScript("return JSON.stringify(window.state.pdp.detailsState.response.body.products)").ToString();
                var images = coach_bags_selenium.Outnet.OutnetProduct.FromJson(json)
                    .SelectMany(p => p.ToEntity(this).Images)
                    .ToArray();

                var html = await ScrapeCommandHandler.GetHtml(driver);

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

            public async override Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                var products = new List<Product>();
                var pageNumber = 0;
                while (products.Count < maxCount)
                {
                    var url = GetProductsUrl(++pageNumber);
                    driver.Navigate().GoToUrl(url);
                    var json = driver.ExecuteScript("return JSON.stringify(window.state?.plp.listing.visibleProducts[0].products || [])").ToString();
                    
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