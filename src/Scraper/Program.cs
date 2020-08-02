using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Chrome;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.UserSecrets;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Models;
using AngleSharp;
using Microsoft.Extensions.Logging;
using Amazon.S3;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

[assembly: UserSecretsIdAttribute("35c1247a-0256-4d98-b811-eb58b6162fd7")]
namespace coach_bags_selenium
{

    [Subcommand(typeof(GenerateContent), typeof(GenerateImages), typeof(ScrapeCommand))]
    class Program
    {
        static Task Main(string[] args) => CreateHostBuilder().RunCommandLineApplicationAsync<Program>(args);
                
        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddTransient<ChromeDriver>(s => ConfigureDriver())
                        .AddMediatR(typeof(Program).Assembly)
                        .AddTransient<DataFactory>();
                });

        private IHostEnvironment _env;
        private Microsoft.Extensions.Configuration.IConfiguration _config;
        private ILogger<Program> _logger;

        public Program(IHostEnvironment env, Microsoft.Extensions.Configuration.IConfiguration config, ILogger<Program> logger)
        {
            _env = env;
            _config = config;
            _logger = logger;
        }

        public void OnExecute()
        {
            var twitterOptions = _config.GetSection("Twitter").Get<TwitterOptions>();
            var count = _config.GetValue<int>("Count");
            var category = Enum.Parse<Category>(_config.GetValue<string>("Category"));
            var connectionString = _config.GetConnectionString("Postgres");
            var db = new coach_bags_selenium.Data.DatabaseContext(connectionString);
            var imageProcessor = new ImageProcessor(category);
            db.Database.Migrate();

            var driver = ConfigureDriver();

            try
            {
                var now = DateTime.UtcNow;
                var product = db.ChooseProductToTweet(category, now);

                if (product != null)
                {
                    product.LastPostedUtc = now;
                    var images = imageProcessor.GetImages(category, product).ToList();

                    var text = $"{product.Brand} - {product.Name} - {product.SavingsPercent}% off, was ${product.Price}, now ${product.SalePrice} {product.Link}";

                    Auth.SetUserCredentials(twitterOptions.ConsumerKey, twitterOptions.ConsumerSecret, twitterOptions.AccessToken, twitterOptions.AccessTokenSecret);
                    var media = UploadImagesToTwitter(images);
                    var tweet = Tweet.PublishTweet(text, new PublishTweetOptionalParameters
                    {
                        Medias = media.ToList()
                    });
                    _logger.LogInformation($"Tweeted: {text}");
                    db.SaveChanges();
                }
                else
                    _logger.LogWarning("Nothing new to tweet");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Critical exception");
            }
            finally
            {
                driver?.Quit();
            }
        }


        private static IEnumerable<IMedia> UploadImagesToTwitter(IEnumerable<byte[]> images)
        {
            foreach (var image in images)
                yield return Upload.UploadBinary(image);
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
    }
}
