using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using coach_bags_selenium.Data;

namespace coach_bags_selenium
{
    public class IconicProduct
    {
        private readonly IElement _element;
        public string Link => "https://www.theiconic.com.au" + _element.QuerySelector("a").GetAttribute("href");
        public string Brand => _element.QuerySelector("span.brand").Text();
        public string Name => _element.QuerySelector("span.name").Text();
        private IElement SalePriceElement => _element.QuerySelector("span.price.final") ?? _element.QuerySelector("span.price");
        private decimal GetPrice(IElement element) => decimal.Parse(element.Text().Replace("$", "").Trim());
        public decimal SalePrice => GetPrice(SalePriceElement); 
        private IElement PriceElement => _element.QuerySelector("span.price.original") ?? _element.QuerySelector("span.price");
        public decimal Price => GetPrice(PriceElement);
        public decimal Savings => Price - SalePrice;
        public string Id => _element.GetAttribute("data-ti-track-product");
        public string Image => "https:" + _element.QuerySelector("img").GetAttribute("src");
        
        public IconicProduct(IElement element)
        {
            _element = element;
        }

        public coach_bags_selenium.Data.Product AsEntity(ProductCategory category) => new Data.Product
        {
            Link = Link,
            Brand = Brand,
            Name = Name,
            SalePrice = SalePrice,
            Price = Price,
            Savings = Savings,
            Id = Id,
            Image = Image,
            CategoryId = category.Id,
            Edit = category.Edit,
        };
    }
}
