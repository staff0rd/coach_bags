using System.Linq;
using coach_bags_selenium.Data;
using OpenQA.Selenium;

namespace coach_bags_selenium
{
    public class ForwardProduct
    {
        private readonly IWebElement _element;
        public string Link => _element.FindElement(By.CssSelector("a.product-grids__link")).GetAttribute("href");
        public string Name => string.Join(" - ", _element.FindElements(By.ClassName("product-grids__copy-item"))
            .ToList().Take(2).Select(p => p.Text));
        public decimal SalePrice => decimal.Parse(_element.FindElement(By.ClassName("price__sale")).Text.Replace("AU$ ", ""));
        public decimal Price => decimal.Parse(_element.FindElement(By.ClassName("price__retail")).Text.Replace("AU$ ", "").Replace("original price", "").Trim());
        public decimal Savings => Price - SalePrice;
        public string Id => _element.FindElement(By.ClassName("product__image-container")).GetAttribute("data-code");
        public string Image => _element.FindElement(By.ClassName("product__image-alt-view")).GetAttribute("src").Replace("/p/", "/z/");

        public ForwardProduct(IWebElement element)
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
