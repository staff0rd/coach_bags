using System;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html;
using coach_bags_selenium.Data;

namespace coach_bags_selenium
{
    public class ToryBurchProduct
    {
        private readonly IElement _element;
        public string Link => "https://www.toryburch.com" + _element.QuerySelector("a").GetAttribute("href");
        public string Brand => "Tory Burch";
        public string Name => _element.QuerySelector("a").GetAttribute("title");

        private IElement SaleOrStandardPrice => _element.QuerySelector("[data-id='SalePrice']") ?? _element.QuerySelector("[data-id='StandardPrice']");
        public decimal SalePrice => decimal.Parse(SaleOrStandardPrice.Text().Replace("$", ""));
        private IElement OldOrStandardPrice => _element.QuerySelector("[data-id='OldPrice']") ?? _element.QuerySelector("[data-id='StandardPrice']");
        public decimal Price => decimal.Parse(OldOrStandardPrice.Text().Replace("$", ""));
        public decimal Savings => Price - SalePrice;
        public string Id => _element.GetAttribute("data-product-id");
        public string Image => null;

        public ToryBurchProduct(IElement element)
        {
            _element = element;
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
