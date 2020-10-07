using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;
using Flurl.Http;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class IconicCategory : ProductCategory
        {
            public IconicCategory(int id, ProductType productType, Edit edit = Edit.IconicDresses) : base(id, $"Iconic{productType.ToString()}", productType, edit)  { }

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1440, 1440);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch
            {
                ProductType.Dresses => $"https://www.theiconic.com.au/womens-clothing-dresses/?page={pageNumber}&sort=new",
                _ => throw new NotImplementedException()
            };
        
            public async override Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount) => 
                await HtmlHelpers.LoopPages(10, GetProductsUrl, GetPage);

            private async Task<IEnumerable<Product>> GetPage(string url)
            {
                var source = await url.GetStringAsync();
                var document = await HtmlHelpers.GetDocumentFromSource(source);

                var products = document.QuerySelectorAll(".product")
                    .Select(p => new IconicProduct(p))
                    .Select(p => p.AsEntity(this))
                    .ToList();

                return products;
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                var html = await HtmlHelpers.GetHtml(driver, product.Link);

                var images = html.QuerySelectorAll(".thumbnails img")
                    .Select(p => p.GetAttribute("data-src"))
                    .Select(p => p.GetGroupMatches(@"\/(http.+)").FirstOrDefault())
                    .Select(p => System.Web.HttpUtility.UrlDecode(p))
                    .ToArray();

                var tags = html.QuerySelector(".product-description")
                    .TextContent
                    .Split('\n')
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim(new char[] { '-', ' '}).Trim())
                    .ToArray();

                return new ProductMetadata
                {
                    Tags = tags,
                    Images = images,
                };
            }
        }
    }
}