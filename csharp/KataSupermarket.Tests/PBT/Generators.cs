using System;
using System.Linq;
using FsCheck;

namespace KataSupermarket.Tests.PBT;

public class Generators
{
    public static Gen<int> PositiveInt() =>
        from value in Arb.Generate<int>()
        select Math.Abs(value) + 1;

    public static Gen<int> PositiveIntLargerThan(int threshold) =>
        from value in PositiveInt()
        select value + threshold;

    public static Gen<int> PositiveIntSmallerThan(int threshold) =>
        Gen.Choose(1, threshold - 1);

    public static Gen<int> PositiveIntBiggerThan(int threshold) =>
        // Gen.Elements(Enumerable.Range(threshold + 1, int.MaxValue - threshold).ToArray());
        Gen.Choose(threshold + 1, threshold * 2);

    public static Gen<string> NonEmptyString() =>
        Arb.Default.String().Filter(s => !string.IsNullOrEmpty(s)).Generator;

    public static Gen<string> NonBlankAlphaNumericString()
    {
        // Define a string that holds all alphanumeric characters.
        const string alphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // Define a generator for indexes into the alphanumeric string.
        var indexGen = Gen.Choose(0, alphanumeric.Length - 1);

        // Define a generator for alphanumeric character:
        var alphanumericCharGen = indexGen.Select(i => alphanumeric[i]);

        // Define a generator for non-blank, alphanumeric strings, that involves generating lists of alphanumeric characters and joining them into strings.
        return Gen.NonEmptyListOf(alphanumericCharGen).Select(chars => new string(chars.ToArray()));
    }

    public static Gen<Offer> OfferForProduct(int quantity, Product product) =>
        from discountedPrice in PositiveInt()
        select new Offer(quantity, product, discountedPrice);

    public static Gen<Offer> NonTriggeringOffer() =>
        from quantity in PositiveInt()
        from product in Product()
        from discountedPrice in PositiveInt()
        select new Offer(quantity, product, discountedPrice);

    public static Gen<Product> Product() =>
        from price in PositiveInt()
        from name in NonBlankAlphaNumericString()
        select new Product(name, price);

    public static Gen<Product> ProductOtherThan(string productName) =>
        from price in PositiveInt()
        from name in Arb.From<string>().Filter(s => s != productName).Generator
        select new Product(name, price);
}