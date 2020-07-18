using Microsoft.Extensions.Configuration.UserSecrets;

namespace coach_bags_selenium
{
    public class TwitterOptions
    {
        public string ConsumerSecret { get; set; }
        public string ConsumerKey { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
