using System.Linq;
using AngleSharp.Dom;
using coach_bags_selenium.Data;

namespace coach_bags_selenium
{
    public class ForwardProduct
    {
        private readonly IElement _element;
        public string Link => "https://www.fwrd.com" + _element.QuerySelectorAll("a.product-grids__link")[0].GetAttribute("href");
        public string Name => string.Join(" - ", 
            _element.QuerySelectorAll(".product-grids__copy-item")
            .Take(2)
            .Select(p => p.Text()));
        public decimal SalePrice => decimal.Parse(_element.QuerySelectorAll(".price__sale")[0].Text().Replace("AU$ ", ""));
        public decimal Price => decimal.Parse(_element.QuerySelectorAll(".price__retail")[0].Text().Replace("AU$ ", "").Replace("original price", "").Trim());
        public decimal Savings => Price - SalePrice;
        public string Id => _element.QuerySelectorAll(".product__image-container")[0].GetAttribute("data-code");
        public string Image => _element.QuerySelectorAll(".product__image-alt-view")[0].GetAttribute("data-lazy-src").Replace("fw/p", "fw/z");

        public ForwardProduct(IElement element)
        {
            _element = element;
        }

        public coach_bags_selenium.Data.Product AsEntity(Category category) => new Data.Product
        {
            Link = Link,
            Name = Name,
            SalePrice = SalePrice,
            Price = Price,
            Savings = Savings,
            Id = Id,
            Image = Image,
            Category = category
        };
    }
}
