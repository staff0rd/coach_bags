using System.Collections.Generic;
using coach_bags_selenium.Data;
using MediatR;

namespace coach_bags_selenium
{
    public class GetImageSourcesFromPageCommand : IRequest<IEnumerable<string>>
    {
        public Category Category { get; set; }
        public string Url { get; set; }
    }
}