using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
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
                .Where(p => p.LastPostedUtc.HasValue)
                .Where(p => request.Category != null || p.CategoryId == request.Category.Id)
                .Where(p => request.Overwrite || p.Images == null)
                .ToList();

            _logger.LogInformation($"Found {productsWithoutImages.Count} products without images");

            var count = 0;
            foreach (var product in productsWithoutImages)
            {
                var result = await _mediator.Send(new GetMetadataCommand { Now = product.LastPostedUtc.Value, Product = product });
                product.Images = result.ImagesS3Uploaded.ToArray();
                await db.SaveChangesAsync();
                count++;
                _logger.LogInformation($"Completed {count}/{productsWithoutImages.Count} images");
            }

            return Unit.Value;
        }
    }
}