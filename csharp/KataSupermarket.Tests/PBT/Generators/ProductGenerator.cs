using System.Linq;
using FsCheck;

namespace KataSupermarket.Tests.PBT.Generators;

public static class ProductGenerator
{
    public static Gen<Product> Product() =>
        from price in IntGenerator.PositiveInt()
        from name in StringGenerator.NonBlankAlphaNumericString()
        select new Product(name, price);

    public static Gen<Product> ProductOtherThan(string productName) =>
        from price in IntGenerator.PositiveInt()
        from name in Arb.From<string>().Filter(s => s != productName).Generator
        select new Product(name, price);

    public static Gen<Product> InvalidPriceProduct() =>
        from price in IntGenerator.ZeroOrNegativeInt()
        from name in StringGenerator.NonBlankAlphaNumericString()
        select new Product(name, price);

    public static Product ToProduct(this Gen<Product> generator) =>
        generator.Sample(1, 1).First();
}