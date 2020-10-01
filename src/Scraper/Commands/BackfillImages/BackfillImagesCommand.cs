using System.Threading.Tasks;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("backfill")]
    public class BackfillImagesCommand : Request
    {
        [Option("-c|--category", CommandOptionType.SingleValue)]
        public string CategoryName
        {
            set { Category = Enumeration.FromDisplayName<ProductCategory>(value); }
            get { return Category.DisplayName; }
        }
        public ProductCategory Category { get; set; }
        

        [Option("-o|--overwrite", CommandOptionType.NoValue)]
        public bool Overwrite { get; set; }
    }
}