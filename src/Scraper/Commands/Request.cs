using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace coach_bags_selenium
{
    public abstract class Request : IRequest
    {
        public async Task OnExecute(IMediator mediator, ILogger<Request> logger)
        {
            try
            {
                await mediator.Send(this);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to handle");
                throw;
            }
        }
    }

    public abstract class Request<T> : IRequest<T>
    {
        public async Task OnExecute(IMediator mediator, ILogger<Request<T>> logger)
        {
            try
            {
                await mediator.Send(this);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to handle");
                throw;
            }
        }
    }
}