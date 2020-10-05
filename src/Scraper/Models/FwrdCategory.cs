using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class FwrdCategory : ProductCategory
        {
            public FwrdCategory(int id, ProductType productType) : base(id, $"Fwrd{productType.ToString()}", productType) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1440, 1440);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch {
                ProductType.Shoes => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3FpageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-shoes%2Fba4e3d%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DShoes&currentPageSortBy=newestMarkdown&useLargerImages=true&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                ProductType.Dresses => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3Fnavsrc%3Dleft%26pageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-dresses%2F28a4b1%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DDresses&currentPageSortBy=newestMarkdown&useLargerImages=false&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                ProductType.Bags => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3Fnavsrc%3Dleft%26pageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-bags%2F01ef40%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DBags&currentPageSortBy=newestMarkdown&useLargerImages=true&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                _ => throw new NotImplementedException(),
            };

            public async override Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                string url = GetProductsUrl(-1);
                driver.Navigate().GoToUrl(url);

                var text = driver.PageSource;
                var pre = driver.FindElementByCssSelector("pre").Text;
                var config = AngleSharp.Configuration.Default;
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(req => req.Content(pre));
                var ps = document.QuerySelectorAll(".products-grid__item");

                var products = ps
                    .Select(p => new ForwardProduct(p))
                    .Where(p => p.SalePrice.HasValue)
                    .Select(p => p.AsEntity(this))
                    .Where(p => p.SalePrice < 1000)
                    .ToList();

                return products;
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                var html = await HtmlHelpers.GetHtml(driver, product.Link);

                var tags = html.QuerySelectorAll("#pdp-details li")
                    .Select(p => p.TextContent.Trim())
                    .ToArray();

                var zoomed = Regex.Replace(product.Image, @"p\/fw\/.+\/", "p/fw/z/");

                return new ProductMetadata
                {
                    Tags = tags,
                    Images = Enumerable.Range(1, 9)
                        .Select(i => Regex.Replace(zoomed, @"_V\d\.jpg", $"_V{i}.jpg"))
                        .ToArray(),
                };
            }

            public override string GetProductImageClass()
            {
                return ProductType switch {
                    ProductType.Bags => ".product__image-main-view",
                    _ => ".product__image-alt-view",
                };
            }
        }
    }

}