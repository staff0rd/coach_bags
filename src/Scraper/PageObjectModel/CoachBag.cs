using AngleSharp.Dom;
using OpenQA.Selenium;

namespace coach_bags_selenium
{
    public class CoachBag
    {
        private readonly IElement _element;
        public string Link => "https://coachaustralia.com" + _element.QuerySelector(".card-img a").GetAttribute("href");
        public string Name => _element.QuerySelector(".product-tile-name").TextContent;
        public decimal SalePrice => decimal.Parse(_element.QuerySelector(".sales .value").GetAttribute("content"));
        private IElement oldPriceElement => _element.QuerySelector(".strike-through .value");
        public decimal Price => oldPriceElement != null ? decimal.Parse(oldPriceElement.GetAttribute("content")) : SalePrice;
        public decimal Savings => Price - SalePrice;
        public string Id => _element.GetAttribute("data-pid");
        public string Image => _element.QuerySelector(".card-img-top").GetAttribute("src");

        public CoachBag(IElement element)
        {
            _element = element;
        }

        public coach_bags_selenium.Data.Product AsEntity => new Data.Product
        {
            Link = Link,
            Name = Name,
            Brand = "Coach",
            SalePrice = SalePrice,
            Price = Price,
            Savings = Savings,
            Id = Id,
            Image = Image,
            Edit = Edit.BagsOnSale,
        };
    }
}
