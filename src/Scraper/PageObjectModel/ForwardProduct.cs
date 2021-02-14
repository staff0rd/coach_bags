using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using coach_bags_selenium.Data;

namespace coach_bags_selenium
{
    public class ForwardProduct
    {
        private readonly IElement _element;
        public string Link => "https://www.fwrd.com" + _element.QuerySelectorAll("a.product-grids__link")[0].GetAttribute("href");
        public IEnumerable<string> NameBlock => 
            _element.QuerySelectorAll(".product-grids__copy-item")
            .Take(2)
            .Select(p => p.Text());
        
        public string Brand => NameBlock.ElementAt(0);
        public string Name => NameBlock.ElementAt(1);
        private IElement SalePriceElement => _element.QuerySelectorAll(".price__sale").FirstOrDefault();
        public decimal? SalePrice
        {
            get
            {
                try
                {
                    return SalePriceElement != null ? 
                    (decimal?)decimal.Parse(SalePriceElement.Text()
                        .Replace("AU$ ", "") // AUD
                        .Replace("$", "")) // USD
                         : null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("SalePrice:");
                    Console.WriteLine("----------");
                    Console.WriteLine(SalePriceElement.Text());  
                    Console.WriteLine("----------");
                    throw;
                }
            }
        }

        public decimal Price => decimal.Parse(_element.QuerySelectorAll(".price__retail")[0].Text()
            .Replace("AU$ ", "") // AUD
            .Replace("$", "") // USD
            .Replace("original price", "").Trim());
            
        public decimal Savings => SalePrice.HasValue ? Price - SalePrice.Value : 0;
        public string Id => _element.QuerySelectorAll(".product__image-container")[0].GetAttribute("data-code");
        public string Image(ProductCategory category) => _element
            .QuerySelectorAll(category.GetProductImageClass())[0].GetAttribute("data-lazy-src");
        
        public ForwardProduct(IElement element)
        {
            _element = element;
        }

        public coach_bags_selenium.Data.Product AsEntity(ProductCategory category)
        {
            try
            {
                return new Data.Product
                {
                    Link = Link,
                    Brand = Brand,
                    Name = Name,
                    SalePrice = SalePrice ?? Price,
                    Price = Price,
                    Savings = Savings,
                    Id = Id,
                    Image = Image(category),
                    CategoryId = category.Id,
                    Edit = category.Edit,
                };
            } 
            catch
            {
                Console.WriteLine("InnerHtml...");
                Console.WriteLine(_element.InnerHtml);
                System.IO.File.WriteAllText("payload.txt", _element.InnerHtml);
                throw;
            }
        }
    }
}
