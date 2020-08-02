using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace coach_bags_selenium
{
    public class TweetRandomProductCommandHandler : IRequestHandler<TweetRandomProductCommand>
    {
        private readonly ILogger<TweetRandomProductCommandHandler> _logger;
        private readonly DataFactory _data;
        private readonly IMediator _mediator;
        private readonly TwitterOptions _twitterOptions;

        public TweetRandomProductCommandHandler(
            ILogger<TweetRandomProductCommandHandler> logger,
            IMediator mediator,
            IOptions<TwitterOptions> twitterOptions,
            DataFactory data)
        {
            _logger = logger;   
            _data = data;
            _twitterOptions = twitterOptions.Value;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(TweetRandomProductCommand request, CancellationToken cancellationToken)
        {
            var db = _data.GetDatabaseContext();
            var product = db.ChooseProductToTweet(request.Category, request.Since);

            if (product != null)
            {
                product.LastPostedUtc = request.Since;
                var images = await _mediator.Send(new GetImagesCommand{ Category = request.Category, SourceUrl = product.Image});
                product.Images = images.S3Uploaded.ToArray();
                var text = $"{product.Brand} - {product.Name} - {product.SavingsPercent}% off, was ${product.Price}, now ${product.SalePrice} {product.Link}";

                Auth.SetUserCredentials(_twitterOptions.ConsumerKey, _twitterOptions.ConsumerSecret, _twitterOptions.AccessToken, _twitterOptions.AccessTokenSecret);
                var media = UploadImagesToTwitter(images.ForTwitter);
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

            return Unit.Value;
        }

        private static IEnumerable<IMedia> UploadImagesToTwitter(IEnumerable<byte[]> images)
        {
            foreach (var image in images)
                yield return Upload.UploadBinary(image);
        }
    }
}