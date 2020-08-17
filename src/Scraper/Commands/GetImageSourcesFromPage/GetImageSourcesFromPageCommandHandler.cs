using System.Collections.Generic;
using System.Linq;
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
                _driver.Navigate().GoToUrl(request.Url);
                var json = _driver.ExecuteScript("return JSON.stringify(window.state.pdp.detailsState.response.body.products)").ToString();
                
                var images = coach_bags_selenium.Outnet.OutnetProduct.FromJson(json)
                    .SelectMany(p => p.ToEntity.Images);

                await Task.Delay(0);

                return images;
            }
            finally
            {
                _driver?.Quit();
            }
        }
    }
}