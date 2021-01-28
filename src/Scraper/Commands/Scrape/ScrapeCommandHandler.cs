using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;

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

                _logger.LogInformation($"Found {products.Count()} products");

                _data.GetDatabaseContext().Save(deDupedProducts);
            }
            finally
            {
                _driver?.Quit();
            }

            return Unit.Value;
        }
    }
}