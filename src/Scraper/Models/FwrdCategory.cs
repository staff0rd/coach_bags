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
        private class FwrdCategory : ProductCategory
        {
            private readonly bool IsSale;
            public FwrdCategory(int id, ProductType productType, bool isSale, Edit edit = Edit.BagsOnSale) : base(id, $"Fwrd{productType.ToString()}" + (isSale ? "" : "All"), productType, edit) 
            {
                IsSale = isSale;
            }

            public override Size GetTwitterImageSize(int count)
            {
                if (count == 2) return new Size (1440, 1440);
                return base.GetTwitterImageSize(count);
            }
            
            protected override string GetProductsUrl(int pageNumber)
            {
                if (IsSale)
                {                 
                    return ProductType switch {
                        ProductType.Shoes => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3FpageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-shoes%2Fba4e3d%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DShoes&currentPageSortBy=newestMarkdown&useLargerImages=true&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                        ProductType.Dresses => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3Fnavsrc%3Dleft%26pageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-dresses%2F28a4b1%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DDresses&currentPageSortBy=newestMarkdown&useLargerImages=false&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                        ProductType.Bags => "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3Fnavsrc%3Dleft%26pageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-bags%2F01ef40%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DBags&currentPageSortBy=newestMarkdown&useLargerImages=true&outfitViewSession=false&showBagSize=false&lookfwrd=false",
                        _ => throw new NotImplementedException(),
                    };
                } else 
                {
                    return ProductType switch {
                        ProductType.Dresses => $"https://www.fwrd.com/fw/productsinc.jsp?&site=f&aliasURL=category-dresses%2Fa8e981&s=c&c=Dresses&navsrc=clothing&pageNum={pageNumber}&sortBy=newest",
                        _ => throw new NotImplementedException()
                    };
                }
            }

            public async override Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
            {
                if (IsSale)
                {
                    string url = GetProductsUrl(-1);
                    return await GetPage(url);
                } else
                {
                    return await HtmlHelpers.LoopPages(5, GetProductsUrl, GetPage);
                }
            }

            private async Task<IEnumerable<Product>> GetPage(string url)
            {
                var source = await url.GetStringAsync();
                var document = await HtmlHelpers.GetDocumentFromSource(source);

                var products = document.QuerySelectorAll(".products-grid__item")
                    .Select(p => new ForwardProduct(p))
                    .Where(p => !IsSale || p.SalePrice.HasValue)
                    .Select(p => p.AsEntity(this))
                    .Where(p => !IsSale || p.SalePrice < 1000)
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
                if (IsSale) {
                    return ProductType switch {
                        ProductType.Bags => ".product__image-main-view",
                        _ => ".product__image-alt-view",
                    };
                } 
                else
                {
                    return ".product__image-main-view";
                }
            }
        }
    }

}