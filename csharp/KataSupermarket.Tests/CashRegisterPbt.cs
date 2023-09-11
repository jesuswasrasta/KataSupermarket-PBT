using System;
using FsCheck;
using FsCheck.Xunit;


namespace KataSupermarket.Tests.PBt;

record Product(int Price);

record OfferArbitraryOffer(int MinimumQuantity, Product Product, int DiscountedPrice);

record UseCase(int NumberOfItems, Product Product);

record UseCaseOffer(int NumberOfItems, Product Product, OfferArbitraryOffer Offer);

record UseCaseTiggeringOffer(
    int ItemsDiscounted,
    int ItemsNotDiscounted,
    Product Product,
    OfferArbitraryOffer Offer);

public class TestsPbt
{
    static Gen<int> PositiveInt() =>
        from price in Arb.Generate<int>()
        select Math.Abs(price) + 1;

    static Gen<int> PositiveIntLargerThan(int threshold) =>
        from price in PositiveInt()
        select price + threshold;

    static Gen<int> PositiveIntSmallerThan(int threshold) =>
        Gen.Choose(1, threshold);

    static Gen<Product> Product() =>
        from price in PositiveInt()
        select new Product(Price: price);

    static Gen<OfferArbitraryOffer> Offer() =>
        from quantity in PositiveInt()
        from product in Product()
        from discountedPrice in PositiveInt()
        select new OfferArbitraryOffer(quantity, product, discountedPrice);

    [Property
    private Property offer_does_not_trigger()
    {
        var useCases = Arb.From(
            from offer in Offer()
            from product in Product()
            from numberOfItems in PositiveIntSmallerThan(offer.MinimumQuantity)
            select new UseCaseOffer(numberOfItems, product, offer));

        bool CheckGrandTotal(UseCaseOffer useCase)
        {
            var cashRegister = new CashRegisterPbt();

            for (int i = 0; i < useCase.NumberOfItems; i++)
            {
                cashRegister.Scan(useCase.Product);
            }

            var grandTotal = cashRegister.Checkout();

            return grandTotal == useCase.Product.Price * useCase.NumberOfItems;
        }

        return Prop.ForAll(useCases, CheckGrandTotal);
    }

    [Property]
    private Property offer_does_triggers()
    {
        var useCases = Arb.From(
            from product in Product()
            from offer in Offer()
            from productNotDiscounted in PositiveInt()
            select new UseCaseTiggeringOffer(
                ItemsDiscounted: offer.MinimumQuantity,
                ItemsNotDiscounted: productNotDiscounted,
                product,
                offer));

        bool CheckGrandTotal(UseCaseTiggeringOffer useCase)
        {
            var cashRegister = new CashRegisterPbt(useCase.Offer);

            for (int i = 0; i < useCase.ItemsDiscounted + useCase.ItemsNotDiscounted; i++)
            {
                cashRegister.Scan(useCase.Product);
            }

            var grandTotal = cashRegister.Checkout();

            return grandTotal ==
                   useCase.ItemsDiscounted * useCase.Offer.DiscountedPrice
                   + useCase.ItemsNotDiscounted * useCase.Product.Price;
        }

        return Prop.ForAll(useCases, CheckGrandTotal);
    }

    [Property]
    private Property sum_product_prices()
    {
        var useCases = Arb.From(
            from numberOfItems in PositiveInt()
            from product in Product()
            select new UseCase(NumberOfItems: numberOfItems, Product: product));

        bool CheckGrandTotal(UseCase useCase)
        {
            var cashRegister = new CashRegisterPbt();

            for (int i = 0; i < useCase.NumberOfItems; i++)
            {
                cashRegister.Scan(useCase.Product);
            }


            var grandTotal = cashRegister.Checkout();

            return grandTotal == useCase.Product.Price * useCase.NumberOfItems;
        }

        return Prop.ForAll(useCases, CheckGrandTotal);
    }
}

internal class CashRegisterPbt
{
    private readonly OfferArbitraryOffer _offer;
    private Product _product;
    private int _count;

    public CashRegisterPbt(OfferArbitraryOffer offer)
    {
        _offer = offer;
    }

    public CashRegisterPbt()
    {
    }

    internal void Scan(Product product)
    {
        _product = product;
        _count++;
    }

    internal int Checkout()
    {
        if(_offer == null)
            return _count * _product.Price;
        
        if (_count < _offer.MinimumQuantity)
            return _count * _product.Price;
        else
        {
            return
                (_count - _offer.MinimumQuantity) * _product.Price +
                _offer.MinimumQuantity * _offer.DiscountedPrice;
        }
    }
}
