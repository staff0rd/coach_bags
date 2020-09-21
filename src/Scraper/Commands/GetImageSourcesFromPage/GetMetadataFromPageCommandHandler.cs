using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using MediatR;
using OpenQA.Selenium.Chrome;

namespace coach_bags_selenium
{
    public class GetMetadataFromPageCommandHandler : IRequestHandler<GetMetadataFromPageCommand, ProductMetadata>
    {
        private readonly ChromeDriver _driver;

        public GetMetadataFromPageCommandHandler(
            ChromeDriver driver
        ) {
            _driver = driver;
        }

        public async Task<ProductMetadata> Handle(GetMetadataFromPageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var images = request.Category switch {
                    var x when x.In(Category.OutnetCoats, Category.OutnetShoes) => await GetOutnet(request.Url, request.Category),
                    var x when x.In(Category.FarfetchDresses, Category.FarfetchShoes) => await GetFarfetch(request.Url),
                    var x when x.In(Category.FwrdBags, Category.FwrdDresses, Category.FwrdShoes) => await GetFwrd(request.Url),
                    var x when x.In(Category.CoachBags) => await GetCoach(request.Url),
                    _ => throw new NotImplementedException(),
                };

                return images;
            }
            finally
            {
                _driver?.Quit();
            }
        }

        private async Task<ProductMetadata> GetFarfetch(string url)
        {
            var html = await ScrapeCommandHandler.GetHtml(_driver, url);
            var images = html.QuerySelectorAll("div[data-tstid=slideshow] img")
                .Select(p => p.GetAttribute("src"))
                .Select(p => Regex.Replace(p, @"_\d+\.jpg", "_1000.jpg"));

            var tags = html.QuerySelectorAll("[data-tstid=productDetails] li")
                .Select(p => p.TextContent.Trim())
                .ToArray();

            return new ProductMetadata
            {
                Images = images.Take(images.Count() - 1).ToArray(),
                Tags = tags,
            };
        }
        private async Task<ProductMetadata> GetFwrd(string url)
        {
            var html = await ScrapeCommandHandler.GetHtml(_driver, url);

            var tags = html.QuerySelectorAll("#pdp-details li")
                .Select(p => p.TextContent.Trim())
                .ToArray();

            return new ProductMetadata
            {
                Tags = tags,
            };
        }
        private async Task<ProductMetadata> GetCoach(string url)
        {
            var html = await ScrapeCommandHandler.GetHtml(_driver, url);

            var tags = html.QuerySelectorAll(".d-none.features li")
                .Select(p => p.TextContent.Trim())
                .ToArray();

            return new ProductMetadata
            {
                Tags = tags,
            };
        }

        private async Task<ProductMetadata> GetOutnet(string url, Category category)
        {
            _driver.Navigate().GoToUrl(url);
            var json = _driver.ExecuteScript("return JSON.stringify(window.state.pdp.detailsState.response.body.products)").ToString();
            var images = coach_bags_selenium.Outnet.OutnetProduct.FromJson(json)
                .SelectMany(p => p.ToEntity(category).Images)
                .ToArray();

            var html = await ScrapeCommandHandler.GetHtml(_driver);

            var tags = html.QuerySelectorAll("#TECHNICAL_DESCRIPTION p")
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