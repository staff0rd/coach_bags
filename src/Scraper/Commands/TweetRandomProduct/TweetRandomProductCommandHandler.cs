using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace coach_bags_selenium
{
    public class TweetRandomProductCommandHandler : IRequestHandler<TweetRandomProductCommand, Result>
    {
        private readonly ILogger<TweetRandomProductCommandHandler> _logger;
        private readonly DataFactory _data;
        private readonly ImageProcessor _imageProcessor;

        public TweetRandomProductCommandHandler(
            ILogger<TweetRandomProductCommandHandler> logger,
            ImageProcessor imageProcessor,
            DataFactory data)
        {
            _logger = logger;   
            _data = data;
            _imageProcessor = imageProcessor;
        }
        public async Task<Result> Handle(TweetRandomProductCommand request, CancellationToken cancellationToken)
        {
            var db = _data.GetDatabaseContext();
            var twitterOptions = request.TwitterOptions;
            var product = db.ChooseProductToTweet(request.Category, request.Since);

            if (product != null)
            {
                product.LastPostedUtc = request.Since;
                var images = _imageProcessor.GetImages(request.Category, product.Image).ToList();

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

            await Task.Delay(0); // naughty!

            return Result.Ok();
        }

        private static IEnumerable<IMedia> UploadImagesToTwitter(IEnumerable<byte[]> images)
        {
            foreach (var image in images)
                yield return Upload.UploadBinary(image);
        }
    }
}