using Xunit;

namespace KataSupermarket.Tests;

public class TddTests
{
    [Fact]
    void an_apple_costs_50_cc()
    {
        var cashRegister = new CashRegister();

        cashRegister.Scan("apple");

        var actual = cashRegister.Checkout();

        var expected = 50;
        Assert.Equal(expected, actual);
    }
}