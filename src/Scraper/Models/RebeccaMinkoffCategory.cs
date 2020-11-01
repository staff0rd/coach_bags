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
        private class RebeccaMinkoffCategory : ProductCategory
        {
            public RebeccaMinkoffCategory(int id, ProductType productType) : base(id, $"RebeccaMinkoff{productType.ToString()}", productType, Edit.LegitBags) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (800, 800); //TODO
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch { 
                ProductType.Bags => $"https://www.rebeccaminkoff.com/collections/handbags?sort_by=created-descending",
                _ => throw new NotImplementedException(),
            };

            public override async Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                Func<AngleSharp.Dom.IElement, Product> converter = (element) =>
                {
                    var prod = new RebeccaMinkoffProduct(element);
                    var entity = prod.AsEntity(this);
                    return entity;
                };
                return await HtmlHelpers.GetProductsFromInfiniteScroll(driver, "[class*='mix collection-grid--block']", converter, GetProductsUrl(0), 2);
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                var html = await HtmlHelpers.GetHtml(driver, product.Link);
                var images = html.QuerySelectorAll(".product-main picture img")
                    .Select(i => i.GetAttribute("src"))
                    .Where(p => p != null)
                    .GroupBy(p => p)
                    .OrderByDescending(g => g.Count())
                    .Select(g => "https:" + g.First())
                    .OrderBy(p => p)
                    .ToArray();
                
                var tags = html.QuerySelectorAll(".product-utility-content li")
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