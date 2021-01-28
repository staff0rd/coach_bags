using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PlaywrightSharp;

namespace coach_bags_selenium
{
    public class InstallPlaywrightCommandHandler : IRequestHandler<InstallPlaywrightCommand, Unit>
    {
        public InstallPlaywrightCommandHandler()
        {

        }

        public async Task<Unit> Handle(InstallPlaywrightCommand request, CancellationToken cancellationToken)
        {
            await Playwright.InstallAsync();
            return Unit.Value;
        }
    }
}