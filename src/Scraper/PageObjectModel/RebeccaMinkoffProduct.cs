using AngleSharp.Dom;
using coach_bags_selenium.Data;

namespace coach_bags_selenium
{
    public class RebeccaMinkoffProduct
    {
        private readonly IElement _element;
        public string Link => "https://www.rebeccaminkoff.com" + _element.QuerySelector("a").GetAttribute("href");
        public string Brand => "Rebecca Minkoff";
        public string Name => _element.QuerySelector("a").GetAttribute("ga-event-label");
        private string _bfxPrice => _element.QuerySelector("span.bfx-price").Text().Replace("AUD", "");
        private string _bfxPriceStripped => _bfxPrice.Trim(new char[]{'\uFEFF', '\u0020', '$'});
        public decimal SalePrice => decimal.Parse(_bfxPriceStripped);
        public decimal Price => SalePrice;
        public decimal Savings => Price - SalePrice;
        public string Id => _element.QuerySelector("a").GetAttribute("data-sku");
        public string Image => null;

        public RebeccaMinkoffProduct(IElement element)
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
