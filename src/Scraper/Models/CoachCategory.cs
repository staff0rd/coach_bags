using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using PlaywrightSharp;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class CoachCategory : ProductCategory
        {
            public CoachCategory() : base(0, "CoachBags", ProductType.Bags, Edit.BagsOnSale) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1200, 1200);
                return base.GetTwitterImageSize(count);
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(Browser browser, Product product)
            {
                var html = await browser.GetHtml(product.Link);

                var tags = html.QuerySelectorAll(".d-none.features li")
                    .Select(p => p.TextContent.Trim())
                    .ToArray();

                return new ProductMetadata
                {
                    Tags = tags,
                    Images = Enumerable.Range(1, 9)
                    .Select(i => Regex.Replace(product.Image, @"_(\d)\.jpg\?sw=(\d+)&sh=(\d+)", $"_{i}.jpg?sw=1200&sh=1200"))
                    .ToArray(),
                };
            }

            public async override Task<IEnumerable<Product>> GetProducts(Browser browser, int maxCount)
            {
                var url = $"https://coachaustralia.com/on/demandware.store/Sites-au-coach-Site/en_AU/Search-UpdateGrid?cgid=outlet-women-bags&start=0&sz={maxCount}";
                
                var html = await browser.GetHtml(url);
                
                var products = html.QuerySelectorAll(".product")
                    .Select(p => new CoachBag(p).AsEntity)
                    .ToList();

                return products;
            }
        }
    }

}