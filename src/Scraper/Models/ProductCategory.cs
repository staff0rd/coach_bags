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
        public static readonly ProductCategory FwrdShoes = new FwrdCategory(1, ProductType.Shoes, true);
        public static readonly ProductCategory FwrdDresses = new FwrdCategory(2, ProductType.Dresses, true);
        public static readonly ProductCategory FwrdBags = new FwrdCategory(3, ProductType.Bags, true);
        public static readonly ProductCategory OutnetCoats = new OutnetCategory(4, ProductType.Coats);
        public static readonly ProductCategory FarfetchDresses = new FarfetchCategory(5, ProductType.Dresses);
        public static readonly ProductCategory FarfetchShoes = new FarfetchCategory(6, ProductType.Shoes);
        public static readonly ProductCategory OutnetShoes = new OutnetCategory(7, ProductType.Shoes);
        public static readonly ProductCategory ToryBurchBags = new ToryBurchCategory(8, ProductType.Bags);
        public static readonly ProductCategory FerragamoBags = new FerragamoCategory(9, ProductType.Bags);
        public static readonly ProductCategory RebeccaMinkoffBags = new RebeccaMinkoffCategory(10, ProductType.Bags);
        public static readonly ProductCategory FwrdDressesAll = new FwrdCategory(11, ProductType.Dresses, false);

        public ProductType ProductType { get; private set; }

        public virtual string GetProductImageClass()
        {
            throw new NotImplementedException();
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