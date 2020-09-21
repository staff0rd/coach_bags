using System.Collections.Generic;

namespace coach_bags_selenium
{
    public class GetMetadataCommandResult
    {
        public List<string> ImagesS3Uploaded { get; set; }
        public List<byte[]> ImagesForTwitter { get; set; }

        public List<string> Tags { get; set; }
    }
}