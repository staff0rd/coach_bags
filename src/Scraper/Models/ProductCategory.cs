using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using SixLabors.ImageSharp;

namespace coach_bags_selenium.Data
{
    public partial class ProductCategory : Enumeration
    {
        public static readonly ProductCategory CoachBags = new CoachCategory();
        public static readonly ProductCategory FwrdShoes = new FwrdCategory(1, ProductType.Shoes);
        public static readonly ProductCategory FwrdDresses = new FwrdCategory(2, ProductType.Dresses);
        public static readonly ProductCategory FwrdBags = new FwrdCategory(3, ProductType.Bags);
        public static readonly ProductCategory OutnetCoats = new OutnetCategory(4, ProductType.Coats);
        public static readonly ProductCategory FarfetchDresses = new FarfetchCategory(5, ProductType.Dresses);
        public static readonly ProductCategory FarfetchShoes = new FarfetchCategory(6, ProductType.Shoes);
        public static readonly ProductCategory OutnetShoes = new OutnetCategory(7, ProductType.Shoes);

        public ProductType ProductType { get; private set; }

        public virtual string GetProductImageClass()
        {
            return null;
        }

        public virtual Task<ProductMetadata> GetProductMetadataFromUrl(ChromeDriver driver, Product product)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<Product>> GetProducts(ChromeDriver driver, int maxCount)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetProductsUrl(int pageNumber)
        {
            throw new NotImplementedException();
        }

        public virtual Size GetTwitterImageSize(int count) => new Size (2400, 1256);

        public ProductCategory() { }
        public ProductCategory(int value, string displayName, ProductType productType) : base(value, displayName)
        {
            ProductType = productType;
        }
    }

}