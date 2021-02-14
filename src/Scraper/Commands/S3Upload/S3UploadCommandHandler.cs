using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace coach_bags_selenium
{
    public class S3UploadCommandHandler : IRequestHandler<S3UploadCommand, string>
    {
        private readonly S3Options _options;
        private readonly IAmazonS3 _s3;
        private readonly ILogger<S3UploadCommandHandler> _logger;

        public S3UploadCommandHandler(
            IOptions<S3Options> options,
            IAmazonS3 s3,
            ILogger<S3UploadCommandHandler> logger)
        {
            _options = options.Value;
            _s3 = s3;
            _logger = logger;
        }
        
        public async Task<string> Handle(S3UploadCommand request, CancellationToken cancellationToken)
        {
            var s3Path = Path.Combine(_options.Directory, request.TargetDirectory, request.TargetFileName);
            await new TransferUtility(_s3).UploadAsync(request.SourceFilePath, _options.Bucket, s3Path);
            _logger.LogDebug("Uploaded {s3Path} to S3", s3Path);
            return s3Path;
        }
    }
}