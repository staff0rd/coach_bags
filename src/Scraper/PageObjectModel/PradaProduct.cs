using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using coach_bags_selenium.Data;

namespace coach_bags_selenium
{
    public class PradaProduct
    {
        private readonly IElement _element;
        public string Link => "https://www.prada.com" + _element.QuerySelector("a").GetAttribute("href");
        public string Brand => "Prada";
        public string Name => _element.QuerySelector("a.productQB__title").Text().Replace("Prada","").Trim();
        public decimal Price => decimal.Parse(_element.QuerySelector(".productQB__price").Text().Replace("AUD", "").Trim());
        public string Id => _element.GetAttribute("data-part-number");
        
        public PradaProduct(IElement element)
        {
            _element = element;
        }

        public coach_bags_selenium.Data.Product AsEntity(ProductCategory category) => new Data.Product
        {
            Link = Link,
            Brand = Brand,
            Name = Name,
            SalePrice = Price,
            Price = Price,
            Savings = 0,
            Id = Id,
            Image = null,
            CategoryId = category.Id,
            Edit = category.Edit,
        };
    }
}
