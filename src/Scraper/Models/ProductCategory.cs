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
        public static readonly ProductCategory FwrdDressesAll = new FwrdCategory(11, ProductType.Dresses, false, Edit.FwrdDresses);
        public static readonly ProductCategory IconicDresses = new IconicCategory(12, ProductType.Dresses, Edit.IconicDresses);
        public static readonly ProductCategory PradaBags = new PradaCategory(13, ProductType.Bags, Edit.LegitBags);
        public static readonly ProductCategory MichaelKorsBags = new MichaelKorsCategory(14, ProductType.Bags, Edit.LegitBags);
        public static readonly ProductCategory CoachBagsLegit = new CoachCategory();
        public ProductType ProductType { get; private set; }
        public Edit Edit { get; protected set; }

        public virtual string GetProductImageClass()
        {
            throw new NotImplementedException();
        }

        public virtual Task<ProductMetadata> GetProductMetadataFromUrl(Browser browser, Product product)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<Product>> GetProducts(Browser browser, int maxCount)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetProductsUrl(int pageNumber)
        {
            throw new NotImplementedException();
        }

        public virtual Size GetTwitterImageSize(int count) => new Size (2400, 1256);

        public ProductCategory() { }
        public ProductCategory(int value, string displayName, ProductType productType, Edit edit) : base(value, displayName)
        {
            ProductType = productType;
            Edit = edit;
        }
    }

}