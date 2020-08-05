using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace coach_bags_selenium
{
    public class ScrapeAndTweetCommandHandler : IRequestHandler<ScrapeAndTweetCommand>
    {
        private readonly IMediator _mediator;

        public ScrapeAndTweetCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task<Unit> Handle(ScrapeAndTweetCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            await _mediator.Send(new ScrapeCommand
            {
                Category = request.Category,
                Count = request.Count,
            });
            
            await _mediator.Send(new TweetRandomProductCommand
            {
                Category = request.Category,
                PrepareOnly = request.PrepareOnly,
                Since = now,
            });

            return Unit.Value;
        }
    }
}