using System;
using AngleSharp.Dom;
using coach_bags_selenium.Data;

namespace coach_bags_selenium
{
    public class OutnetProduct
    {
        private readonly IElement _element;
        public string Link => _element.QuerySelector("[itemprop='item'] meta[itemprop='url mainEntityOfPage']").GetAttribute("content");
        public string Brand => _element.QuerySelector("span[itemprop='brand']").Text();
        public string Name => _element.QuerySelector("span[itemprop='name']").Text();
        public decimal SalePrice => decimal.Parse(_element.QuerySelector("span[itemprop='price']").GetAttribute("content"));
        public decimal Price => decimal.Parse(_element.QuerySelector("s").Text().Replace("$", ""));
        public bool HasPrice => _element.QuerySelectorAll("s").Length > 0;
        public decimal Savings => Price - SalePrice;
        public string Id => _element.GetAttribute("id").Replace("pid-", "");
        public string Image => "https:" + _element.QuerySelector("noscript img[itemprop=image]").GetAttribute("src");

        public OutnetProduct(IElement element)
        {
            _element = element;
            Console.WriteLine(Image);
        }

        public coach_bags_selenium.Data.Product AsEntity(ProductCategory category) => new Data.Product
        {
            Link = Link,
            Name = Name,
            Brand = Brand,
            CategoryId = category.Id,
            SalePrice = SalePrice,
            Price = Price,
            Savings = Savings,
            Id = Id,
            Image = Image,
        };
    }
}
