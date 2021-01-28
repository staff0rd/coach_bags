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
        private readonly Browser _html;

        public GetMetadataFromPageCommandHandler(Browser browser)
        {
            _html = browser;
        }

        public async Task<ProductMetadata> Handle(GetMetadataFromPageCommand request, CancellationToken cancellationToken)
        {
            
            var metadata = await request.Product.Category.GetProductMetadataFromUrl(_html, request.Product);
            if (!metadata.Images.Any())
                throw new InvalidOperationException("No images found");
            return metadata;
        }
    }
}