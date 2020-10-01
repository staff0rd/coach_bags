using System.Collections.Generic;
using coach_bags_selenium.Data;
using MediatR;

namespace coach_bags_selenium
{
    public class GetMetadataFromPageCommand : IRequest<ProductMetadata>
    {
        public Product Product { get; set; }
    }
}