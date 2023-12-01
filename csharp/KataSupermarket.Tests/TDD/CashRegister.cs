using System;

namespace KataSupermarket.Tests;

public class CashRegister
{
    private int _countApples;
    private int _countPears;

    internal void Scan(string product)
    {
        if (product == "apple") _countApples++;
        else if (product == "pear") _countPears++;
        else throw new Exception("Only apples are supported");
    }

    internal int Checkout() => 50 * _countApples + 30 * _countPears;
}