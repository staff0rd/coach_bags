using System.Threading.Tasks;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("backfill")]
    public class BackfillImagesCommand : IRequest
    {
        [Option("-c|--category", CommandOptionType.SingleValue)]
        public Category? Category { get; set; }

        [Option("-o|--overwrite", CommandOptionType.NoValue)]
        public bool Overwrite { get; set; }

        public async Task OnExecute(IMediator mediator)
        {
            await mediator.Send(this);
        }
    }
}