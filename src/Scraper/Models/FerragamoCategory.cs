using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using coach_bags_selenium.Ferragamo;
using Flurl.Http;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory
    {
        private class FerragamoCategory : ProductCategory
        {
            public FerragamoCategory(int id, ProductType productType) : base(id, $"Ferragamo{productType.ToString()}", productType, Edit.LegitBags) {}

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1002, 1002);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber) => ProductType switch { 
                ProductType.Bags => $"https://www.ferragamo.com/wcs/resources/store/33751/category_custom/products/byId/3074457345616810885?langId=-24&isClp=false&imgSwap=5&parentCategoryRn=3074457345616810881&catalogId=38554&pageNumber={pageNumber}",
                _ => throw new NotImplementedException(),
            };

            private async Task<Product[]> GetPage(int pageNumber) 
            {
                var url = GetProductsUrl(pageNumber);
                try {
                    var products = await url
                    .WithHeader("Accept", "application/json")
                        .GetJsonAsync<FerragamoProducts>();
                    
                    return products
                        .Products
                        .Select(p => p.AsEntity(this))
                        .ToArray();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public override async Task<IEnumerable<Product>> GetProducts(Browser browser, int maxCount)
            {
                var pageNumber = 0;
                var products = new List<Product> (await GetPage(++pageNumber));
                products.AddRange(await GetPage(++pageNumber));
                return products;
            }

            public async override Task<ProductMetadata> GetProductMetadataFromUrl(Browser browser, Product product)
            {
                var html = await browser.GetHtml(product.Link, 2);
                
                var images = html.QuerySelectorAll("img[class*=product-gallery]")
                    .Select(i => i.GetAttribute("data-lazy") ?? i.GetAttribute("src"))
                    .GroupBy(p => p)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.First())
                    .OrderBy(p => p)
                    .ToArray();
                
                var parser = new HtmlParser();

                var tags = html.QuerySelectorAll("[class^=product-detail__list] li:not([class])")
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