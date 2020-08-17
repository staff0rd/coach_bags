using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenQA.Selenium.Chrome;

namespace coach_bags_selenium
{
    public class GetImageSourcesFromPageCommandHandler : IRequestHandler<GetImageSourcesFromPageCommand, IEnumerable<string>>
    {
        private readonly ChromeDriver _driver;

        public GetImageSourcesFromPageCommandHandler(
            ChromeDriver driver
        ) {
            _driver = driver;
        }

        public async Task<IEnumerable<string>> Handle(GetImageSourcesFromPageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var html = await ScrapeCommandHandler.GetHtml(_driver, request.Url);
                return new OutnetProductDetail(html).Images;
            }
            finally
            {
                _driver?.Quit();
            }
        }
    }
}