using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using MediatR;
using OpenQA.Selenium.Chrome;

namespace coach_bags_selenium
{
    public class GetMetadataFromPageCommandHandler : IRequestHandler<GetMetadataFromPageCommand, ProductMetadata>
    {
        private readonly ChromeDriver _driver;

        public GetMetadataFromPageCommandHandler(ChromeDriver driver)
        {
            _driver = driver;
        }

        public async Task<ProductMetadata> Handle(GetMetadataFromPageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var metadata = await request.Product.Category.GetProductMetadataFromUrl(_driver, request.Product);
                return metadata;
            }
            finally
            {
                _driver?.Quit();
            }
        }

        
    }
}