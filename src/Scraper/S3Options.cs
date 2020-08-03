using Microsoft.Extensions.Configuration.UserSecrets;

namespace coach_bags_selenium
{
    public class S3Options
    {
        public string Secret { get; set; }
        public string AccessKey { get; set; }
        public string Bucket { get; set; }
        public string ImageDirectory { get; set; }
    }
}
