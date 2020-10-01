using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using coach_bags_selenium.Farfetch;
using Flurl.Http;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class FarfetchCategory : ProductCategory
        {
            public FarfetchCategory(int id, ProductType productType) : base(id, $"Farfetch{productType.ToString()}", productType) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1334, 1334);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch { 
                ProductType.Dresses => $"https://www.farfetch.com/au/plpslice/listing-api/products-facets?page={pageNumber}&view=180&sort=2&category=135979&pagetype=Shopping&gender=Women&pricetype=Sale",
                ProductType.Shoes => $"https://www.farfetch.com/au/plpslice/listing-api/products-facets?page={pageNumber}&view=180&sort=2&category=136307|136308&attributes:17=58|59|75&pagetype=Shopping&gender=Women&pricetype=Sale",
                _ => throw new NotImplementedException(),
            };

            public override async Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                var products = new List<Product>();
                var pageNumber = 0;
                while (products.Count < maxCount)
                {
                    var json = await GetProductsUrl(++pageNumber)
                        .GetJsonAsync<FarfetchProducts>();
                        
                    var p = json.ListingItems.Items
                        .Where(p => p.PriceInfo.FinalPrice < 1000)
                        .Select(p => p.ToEntity(this));

                    products.AddRange(p);
                }

                return products;
            }
            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                var html = await ScrapeCommandHandler.GetHtml(driver, product.Link);
                var images = html.QuerySelectorAll("div[data-tstid=slideshow] img")
                    .Select(p => p.GetAttribute("src"))
                    .Select(p => Regex.Replace(p, @"_\d+\.jpg", "_1000.jpg"));

                var tags = html.QuerySelectorAll("[data-tstid=productDetails] li")
                    .Select(p => p.TextContent.Trim())
                    .ToArray();

                return new ProductMetadata
                {
                    Images = images.Take(images.Count() - 1).ToArray(),
                    Tags = tags,
                };
            }
        }
    }

}