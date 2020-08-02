using System;
using coach_bags_selenium.Data;
using MediatR;

namespace coach_bags_selenium
{
    public class TweetRandomProductCommand : IRequest<Result>
    {
        public Category Category { get; set; }
        public DateTime Since { get; set; }
        public TwitterOptions TwitterOptions { get; set; }
    }
}