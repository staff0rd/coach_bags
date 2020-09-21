using System.Text.RegularExpressions;

namespace coach_bags_selenium
{
    public class HashtagGenerator
    {
        public string Generate(string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            var transformed = rgx.Replace(input, "")
                .ToLower();

            return $"#{transformed}";
        }
    }
}