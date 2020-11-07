using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using MediatR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;

namespace coach_bags_selenium
{
    public class GetTwitterImagesCommandHandler : IRequestHandler<GetTwitterImagesCommand, GetTwitterImagesCommandResult>
    {
        const string LOCAL_DIRECTORY = GetMetadataCommandHandler.LOCAL_DIRECTORY;
        private readonly IMediator _mediator;
        public GetTwitterImagesCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<GetTwitterImagesCommandResult> Handle(GetTwitterImagesCommand request, CancellationToken cancellationToken)
        {
            var totalCount = request.Category == ProductCategory.FwrdDresses ? 3 : 2;
            
            var twitterImageSize = request.Category.GetTwitterImageSize(totalCount);

            var images = new List<byte[]>();

            foreach (var imagePath in request.Sources)
            {
                var resized = await GetImage(imagePath, twitterImageSize, request.Now, request.Category.Edit);
                if (resized != null)
                    images.Add(resized);
                if (images.Count == totalCount)
                    break;
            }

            return new GetTwitterImagesCommandResult
            {
                Images = images,
            };
        }

        private async Task<byte[]> GetImage(string src, Size size, DateTime timestamp, Edit edit)
        {
            var imageFilePath = await PrepareImageForTwitter(src, size, timestamp, edit);
            if (imageFilePath == null)
                return null;
            byte[] image = File.ReadAllBytes(Path.Combine(LOCAL_DIRECTORY, Path.GetFileName(imageFilePath)));
            return image;
        }

        private async Task<string> PrepareImageForTwitter(string file, Size size, DateTime timestamp, Edit edit)
        {
            file = Path.GetFileName(file);
            var outputPath = Path.Combine(LOCAL_DIRECTORY, file).Replace(".jpg", "-twitter.jpg");

            if (!TransformImage(size, outputPath, file, edit))
                return null;
            
            return await _mediator.Send(new S3UploadImageCommand {
                FilePath = outputPath,
                Timestamp = timestamp,
                Edit = edit
            });
        }

        private bool TransformImage(Size size, string outputPath, string file, Edit edit)
        {
            using (var newImage = new Image<Rgba32>(size.Width, size.Height))
            using (var img = Image.Load<Rgba32>(Path.Combine(LOCAL_DIRECTORY, file)))
            {
                img.Mutate(i =>
                {
                    i.Resize(0, size.Height);
                });
                var leftStrip = img.Clone(i =>
                {
                    i.Crop(1, size.Height);
                });
                var rightStrip = img.Clone(i =>
                {
                    var size = i.GetCurrentSize();
                    i.Crop(new Rectangle(size.Width - 1, 0, 1, size.Height));
                });

                if (edit == Edit.LegitBags)
                {
                    var leftPixels = GetNumberOfDistinctPixels(leftStrip);
                    var rightPixels = GetNumberOfDistinctPixels(rightStrip);

                    if (leftPixels > 5)
                    {
                        return false;
                    }
                    if (rightPixels > 5)
                    {
                        return false;
                    }
                }

                newImage.Mutate(i =>
                {
                    var width = img.Width;
                    var gutterWidth = size.Width / 2 - width / 2;
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
            return true;
        }

        private static int GetNumberOfDistinctPixels(Image<Rgba32> image)
        {
            if (image.TryGetSinglePixelSpan(out var pixelSpan))
            {
                return pixelSpan
                    .ToArray()
                    .Distinct()
                    .ToArray()
                    .Count();
            }
            return 999;
        }
    }
}