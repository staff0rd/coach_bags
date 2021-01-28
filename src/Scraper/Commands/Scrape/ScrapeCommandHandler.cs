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
        private readonly ILogger<ScrapeCommandHandler> _logger;
        private readonly Browser _browser;

        public ScrapeCommandHandler(
            ILogger<ScrapeCommandHandler> logger,
            DataFactory data,
            Browser browser
            )
        {
            _data = data;
            _logger = logger;
            _browser = browser;
        }

        public async Task<Unit> Handle(ScrapeCommand request, CancellationToken cancellationToken)
        {
            var products = await request.Category.GetProducts(_browser, request.Count);

            var deDupedProducts = products
                .GroupBy(p => p.Id)
                .OrderByDescending(g => g.Count())
                .Select(g => g.First())
                .ToList();

            var dupeCount = products.Count() - deDupedProducts.Count;

            if (dupeCount > 0)
            {
                _logger.LogInformation($"Deduped {dupeCount} products");
            }

            _logger.LogInformation($"Found {deDupedProducts.Count} products");

            _data.GetDatabaseContext().Save(deDupedProducts);
        

            return Unit.Value;
        }
    }
}