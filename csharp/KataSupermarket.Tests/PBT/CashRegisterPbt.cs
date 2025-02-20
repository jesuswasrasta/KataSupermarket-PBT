using System;

namespace KataSupermarket.Tests.PBT;

public class CashRegisterPbt
{
    private readonly Offer _offer;
    private Product _product;
    private int _count;

    public CashRegisterPbt(Offer offer)
    {
        _offer = offer;
    }

    public CashRegisterPbt()
    {
    }

    internal void Scan(Product product)
    {
        if (product.Price <= 0)
        {
            throw new ArgumentException("Product price must be 1 or more");
        }
        
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