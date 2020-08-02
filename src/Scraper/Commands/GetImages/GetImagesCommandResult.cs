using System.Collections.Generic;

namespace coach_bags_selenium
{
    public class GetImagesCommandResult
    {
        public List<string> S3Uploaded { get; set; }
        public List<byte[]> ForTwitter { get; set; }
    }
}