using System.Linq;
using FsCheck;

namespace KataSupermarket.Tests.PBT.Generators;

public static class OfferGenerator
{
    public static Gen<Offer> OfferForProduct(int quantity, Product product) =>
        from discountedPrice in IntGenerator.PositiveInt()
        select new Offer(quantity, product, discountedPrice);

    public static Gen<Offer> NonTriggeringOffer() =>
        from quantity in IntGenerator.PositiveInt()
        from product in ProductGenerator.Product()
        from discountedPrice in IntGenerator.PositiveInt()
        select new Offer(quantity, product, discountedPrice);
    
    public static Offer ToOffer(this Gen<Offer> generator) =>
        generator.Sample(1, 1).First();
}