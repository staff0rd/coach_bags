using System;
using MediatR;

namespace coach_bags_selenium
{
    public class S3UploadImageCommand : IRequest<string>
    {
        public string FilePath { get; set; }
        public DateTime Timestamp { get; set; }
        public Edit Edit { get; set; }
    }
}