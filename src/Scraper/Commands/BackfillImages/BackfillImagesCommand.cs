using System.Threading.Tasks;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("backfill")]
    public class BackfillImagesCommand : IRequest
    {
        public async Task OnExecute(IMediator mediator)
        {
            await mediator.Send(this);
        }
    }
}