using Xunit;

namespace KataSupermarket.Tests.EBT;

public class TddTests
{
    [Fact (DisplayName = "An apple costs 50 cents")]
    void an_apple_costs_50_cents()
    {
        var cashRegister = new CashRegister();

        cashRegister.Scan("apple");

        var actual = cashRegister.Checkout();

        var expected = 50;
        Assert.Equal(expected, actual);
    }
}