using System.Threading.Tasks;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("images")]
    public class GetImagesCommand : IRequest<GetImagesCommandResult>
    {
        public Category Category { get; set; }
        public string SourceUrl { get; set; }

        public async Task OnExecute(IMediator mediator)
        {
            this.Category = Category.FwrdBags;
            this.SourceUrl = "https://is4.fwrdassets.com/images/p/fw/45s/HTSF-WY6_V1.jpg";
            await mediator.Send(this);
        }
    }
}