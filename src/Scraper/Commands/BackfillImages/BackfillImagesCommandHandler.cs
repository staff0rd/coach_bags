using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace coach_bags_selenium
{
    public class BackfillImagesCommandHandler : IRequestHandler<BackfillImagesCommand>
    {
        private readonly DataFactory _data;
        private readonly ILogger<BackfillImagesCommandHandler> _logger;
        private readonly IMediator _mediator;

        public BackfillImagesCommandHandler(
            DataFactory data,
            ILogger<BackfillImagesCommandHandler> logger,
            IMediator mediator)
        {
            _data = data;
            _logger = logger;
            _mediator = mediator;
        }
        
        public async Task<Unit> Handle(BackfillImagesCommand request, CancellationToken cancellationToken)
        {
            var db = _data.GetDatabaseContext();
            var productsWithoutImages = db.Products
                .Where(p => p.Category == request.Category)
                .Where(p => p.LastPostedUtc.HasValue)
                .Where(p => p.Images == null)
                .ToList();

            _logger.LogInformation($"Found {productsWithoutImages.Count} products without images");

            var count = 0;
            foreach (var product in productsWithoutImages)
            {
                var result = await _mediator.Send(new GetImagesCommand { Now = product.LastPostedUtc.Value, SourceUrl = product.Image, Category = product.Category });
                product.Images = result.S3Uploaded.ToArray();
                await db.SaveChangesAsync();
                count++;
                _logger.LogInformation($"Completed {count}/{productsWithoutImages.Count} images");
            }

            return Unit.Value;
        }
    }
}