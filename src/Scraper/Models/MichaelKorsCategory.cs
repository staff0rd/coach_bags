using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using coach_bags_selenium.MichaelKors;
using Flurl.Http;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class MichaelKorsCategory : ProductCategory
        {
            public MichaelKorsCategory(int id, ProductType productType, Edit edit) : base(id, $"MichaelKors{productType.ToString()}", productType, edit) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1347, 1347);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch { 
                ProductType.Bags => $"https://www.michaelkors.global/en_AU/server/data/guidedSearch?stateIdentifier=_/N-10qbalf&Ns=NewArrival|0&No={(pageNumber-1)*40}",
                _ => throw new NotImplementedException(),
            };

            private async Task<IEnumerable<Product>> GetPage(string url) 
            {
                try {
                    var products = await url
                    .WithHeader("Accept", "application/json")
                        .GetJsonAsync<MichaelKorsProducts>();
                    
                    return products
                        .Result.ProductList
                        .Select(p => p.AsEntity(this))
                        .ToArray();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public override async Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                return await HtmlHelpers.LoopPages(10, GetProductsUrl, GetPage);
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                var html = await HtmlHelpers.GetHtml(driver, product.Link, 2);
                
                var images = html.QuerySelectorAll(".pdp-gallery button img")
                    .Select(i => "https:" + i.GetAttribute("src"))
                    .Select(p => new Uri(p).GetLeftPart(UriPartial.Path).ToString() + "?wid=1000&op_sharpen=1&resMode=sharp2&qlt=100")
                    .ToArray();

                var tags = html.QuerySelector(".detail")
                    .TextContent
                    .Replace("•", "")
                    .Split('\n')
                    .Select(p => p.Trim())
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