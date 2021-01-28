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
        private class ToryBurchCategory : ProductCategory
        {
            public ToryBurchCategory(int id, ProductType productType) : base(id, $"ToryBurch{productType.ToString()}", productType, Edit.LegitBags) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1770, 1770);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch { 
                ProductType.Bags => $"https://www.toryburch.com/handbags/view-all/?sort=newest",
                _ => throw new NotImplementedException(),
            };

            public override async Task<IEnumerable<Product>> GetProducts(Browser browser, int maxCount)
            {
                return await browser.GetProductsFromInfiniteScroll("[data-id='ProductTile']", (element) => new ToryBurchProduct(element).AsEntity(this), GetProductsUrl(0));
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(Browser browser, Product product)
            {
                var html = await browser.GetHtml(product.Link, 2);
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