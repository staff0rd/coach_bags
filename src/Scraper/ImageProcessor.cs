using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using coach_bags_selenium.Data;
using Flurl.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace coach_bags_selenium
{
    public class ImageProcessor
    {
        private readonly Size _size;

        public ImageProcessor(Category category)
        {
            _size = category switch {
                Category.CoachBags => new Size (1200, 628),
                Category.FwrdBags => new Size (2400, 2400),
                _ => new Size (2400, 1256)
            };
        }

        public IEnumerable<byte[]> GetImages(Category category, Product product)
        {
            var src = product.Image;
            if (category == Category.FwrdDresses || category == Category.FwrdBags)
            {
                yield return GetImage(Regex.Replace(src, @"_V\d\.jpg", @"_V1.jpg"));
                yield return GetImage(Regex.Replace(src, @"_V\d\.jpg", @"_V2.jpg"));
                if (category == Category.FwrdDresses)
                    yield return GetImage(Regex.Replace(src, @"_V\d\.jpg", @"_V3.jpg"));
            } else
                yield return GetImage(src);
        }

        private byte[] GetImage(string src)
        {
            var fileName = "image.jpg";
            var directory = "download";
            src.DownloadFileAsync(directory, fileName).Wait();
            var imageFilePath = PrepareImage(directory, fileName);
            byte[] image = File.ReadAllBytes(imageFilePath);
            return image;
        }

        private string PrepareImage(string directory, string file)
        {
            var outputPath = Path.Combine(directory, "output.jpg");
            using (var newImage = new Image<Rgba32>(_size.Width, _size.Height))
            using (Image img = Image.Load(Path.Combine(directory, file)))
            {
                img.Mutate(i =>
                {
                    i.Resize(0, _size.Height);
                });
                var leftStrip = img.Clone(i => {
                    i.Crop(1, _size.Height);
                });
                var rightStrip = img.Clone(i => {
                    var size = i.GetCurrentSize();
                    i.Crop(new Rectangle(size.Width-1, 0, 1, _size.Height));
                });
                newImage.Mutate(i => {
                    var width = img.Width;
                    var gutterWidth = _size.Width/2-width/2;
                    i.DrawImage(img, new Point(gutterWidth, 0), 1);

                    // fill gutters
                    for (int ix = 0; ix < gutterWidth; ix++)
                    {
                        i.DrawImage(leftStrip, new Point(ix, 0), 1);
                        if (ix + width + gutterWidth < _size.Width)
                            i.DrawImage(rightStrip, new Point(ix + width + gutterWidth, 0), 1);
                    }
                });

                newImage.Save(outputPath);
            }
            
            return outputPath;
        }
    }
}