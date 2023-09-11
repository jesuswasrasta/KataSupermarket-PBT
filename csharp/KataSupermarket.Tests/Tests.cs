using System;
using System.ComponentModel.Design;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using static FsCheck.Prop;

namespace KataSupermarket.Tests
{
    public class Tests
    {
        // Story 1: only apples, without offers
        // Example: 1 apple, 50cc
        // n apples = n * apples
        // grand total >= 0
        // scanning a product other than an apple => raise an exception
        
        [Property]
        private bool n_apples_cost_n_times_50(PositiveInt positiveInt)
        {
            var cashRegister = new CashRegister();
            int numberOfApples = positiveInt.Item;

            for (var i = 1; i <= numberOfApples; i++)
            {
                cashRegister.Scan("apple");
            }

            var grandTotal = cashRegister.Checkout();

            return grandTotal == 50 * numberOfApples;
        }

        [Property]
        private bool grand_total_for_apples_is_positive(PositiveInt positiveInt)
        {
            var cashRegister = new CashRegister();

            var numberOfApples = positiveInt.Item;

            for (var i = 0; i <= numberOfApples; i++)
            {
                cashRegister.Scan("apple");
            }

            var grandTotal = cashRegister.Checkout();

            return grandTotal > 0;
        }

        [Property]
        private Property only_apples_are_accepted()
        {
            Arbitrary<string> productsOtherThanApple = Arb.From<string>().Filter(s => s != "apple");

            bool raisesAnException(string product)
            {
                var cashRegister = new CashRegister();

                try
                {
                    cashRegister.Scan(product);

                    return false;
                }
                catch (Exception e)
                {
                    return true;
                }
            }

            return ForAll(productsOtherThanApple, raisesAnException);
        }

        // Story 2: pears cost 30
        // n pears = n * pears
        // grand total >= 0
        // scanning a product other than an pear => raise an exception

        [Property]
        private Property only_apples_and_pears_are_accepted()
        {
            Arbitrary<string> productsOtherThanApple =
                Arb.From<string>()
                    .Filter(s => s != "apple" && s != "pear");

            bool raisesAnException(string product)
            {
                var cashRegister = new CashRegister();

                try
                {
                    cashRegister.Scan(product);

                    return false;
                }
                catch (Exception e)
                {
                    return true;
                }
            }

            return ForAll(productsOtherThanApple, raisesAnException);
        }

        [Property]
        private Property grand_total_for_apples_and_pears_is_positive()
        {
            var useCases = Arb.From(
                from nApples in Arb.Generate<PositiveInt>()
                from nPears in Arb.Generate<PositiveInt>()
                select new UseCase(nApples.Item, nPears.Item));

            bool myProperty(UseCase useCase)
            {
                var cashRegister = new CashRegister();

                for (var i = 0; i <= useCase.NApple; i++)
                {
                    cashRegister.Scan("apple");
                }

                for (var i = 0; i <= useCase.NPear; i++)
                {
                    cashRegister.Scan("pear");
                }

                var checkout = cashRegister.Checkout();

                return checkout > 0;
            }

            return ForAll(useCases, myProperty);
        }


        [Property]
        private bool n_pears_cost_n_times_30(PositiveInt positiveInt)
        {
            var cashRegister = new CashRegister();
            int numberOfApples = positiveInt.Item;

            for (var i = 1; i <= numberOfApples; i++)
            {
                cashRegister.Scan("pear");
            }

            var grandTotal = cashRegister.Checkout();

            return grandTotal == 30 * numberOfApples;
        }
    }

    record UseCase(int NApple, int NPear);

    public class Tdd
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

    internal class CashRegister
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
}
