using OpenQA.Selenium;

namespace coach_bags_selenium
{
    public class OutnetProduct
    {
        private readonly IWebElement _element;
        public string Link => _element.FindElement(By.CssSelector("[itemprop='item'] meta[itemprop='url mainEntityOfPage']")).GetAttribute("content");
        public string Brand => _element.FindElement(By.CssSelector("span[itemprop='brand']")).Text;
        public string Name => _element.FindElement(By.CssSelector("span[itemprop='name']")).Text;
        public decimal SalePrice => decimal.Parse(_element.FindElement(By.CssSelector("span[itemprop='price']")).GetAttribute("content"));
        public decimal Price => decimal.Parse(_element.FindElement(By.CssSelector("s")).Text.Replace("$", ""));
        public bool HasPrice => _element.FindElements(By.CssSelector("s")).Count > 0;
        public decimal Savings => Price - SalePrice;
        public string Id => _element.GetAttribute("id").Replace("pid-", "");
        public string Image => _element.FindElement(By.TagName("img")).GetAttribute("src");

        public OutnetProduct(IWebElement element)
        {
            _element = element;
        }

        public coach_bags_selenium.Data.Product AsEntity => new Data.Product
        {
            Link = Link,
            Name = Name,
            Brand = Brand,
            SalePrice = SalePrice,
            Price = Price,
            Savings = Savings,
            Id = Id,
            Image = Image,
        };
    }
}
