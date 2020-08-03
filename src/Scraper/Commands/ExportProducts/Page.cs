using System.Linq;
using System.Text.Json.Serialization;

namespace coach_bags_selenium
{
    public class Page
    {
        [JsonIgnore]
        public string Name { get; set; }
        public LinkedProduct[] Products { get; set; }
        public string NextPage => Products.Last().NextPage;
    }
}
