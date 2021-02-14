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
        private readonly HashtagGenerator _hashtags;
        private readonly TwitterOptions _twitterOptions;

        public TweetRandomProductCommandHandler(
            ILogger<TweetRandomProductCommandHandler> logger,
            IMediator mediator,
            IOptions<TwitterOptions> twitterOptions,
            DataFactory data)
        {
            _logger = logger;   
            _hashtags = new HashtagGenerator();
            _data = data;
            _twitterOptions = twitterOptions.Value;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(TweetRandomProductCommand request, CancellationToken cancellationToken)
        {
            var db = _data.GetDatabaseContext();

            var product = GetProduct(request, db);

            if (product != null)
            {
                product.LastPostedUtc = request.Since;
                var metadata = await _mediator.Send(new GetMetadataCommand { Product = product });
                product.Images = metadata.ImagesS3Uploaded.ToArray();
                string text = GetTweetText(product, metadata);

                if (!request.PrepareOnly)
                {
                    Auth.SetUserCredentials(_twitterOptions.ConsumerKey, _twitterOptions.ConsumerSecret, _twitterOptions.AccessToken, _twitterOptions.AccessTokenSecret);
                    var media = UploadImagesToTwitter(metadata.ImagesForTwitter);
                    var tweet = Tweet.PublishTweet(text, new PublishTweetOptionalParameters
                    {
                        Medias = media.ToList()
                    });
                }
                _logger.LogInformation("{action}: {text}", request.PrepareOnly ? "Prepared" : "Tweeted", text);
                _logger.LogInformation("Tags: {tags}", string.Join(", ", product.Tags));
                db.SaveChanges();

                await _mediator.Send(new ExportProductsCommand { Edit = product.Category.Edit });
            }
            else
                _logger.LogWarning("{category} has nothing new to tweet", request.Category.DisplayName);

            return Unit.Value;
        }

        private static Product GetProduct(TweetRandomProductCommand request, DatabaseContext db)
        {
            if (string.IsNullOrEmpty(request.ProductId))
                return db.ChooseProductToTweet(request.Category, request.Since);
            else
                return db.Products.FirstOrDefault(p => p.CategoryId == request.Category.Id && p.Id == request.ProductId);
        }

        private string GetTweetText(Product product, GetMetadataCommandResult metadata)
        {
            product.Tags = metadata.Tags.ToArray();
            var brandHashtag = _hashtags.Generate(product.Brand);
            var categoryHashtag = _hashtags.Generate(Enumeration.FromId<ProductCategory>(product.CategoryId).ProductType.ToString());

            if (product.SavingsPercent > 1)
            {
                return $"{product.Brand} - {product.Name} - {product.SavingsPercent}% off, was ${product.Price}, now ${product.SalePrice} {product.Link} {brandHashtag} {categoryHashtag}";
            }
            else 
            {
                return $"{product.Brand} - {product.Name} - ${product.SalePrice} {product.Link} {brandHashtag} {categoryHashtag}";
            } 
        }

        private static IEnumerable<IMedia> UploadImagesToTwitter(IEnumerable<byte[]> images)
        {
            foreach (var image in images)
                yield return Upload.UploadBinary(image);
        }
    }
}