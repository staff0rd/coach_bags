

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using coach_bags_selenium.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using AngleSharp.Dom;
using Flurl.Http;
using coach_bags_selenium.Farfetch;

namespace coach_bags_selenium
{
    public class ScrapeCommandHandler : IRequestHandler<ScrapeCommand>
    {
        private readonly DataFactory _data;
        private readonly ChromeDriver _driver;
        private readonly ILogger<ScrapeCommandHandler> _logger;

        public ScrapeCommandHandler(
            ILogger<ScrapeCommandHandler> logger,
            DataFactory data,
            ChromeDriver driver)
        {
            _data = data;
            _driver = driver;
            _logger = logger;
        }

        public async Task<Unit> Handle(ScrapeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var products = await request.Category.GetProducts(_driver, request.Count);

                var deDupedProducts = products
                    .GroupBy(p => p.Id)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.First())
                    .ToList();

                var dupeCount = products.Count() - deDupedProducts.Count;

                if (dupeCount > 0)
                {
                    _logger.LogInformation($"Removed {dupeCount}, total is now {deDupedProducts.Count}");
                }

                _data.GetDatabaseContext().Save(deDupedProducts);
            }
            finally
            {
                _driver?.Quit();
            }

            return Unit.Value;
        }

        public static async Task<IDocument> GetHtml(ChromeDriver driver, string url, int seconds = 0)
        {
            driver.Navigate().GoToUrl(url);
            await Task.Delay(seconds * 1000);
            return await GetHtml(driver.PageSource);
        }

        public static async Task<IDocument> GetHtml(string pageSource)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(pageSource));
            return document;
        }

    }
}