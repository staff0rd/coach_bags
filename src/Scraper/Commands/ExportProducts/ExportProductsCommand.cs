using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("export")]
    public class ExportProductsCommand : IRequest
    {
        [Option("-p|--page-size", CommandOptionType.SingleValue)]
        public int PageSize { get; set; } = 25;

        [Option("-a|--all", CommandOptionType.NoValue)]
        public bool All { get; set; }

        public async Task OnExecute(IMediator mediator)
        {
            await mediator.Send(this);
        }
    }
}