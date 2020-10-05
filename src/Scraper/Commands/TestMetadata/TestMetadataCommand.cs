using coach_bags_selenium;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;

[Command("testmeta")]
public class TestMetadataCommand : Request
{
    [Option("-c|--category", CommandOptionType.SingleValue)]
    public string CategoryName
    {
        set { Category = Enumeration.FromDisplayName<ProductCategory>(value); }
        get { return Category.DisplayName; }
    }
    
    public ProductCategory Category { get; set; }
    [Option("-i|--id", CommandOptionType.SingleValue)]
    public string Id { get; set; }
    
}