using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using MediatR;
using Microsoft.Extensions.Logging;

namespace coach_bags_selenium
{
    public class GetMetadataCommandHandler : IRequestHandler<GetMetadataCommand, GetMetadataCommandResult>
    {
        public const string LOCAL_DIRECTORY = "download";
        private readonly ILogger<GetMetadataCommandHandler> _logger;
        private readonly IMediator _mediator;

        public GetMetadataCommandHandler(
            ILogger<GetMetadataCommandHandler> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<GetMetadataCommandResult> Handle(GetMetadataCommand request, CancellationToken cancellationToken)
        {
            var metadata = await _mediator.Send(new GetMetadataFromPageCommand { Product = request.Product });

            var sources = await DownloadSources(metadata.Images, request.Now, request.Product.Category.Edit);
            
            if (!sources.Any())
                throw new InvalidOperationException("Couldn't download any images");
            
            var twitterImages = await _mediator.Send(new GetTwitterImagesCommand {
                Category = request.Product.Category,
                Sources = sources.ToArray(),
                Now = request.Now,
            });
            

            return new GetMetadataCommandResult
            {
                ImagesS3Uploaded = sources.ToList(),
                ImagesForTwitter = twitterImages.Images,
                Tags = metadata.Tags.ToList(),
            };
        }

        private async Task<IEnumerable<string>> DownloadSources(IEnumerable<string> sources, DateTime timestamp, Edit edit)
        {
            var downloaded = new List<string>();
            int index = 1;
            foreach (var source in sources)
            {
                var name = $"{timestamp:HHmm}-{index}.jpg";
                var fileName = await Download(source, name, timestamp, edit);
                if (fileName != null)
                    downloaded.Add(fileName);
                else
                    return downloaded;
                index++;
            }
            return downloaded;
        }

        private async Task<string> Download(string source, string downloadFileName, DateTime timestamp, Edit edit)
        {
            try
            {
                await source
                    .WithHeader("Connection", "keep-alive") // needed for outnet
                    .DownloadFileAsync(LOCAL_DIRECTORY, downloadFileName);

                _logger.LogDebug("Downloaded {source}", source);
                return await _mediator.Send(new S3UploadImageCommand { 
                    FilePath = Path.Combine(LOCAL_DIRECTORY, downloadFileName),
                    Timestamp = timestamp,
                    Edit = edit,
                });
            }
            catch
            {
                return null;
            }
        }
    }
}