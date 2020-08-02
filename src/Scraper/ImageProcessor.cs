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
        public IEnumerable<byte[]> GetImages(Category category, string sourceUrl)
        {
            var size = category switch {
                Category.CoachBags => new Size (1200, 628),
                Category.FwrdBags => new Size (2400, 2400),
                _ => new Size (2400, 1256)
            };

            if (category == Category.FwrdDresses || category == Category.FwrdBags)
            {
                yield return GetImage(Regex.Replace(sourceUrl, @"_V\d\.jpg", @"_V1.jpg"), size);
                yield return GetImage(Regex.Replace(sourceUrl, @"_V\d\.jpg", @"_V2.jpg"), size);
                if (category == Category.FwrdDresses)
                    yield return GetImage(Regex.Replace(sourceUrl, @"_V\d\.jpg", @"_V3.jpg"), size);
            } else
                yield return GetImage(sourceUrl, size);
        }

        private byte[] GetImage(string src, Size size)
        {
            var fileName = "image.jpg";
            var directory = "download";
            src.DownloadFileAsync(directory, fileName).Wait();
            var imageFilePath = PrepareImage(directory, fileName, size);
            byte[] image = File.ReadAllBytes(imageFilePath);
            return image;
        }

        private string PrepareImage(string directory, string file, Size size)
        {
            var outputPath = Path.Combine(directory, "output.jpg");
            using (var newImage = new Image<Rgba32>(size.Width, size.Height))
            using (Image img = Image.Load(Path.Combine(directory, file)))
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