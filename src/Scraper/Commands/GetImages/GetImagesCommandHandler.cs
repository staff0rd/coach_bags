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
        public GetImagesCommandHandler(ILogger<GetImagesCommandHandler> logger)
        {
            _logger = logger;
        }
        
        const string DIRECTORY = "download";
        private readonly ILogger<GetImagesCommandHandler> _logger;

        public async Task<GetImagesCommandResult> Handle(GetImagesCommand request, CancellationToken cancellationToken)
        {
            var size = request.Category switch {
                Category.CoachBags => new Size (1200, 628),
                Category.FwrdBags => new Size (2400, 2400),
                _ => new Size (2400, 1256)
            };

            var sources = GetSources(request.Category, request.SourceUrl);

            sources = await DownloadSources(sources);
            
            var twitterSources = request.Category switch {
                Category.FwrdDresses => sources.Take(3),
                _ => sources.Take(2)
            };

            var twitterImages = twitterSources
                .Select(p => GetImage(p, size))
                .ToList();

            return new GetImagesCommandResult
            {
                S3Uploaded = sources.ToList(),
                ForTwitter = twitterImages,
            };
        }

        private async Task<IEnumerable<string>> DownloadSources(IEnumerable<string> sources)
        {
            var downloaded = new List<string>();
            int index = 1;
            var now = DateTime.UtcNow;
            foreach (var source in sources)
            {
                var name = $"{now:yyyyMMdd-HHmm}-{index}.jpg";
                var fileName = await Download(source, name);
                if (fileName != null)
                    downloaded.Add(fileName);
                else
                    return downloaded;
                index++;
            }
            return downloaded;
        }

        private async Task<string> Download(string source, string downloadedFileName)
        {
            try
            {
                await source.DownloadFileAsync(DIRECTORY, downloadedFileName);
                _logger.LogInformation($"Downloaded {source}");
                return downloadedFileName;
            }
            catch
            {
                return null;
            }
        }

        private IEnumerable<string> GetSources(Category category, string sourceUrl)
        {
            if (category == Category.CoachBags)
            {
                yield return sourceUrl;
            } else
            {
                for (int i = 1; i < 10 ; i++)
                {
                    var zoomed = Regex.Replace(sourceUrl, @"p\/fw\/.+\/", "p/fw/z/");
                    yield return Regex.Replace(zoomed, @"_V\d\.jpg", $"_V{i}.jpg");
                }
            }
        }

        private byte[] GetImage(string src, Size size)
        {
            var imageFilePath = PrepareImageForTwitter(src, size);
            byte[] image = File.ReadAllBytes(imageFilePath);
            return image;
        }

        private string PrepareImageForTwitter(string file, Size size)
        {
            var outputPath = Path.Combine(DIRECTORY, file.Replace(".jpg", "-twitter.jpg"));
            using (var newImage = new Image<Rgba32>(size.Width, size.Height))
            using (Image img = Image.Load(Path.Combine(DIRECTORY, file)))
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
            
            return outputPath;
        }
    }
}