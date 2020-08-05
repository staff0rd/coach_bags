using System;
using MediatR;

namespace coach_bags_selenium
{
    public class S3UploadCommand : IRequest<string>
    {
        public string SourceFilePath { get; internal set; }
        public string TargetDirectory { get; internal set; }
        public string TargetFileName { get; internal set; }
    }
}