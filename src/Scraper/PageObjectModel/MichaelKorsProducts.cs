// <auto-generated />

namespace coach_bags_selenium.MichaelKors
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class MichaelKorsProducts
    {
        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("pagination")]
        public Pagination[] Pagination { get; set; }

        [JsonProperty("sortOptions")]
        public SortOption[] SortOptions { get; set; }

        [JsonProperty("alternateURL")]
        public string AlternateUrl { get; set; }

        [JsonProperty("facets")]
        public Facet[] Facets { get; set; }
    }

    public partial class Facet
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refinementType")]
        public string RefinementType { get; set; }

        [JsonProperty("refinements")]
        public Refinement[] Refinements { get; set; }
    }

    public partial class Refinement
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("dimensionId")]
        public string DimensionId { get; set; }

        [JsonProperty("multiSelect")]
        public bool MultiSelect { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("isSelected")]
        public bool IsSelected { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("stateId")]
        public string StateId { get; set; }
    }

    public partial class Pagination
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("stateId")]
        public string StateId { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("totalProducts")]
        public long TotalProducts { get; set; }

        [JsonProperty("productsPerPage")]
        public long ProductsPerPage { get; set; }

        [JsonProperty("lastProduct")]
        public long LastProduct { get; set; }

        [JsonProperty("firstProduct")]
        public long FirstProduct { get; set; }

        [JsonProperty("nParam")]
        public string NParam { get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        [JsonProperty("loyaltyTiers")]
        public string LoyaltyTiers { get; set; }

        [JsonProperty("isValidLoyality")]
        public bool IsValidLoyality { get; set; }

        [JsonProperty("loyalityUserValue")]
        public string LoyalityUserValue { get; set; }

        [JsonProperty("accessibleTiers")]
        public string AccessibleTiers { get; set; }

        [JsonProperty("accessibleTiersEncript")]
        public string AccessibleTiersEncript { get; set; }

        [JsonProperty("korsVipLogoAltText")]
        public string KorsVipLogoAltText { get; set; }

        [JsonProperty("productList")]
        public ProductList[] ProductList { get; set; }
    }

    public partial class ProductList
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("productSEOURL")]
        public string ProductSeourl { get; set; }

        [JsonProperty("longDescription")]
        public string LongDescription { get; set; }

        [JsonProperty("moreColors")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long MoreColors { get; set; }

        [JsonProperty("monogramableProduct")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long MonogramableProduct { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("colorCode")]
        public string ColorCode { get; set; }

        [JsonProperty("media")]
        public ProductListMedia Media { get; set; }

        [JsonProperty("prices")]
        public Prices Prices { get; set; }

        [JsonProperty("variant_options")]
        public VariantOptions VariantOptions { get; set; }

        [JsonProperty("SKUs")]
        public SkUs[] SkUs { get; set; }

        public coach_bags_selenium.Data.Product AsEntity(Data.ProductCategory category) => new Data.Product
        {
            Link = "https://www.michaelkors.global/en_AU" + ProductSeourl,
            Name = SkUs.First().Name,
            Brand = Brand,
            CategoryId = category.Id,
            SalePrice = decimal.Parse(Prices.LowSalePrice),
            Price = decimal.Parse(Prices.HighListPrice),
            Savings = decimal.Parse(Prices.HighListPrice) - decimal.Parse(Prices.LowSalePrice),
            Id = Identifier,
            Image = Media.AlternativeImages.FirstOrDefault()?.ImageUrl,
            Edit = category.Edit
        };
    }

    public partial class ProductListMedia
    {
        [JsonProperty("alternativeImages")]
        public AlternativeImage[] AlternativeImages { get; set; }

        [JsonProperty("mediaSet")]
        public string MediaSet { get; set; }
    }

    public partial class AlternativeImage
    {
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        [JsonProperty("allSuffixes")]
        public string AllSuffixes { get; set; }
    }

    public partial class Prices
    {
        [JsonProperty("highListPrice")]
        public string HighListPrice { get; set; }

        [JsonProperty("lowSalePrice")]
        public string LowSalePrice { get; set; }

        [JsonProperty("highSalePrice")]
        public string HighSalePrice { get; set; }

        [JsonProperty("lowListPrice")]
        public string LowListPrice { get; set; }
    }

    public partial class SkUs
    {
        [JsonProperty("variant_values")]
        public VariantValues VariantValues { get; set; }

        [JsonProperty("identifier")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Identifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("media")]
        public SkUsMedia Media { get; set; }
    }

    public partial class SkUsMedia
    {
        [JsonProperty("mediaSet")]
        public string MediaSet { get; set; }

        [JsonProperty("alternateImage")]
        public object[] AlternateImage { get; set; }

        [JsonProperty("swatchImage")]
        public SwatchImage SwatchImage { get; set; }
    }

    public partial class SwatchImage
    {
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }
    }

    public partial class VariantValues
    {
        [JsonProperty("color")]
        public Color Color { get; set; }
    }

    public partial class Color
    {
        [JsonProperty("colorCode")]
        public string ColorCode { get; set; }

        [JsonProperty("colorName")]
        public string ColorName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("identifier")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Identifier { get; set; }
    }

    public partial class VariantOptions
    {
        [JsonProperty("colors")]
        public Color[] Colors { get; set; }
    }

    public partial class SortOption
    {
        [JsonProperty("selected")]
        public bool Selected { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("stateId")]
        public string StateId { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    public partial class MichaelKorsProducts
    {
        public static MichaelKorsProducts FromJson(string json) => JsonConvert.DeserializeObject<MichaelKorsProducts>(json, coach_bags_selenium.Ferragamo.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MichaelKorsProducts self) => JsonConvert.SerializeObject(self, coach_bags_selenium.Ferragamo.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
