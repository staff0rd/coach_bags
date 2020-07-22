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
        private readonly int _width;
        private readonly int _height;

        public ImageProcessor(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public IEnumerable<byte[]> GetImages(Category category, Product product)
        {
            var src = product.Image;
            if (category == Category.FwrdDresses || category == Category.FwrdBags)
            {
                yield return GetImage(Regex.Replace(src, @"_V\d\.jpg", @"_V1.jpg"));
                yield return GetImage(Regex.Replace(src, @"_V\d\.jpg", @"_V2.jpg"));
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
            using (var newImage = new Image<Rgba32>(_width, _height))
            using (Image img = Image.Load(Path.Combine(directory, file)))
            {
                img.Mutate(i =>
                {
                    i.Resize(0, 628);
                });
                var leftStrip = img.Clone(i => {
                    i.Crop(1, 628);
                });
                var rightStrip = img.Clone(i => {
                    var size = i.GetCurrentSize();
                    i.Crop(new Rectangle(size.Width-1, 0, 1, 628));
                });
                newImage.Mutate(i => {
                    var width = img.Width;
                    var gutterWidth = 1200/2-width/2;
                    i.DrawImage(img, new Point(gutterWidth, 0), 1);

                    // fill gutters
                    for (int ix = 0; ix < gutterWidth; ix++)
                    {
                        i.DrawImage(leftStrip, new Point(ix, 0), 1);
                        i.DrawImage(rightStrip, new Point(ix + width + gutterWidth, 0), 1);
                    }
                });

                newImage.Save(outputPath);
            }
            
            return outputPath;
        }
    }
}