using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class CoachCategory : ProductCategory
        {
            public CoachCategory() : base(0, "CoachBags", ProductType.Bags) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1200, 1200);
                return base.GetTwitterImageSize(count);
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                var html = await HtmlHelpers.GetHtml(driver, product.Link);

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

            public async override Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                var url = $"https://coachaustralia.com/on/demandware.store/Sites-au-coach-Site/en_AU/Search-UpdateGrid?cgid=sale-womens_sale-bags&start=0&sz={maxCount}";
                driver.Navigate().GoToUrl(url);

                var products =
                    driver.FindElementsByClassName("product")
                    .Select(p => new CoachBag(p).AsEntity);

                await Task.Delay(0);

                return products;
            }
        }
    }

}