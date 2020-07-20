using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Chrome;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using AngleSharp;

[assembly: UserSecretsIdAttribute("35c1247a-0256-4d98-b811-eb58b6162fd7")]
namespace coach_bags_selenium
{
    class Program
    {
        static Task Main(string[] args) => CreateHostBuilder().RunCommandLineApplicationAsync<Program>(args);
                
        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {

                });

        private IHostEnvironment _env;
        private Microsoft.Extensions.Configuration.IConfiguration _config;

        public Program(IHostEnvironment env, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public void OnExecute()
        {
            var twitterOptions = _config.GetSection("Twitter").Get<TwitterOptions>();
            var count = _config.GetValue<int>("Count");
            var category = Enum.Parse<Category>(_config.GetValue<string>("Category"));
            var connectionString = _config.GetConnectionString("Postgres");
            var db = new coach_bags_selenium.Data.DatabaseContext(connectionString);
            db.Database.Migrate();

            var driver = ConfigureDriver();

            try
            {
                IEnumerable<Product> products = null;
                switch (category)
                {
                    case Category.CoachBags: products = GetCoachBags(driver, count); break;
                    case Category.FwrdShoes: products = GetFwrdShoes(driver).Result; break;
                }
                
                var now = Save(db, products);
                var product = ChooseProductToTweet(db, products.First().Category, now);

                if (product != null)
                {
                    product.LastPostedUtc = now;

                    var src = products.Single(p => p.Id == product.Id).Image;
                    var fileName = "image.jpg";
                    var directory = "download";
                    src.DownloadFileAsync(directory, fileName).Wait();
                    var imageFilePath = PrepareImage(directory, fileName);
                    byte[] image = File.ReadAllBytes(imageFilePath);

                    var text = $"{product.Name} - {product.SavingsPercent}% off, was ${product.Price}, now ${product.SalePrice} {product.Link}";

                    Auth.SetUserCredentials(twitterOptions.ConsumerKey, twitterOptions.ConsumerSecret, twitterOptions.AccessToken, twitterOptions.AccessTokenSecret);
                    var media = Upload.UploadBinary(image);
                    var tweet = Tweet.PublishTweet(text, new PublishTweetOptionalParameters
                    {
                        Medias = new List<IMedia> { media }
                    });
                    Console.WriteLine($"Tweeted: {text}");
                    db.SaveChanges();
                } else 
                    Console.WriteLine("Nothing new to tweet");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                driver?.Quit();
            }
        }

        private static string PrepareImage(string directory, string file)
        {
            var outputPath = Path.Combine(directory, "output.jpg");
            using (var newImage = new Image<Rgba32>(1200, 628))
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
                    //Console.WriteLine($"{size.Width}x{size.Height}");
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

        private static Data.Product ChooseProductToTweet(DatabaseContext db, Category category, DateTime now)
        {
            var pendingProducts = db.Products
                .Where(p => p.Category == category)
                .Where(p => p.LastUpdatedUtc >= now) // still available on page
                .Where(p => p.LastPostedUtc == null) // not yet tweeted
                .ToArray();

            if (!pendingProducts.Any())
                return null;

            Random rand = new Random();
            int index = rand.Next(pendingProducts.Length);

            var entity = pendingProducts.ElementAt(index);
            return entity;
        }

        private static IEnumerable<Product> GetCoachBags(ChromeDriver driver, int count)
        {
            var url = $"https://coachaustralia.com/on/demandware.store/Sites-au-coach-Site/en_AU/Search-UpdateGrid?cgid=sale-womens_sale-bags&start=0&sz={count}";
            driver.Navigate().GoToUrl(url);

            var products =
                driver.FindElementsByClassName("product")
                .Select(p => new CoachBag(p).AsEntity);
            Console.WriteLine($"Found {products.Count()} products");
            return products;
        }

        private async static Task<IEnumerable<Product>> GetFwrdShoes(ChromeDriver driver)
        {
            var url = "https://www.fwrd.com/fw/content/products/lazyLoadProductsForward?currentPlpUrl=https%3A%2F%2Fwww.fwrd.com%2Ffw%2FCategory.jsp%3FpageNum%3D1%26sortBy%3DnewestMarkdown%26aliasURL%3Dsale-category-shoes%2Fba4e3d%26site%3Df%26%26n%3Ds%26s%3Dc%26c%3DShoes&currentPageSortBy=newestMarkdown&useLargerImages=true&outfitViewSession=false&showBagSize=false&lookfwrd=false";
            driver.Navigate().GoToUrl(url);



            var text = driver.PageSource;
            var pre = driver.FindElementByCssSelector("pre").Text;
            var config = AngleSharp.Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(pre));
            var ps = document.QuerySelectorAll(".products-grid__item");

            var products = ps
                .Select(p => new ForwardProduct(p).AsEntity(Category.FwrdShoes))
                .ToList();

            Console.WriteLine($"Found {products.Count()} products");
            return products;
        }

        private static ChromeDriver ConfigureDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("no-sandbox"); // needed to run inside container

            var driver = new ChromeDriver(options);
            driver.ExecuteChromeCommand("Network.setUserAgentOverride", new System.Collections.Generic.Dictionary<string, object> {
                { "userAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.53 Safari/537.36" }
            });
            driver.ExecuteChromeCommand("Page.addScriptToEvaluateOnNewDocument",
                new System.Collections.Generic.Dictionary<string, object> {
                    { "source", @"
                        Object.defineProperty(navigator, 'webdriver', {
                            get: () => undefined
                        })"
                    }
                 }
            );
            return driver;
        }

        private static DateTime Save(DatabaseContext db, IEnumerable<Data.Product> products)
        {
            var now = DateTime.UtcNow;

            foreach (var product in products)
            {
                product.LastUpdatedUtc = now;
                
                var existing = db.Products.FirstOrDefault(p => p.Id == product.Id && p.Category == product.Category);
                if (existing is null)
                {
                    product.CreatedUtc = now;
                    db.Products.Add(product);
                } else
                {
                    existing.Link = product.Link;
                    existing.Name = product.Name;
                    existing.SalePrice = product.SalePrice;
                    existing.Price = product.Price;
                    existing.Savings = product.Savings;
                    existing.LastUpdatedUtc = now;
                    existing.Image = product.Image;
                }
            }
            Console.WriteLine("Saving...");
            db.SaveChanges();
            return now;
        }
    }
}
