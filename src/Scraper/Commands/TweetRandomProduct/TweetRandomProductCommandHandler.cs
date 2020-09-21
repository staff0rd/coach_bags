using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
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
                var metadata = await _mediator.Send(new GetMetadataCommand{ Category = request.Category, Product = product });
                product.Images = metadata.ImagesS3Uploaded.ToArray();
                product.Tags = metadata.Tags.ToArray();
                var brandHashtag = new HashtagGenerator().Generate(product.Brand);
                var categoryHashtag = GetTypeHashtag(product.Category);

                var text = $"{product.Brand} - {product.Name} - {product.SavingsPercent}% off, was ${product.Price}, now ${product.SalePrice} {product.Link} {brandHashtag} {categoryHashtag}";

                if (!request.PrepareOnly)
                {
                    Auth.SetUserCredentials(_twitterOptions.ConsumerKey, _twitterOptions.ConsumerSecret, _twitterOptions.AccessToken, _twitterOptions.AccessTokenSecret);
                    var media = UploadImagesToTwitter(metadata.ImagesForTwitter);
                    var tweet = Tweet.PublishTweet(text, new PublishTweetOptionalParameters
                    {
                        Medias = media.ToList()
                    });
                }
                _logger.LogInformation($"{(request.PrepareOnly ? "Prepared" : "Tweeted")}: {text}");
                _logger.LogInformation($"Tags: {string.Join(", ", product.Tags)}");
                db.SaveChanges();

                await _mediator.Send(new ExportProductsCommand());
            }
            else
                _logger.LogWarning("Nothing new to tweet");

            return Unit.Value;
        }

        private string GetTypeHashtag(Category category)
        {
            switch (category) {
                case Category.CoachBags: return "#bags";
                case Category.FwrdShoes: return "#shoes";
                case Category.FwrdDresses: return "#dresses";
                case Category.FwrdBags: return "#bags";
                case Category.OutnetCoats: return "#coats";
                case Category.FarfetchDresses: return "#dresses";
                case Category.FarfetchShoes: return "#shoes";
                case Category.OutnetShoes: return "#shoes";
            }

            throw new ArgumentException("Unknown category");;    
        }

        private static IEnumerable<IMedia> UploadImagesToTwitter(IEnumerable<byte[]> images)
        {
            foreach (var image in images)
                yield return Upload.UploadBinary(image);
        }
    }
}