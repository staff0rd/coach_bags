using System.Threading.Tasks;
using MediatR;

namespace coach_bags_selenium
{
    public abstract class Request : IRequest
    {
        public async Task OnExecute(IMediator mediator)
        {
            await mediator.Send(this);
        }
    }

    public abstract class Request<T> : IRequest<T>
    {
        public async Task OnExecute(IMediator mediator)
        {
            await mediator.Send(this);
        }
    }
}