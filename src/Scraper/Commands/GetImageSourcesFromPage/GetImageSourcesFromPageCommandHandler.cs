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
    public class GetImageSourcesFromPageCommandHandler : IRequestHandler<GetImageSourcesFromPageCommand, IEnumerable<string>>
    {
        private readonly ChromeDriver _driver;

        public GetImageSourcesFromPageCommandHandler(
            ChromeDriver driver
        ) {
            _driver = driver;
        }

        public async Task<IEnumerable<string>> Handle(GetImageSourcesFromPageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var images = request.Category switch {
                    Category.OutnetCoats => GetOutnetProducts(request.Url),
                    var x when x.In(Category.FarfetchDresses, Category.FarfetchShoes) => await GetFarfetchProducts(request.Url),
                    _ => throw new NotImplementedException(),
                };

                return images;
            }
            finally
            {
                _driver?.Quit();
            }
        }

        private async Task<IEnumerable<string>> GetFarfetchProducts(string url)
        {
            var html = await ScrapeCommandHandler.GetHtml(_driver, url);
            var result = html.QuerySelectorAll("div[data-tstid=slideshow] img")
                .Select(p => p.GetAttribute("src"))
                .Select(p => Regex.Replace(p, @"_\d+\.jpg", "_1000.jpg"));
            return result.Take(result.Count() - 1);
        }

        private IEnumerable<string> GetOutnetProducts(string url)
        {
            _driver.Navigate().GoToUrl(url);
            var json = _driver.ExecuteScript("return JSON.stringify(window.state.pdp.detailsState.response.body.products)").ToString();
            var images = coach_bags_selenium.Outnet.OutnetProduct.FromJson(json)
                .SelectMany(p => p.ToEntity.Images);
            
            return images;
        }
    }
}