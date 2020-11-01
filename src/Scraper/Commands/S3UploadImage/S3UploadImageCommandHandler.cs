using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace coach_bags_selenium
{
    public class S3UploadImageCommandHandler : IRequestHandler<S3UploadImageCommand, string>
    {
        private readonly IMediator _mediator;
        
        public S3UploadImageCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<string> Handle(S3UploadImageCommand request, CancellationToken cancellationToken)
        {
            var fileName = Path.GetFileName(request.FilePath);
            var s3FileName = $"{request.Timestamp:yyyy/MM/dd}/{fileName}";
            return await _mediator.Send(new S3UploadCommand {
                SourceFilePath = request.FilePath,
                TargetDirectory = $"{request.Edit}/images",
                TargetFileName = s3FileName,
            });
        }
    }
}