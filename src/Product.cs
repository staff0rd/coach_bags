using OpenQA.Selenium;

namespace coach_bags_selenium
{
    public class Product
    {
        private readonly IWebElement _element;
        public string Link => _element.FindElement(By.CssSelector(".card-img a")).GetAttribute("href");
        public string Name => _element.FindElement(By.ClassName("product-tile-name")).Text;
        public decimal SalePrice => decimal.Parse(_element.FindElement(By.CssSelector(".sales .value")).GetAttribute("content"));
        public decimal Price => decimal.Parse(_element.FindElement(By.CssSelector(".strike-through .value")).GetAttribute("content"));
        public decimal Savings => Price - SalePrice;

        public Product(IWebElement element)
        {
            _element = element;
        }
    }
}
