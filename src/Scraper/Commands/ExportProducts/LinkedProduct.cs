using System;
using System.Text.Json.Serialization;

namespace coach_bags_selenium
{
    public class LinkedProduct : Data.Product
    {
        const string FORMAT = "yyyyMMdd-HHmm";

        [JsonIgnore]
        public DateTime? PrevPostedUtc { get; set; }

        [JsonIgnore]
        public string NextPage => PrevPostedUtc?.ToString(FORMAT);

        [JsonIgnore]
        public string PageName => LastPostedUtc?.ToString(FORMAT);
    }
}
