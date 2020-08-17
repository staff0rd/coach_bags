using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using Flurl.Http;
using MediatR;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace coach_bags_selenium
{
    public class GetImagesCommandHandler : IRequestHandler<GetImagesCommand, GetImagesCommandResult>
    {
        const string LOCAL_DIRECTORY = "download";
        private readonly ILogger<GetImagesCommandHandler> _logger;
        private readonly IMediator _mediator;

        public GetImagesCommandHandler(
            ILogger<GetImagesCommandHandler> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        private Size GetTwitterSize(Category category, int count) => category switch {
            Category.CoachBags when count == 2 => new Size (1200, 1200),
            var x when x.In(Category.FwrdBags, Category.FwrdDresses, Category.FwrdShoes, Category.OutnetCoats)
                && count == 2 => new Size (1440, 1440),
            var x when x.In(Category.FarfetchDresses, Category.FarfetchShoes)
                && count == 2 => new Size (1334, 1334),
            _ => new Size (2400, 1256)
        };

        public async Task<GetImagesCommandResult> Handle(GetImagesCommand request, CancellationToken cancellationToken)
        {
            var sources = await GetSources(request.Category, request.Product, request.SourceUrl);

            sources = await DownloadSources(sources, request.Now);
            
            var twitterSources = request.Category switch {
                Category.FwrdDresses => sources.Take(3),
                _ => sources.Take(2)
            };

            var size = GetTwitterSize(request.Category, twitterSources.Count());

            var twitterImages = twitterSources
                .Select(async (p) => await GetImage(p, size, request.Now))
                .Select(p => p.Result)
                .ToList();

            return new GetImagesCommandResult
            {
                S3Uploaded = sources.ToList(),
                ForTwitter = twitterImages,
            };
        }

        private async Task<IEnumerable<string>> DownloadSources(IEnumerable<string> sources, DateTime timestamp)
        {
            var downloaded = new List<string>();
            int index = 1;
            foreach (var source in sources)
            {
                var name = $"{timestamp:HHmm}-{index}.jpg";
                var fileName = await Download(source, name, timestamp);
                if (fileName != null)
                    downloaded.Add(fileName);
                else
                    return downloaded;
                index++;
            }
            return downloaded;
        }

        private async Task<string> Download(string source, string downloadFileName, DateTime timestamp)
        {
            try
            {
                await source
                    .WithHeader("Connection", "keep-alive") // needed for outnet
                    .DownloadFileAsync(LOCAL_DIRECTORY, downloadFileName);

                _logger.LogInformation($"Downloaded {source}");
                return await S3Upload(Path.Combine(LOCAL_DIRECTORY, downloadFileName), timestamp);
            }
            catch
            {
                return null;
            }
        }

        private async Task<IEnumerable<string>> GetSources(Category category, Product product, string sourceUrl)
        {
            if (product != null)
            {
                sourceUrl = product.Image;
            }

            var fwrdZoomed = Regex.Replace(sourceUrl, @"p\/fw\/.+\/", "p/fw/z/");

            return category switch {
                Category.CoachBags => Enumerable.Range(1, 9)
                    .Select(i => Regex.Replace(sourceUrl, @"_(\d)\.jpg\?sw=(\d+)&sh=(\d+)", $"_{i}.jpg?sw=1200&sh=1200")),
                var x when x.In(
                    Category.FarfetchDresses,
                    Category.FarfetchShoes,
                    Category.OutnetCoats) => await _mediator.Send(new GetImageSourcesFromPageCommand { Url = product.Link, Category = category }),
                _ => Enumerable.Range(1, 9)
                    .Select(i => Regex.Replace(fwrdZoomed, @"_V\d\.jpg", $"_V{i}.jpg"))
            };
        }

        private async Task<byte[]> GetImage(string src, Size size, DateTime timestamp)
        {
            var imageFilePath = await PrepareImageForTwitter(src, size, timestamp);
            byte[] image = File.ReadAllBytes(Path.Combine(LOCAL_DIRECTORY, Path.GetFileName(imageFilePath)));
            return image;
        }

        private async Task<string> PrepareImageForTwitter(string file, Size size, DateTime timestamp)
        {
            file = Path.GetFileName(file);
            var outputPath = Path.Combine(LOCAL_DIRECTORY, file).Replace(".jpg", "-twitter.jpg");
            using (var newImage = new Image<Rgba32>(size.Width, size.Height))
            using (Image img = Image.Load(Path.Combine(LOCAL_DIRECTORY, file)))
            {
                img.Mutate(i =>
                {
                    i.Resize(0, size.Height);
                });
                var leftStrip = img.Clone(i => {
                    i.Crop(1, size.Height);
                });
                var rightStrip = img.Clone(i => {
                    var size = i.GetCurrentSize();
                    i.Crop(new Rectangle(size.Width-1, 0, 1, size.Height));
                });
                newImage.Mutate(i => {
                    var width = img.Width;
                    var gutterWidth = size.Width/2-width/2;
                    i.DrawImage(img, new Point(gutterWidth, 0), 1);

                    // fill gutters
                    for (int ix = 0; ix < gutterWidth; ix++)
                    {
                        i.DrawImage(leftStrip, new Point(ix, 0), 1);
                        if (ix + width + gutterWidth < size.Width)
                            i.DrawImage(rightStrip, new Point(ix + width + gutterWidth, 0), 1);
                    }
                });

                newImage.Save(outputPath);
            }
            
            return await S3Upload(outputPath, timestamp);
        }

        private async Task<string> S3Upload(string filePath, DateTime timestamp)
        {
            var fileName = Path.GetFileName(filePath);
            var s3FileName = $"{timestamp:yyyy/MM/dd}/{fileName}";
            return await _mediator.Send(new S3UploadCommand {
                SourceFilePath = filePath,
                TargetDirectory = "images",
                TargetFileName = s3FileName,
            });
        }
    }
}