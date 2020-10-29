using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using coach_bags_selenium.Ferragamo;
using Flurl.Http;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class PradaCategory : ProductCategory
        {
            public PradaCategory(int id, ProductType productType, Edit edit) : base(id, $"Prada{productType.ToString()}", productType, edit) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1200, 1200);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch { 
                ProductType.Bags => $"https://www.prada.com/au/en/women/bags/_jcr_content/par/component-plp-section/component-plp-grid.component-grid-template.{pageNumber}.sortBy_5.html",
                _ => throw new NotImplementedException(),
            };

            private async Task<IEnumerable<Product>> GetPage(string url)
            {
                var source = await url.GetStringAsync();
                var document = await HtmlHelpers.GetDocumentFromSource(source);

                var products = document.QuerySelectorAll("product-qb-component")
                    .Select(p => new PradaProduct(p))
                    .Select(p => p.AsEntity(this))
                    .ToList();

                return products;
            }

            public override async Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                return await HtmlHelpers.LoopPages(10, GetProductsUrl, GetPage);
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
            {
                var html = await HtmlHelpers.GetHtml(driver, product.Link);
                
                var images = html.QuerySelectorAll(".pDetails__slide")
                    .Select(s => {
                        var fragment = s.InnerHtml;
                        var source = s.QuerySelectorAll("source").Last();
                        var srcset = source.GetAttribute("srcset") ?? source.GetAttribute("data-srcset");
                        var parsed = SourceSet.Parse(srcset);
                        return parsed.Last().Url;
                    })
                    .Where(p => p != null)
                    .ToArray();

                var tags = html.QuerySelectorAll("#pdp_details .tab__item li")
                    .Select(p => Regex.Replace(p.TextContent, @"\s+", " ").Trim())
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