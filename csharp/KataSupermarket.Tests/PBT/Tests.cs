using System;
using FsCheck;
using FsCheck.Xunit;
using KataSupermarket.Tests.PBT.UseCases;
using static FsCheck.Prop;

namespace KataSupermarket.Tests.PBT
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
            var cashRegister = new CashRegisterPbt();
            int numberOfApples = positiveInt.Item;

            for (var i = 1; i <= numberOfApples; i++)
            {
                cashRegister.Scan(new Product("apple", 50));
            }

            var grandTotal = cashRegister.Checkout();

            return grandTotal == 50 * numberOfApples;
        }

        [Property]
        private bool grand_total_for_apples_is_positive(PositiveInt positiveInt)
        {
            var cashRegister = new CashRegisterPbt();

            var numberOfApples = positiveInt.Item;

            for (var i = 0; i <= numberOfApples; i++)
            {
                cashRegister.Scan(new Product("apple", 50));
            }

            var grandTotal = cashRegister.Checkout();

            return grandTotal > 0;
        }

        // [Property]
        // private Property only_apples_are_accepted()
        // {
        //     Arbitrary<string> productsOtherThanApple =
        //         Arb.From<string>()
        //             .Filter(s => s != "apple");
        //
        //     bool raisesAnException(string productsOtherThanApple)
        //     {
        //         var cashRegister = new CashRegisterPbt();
        //
        //         try
        //         {
        //             var product = new Product(productsOtherThanApple, 50);
        //             cashRegister.Scan(product);
        //
        //             return false;
        //         }
        //         catch (Exception e)
        //         {
        //             return true;
        //         }
        //     }
        //
        //     return ForAll(productsOtherThanApple, raisesAnException);
        // }

        // // Story 2: pears cost 30
        // // n pears = n * pears
        // // grand total >= 0
        // // scanning a product other than an pear => raise an exception
        // [Property]
        // private Property only_apples_and_pears_are_accepted()
        // {
        //     Arbitrary<string> productsOtherThanApple =
        //         Arb.From<string>()
        //             .Filter(s => s != "apple" && s != "pear");
        //
        //     bool raisesAnException(string productName)
        //     {
        //         var cashRegister = new CashRegisterPbt();
        //
        //         try
        //         {
        //             var product = new Product(productName, 50);
        //             cashRegister.Scan(product);
        //
        //             return false;
        //         }
        //         catch (Exception e)
        //         {
        //             return true;
        //         }
        //     }
        //
        //     return ForAll(productsOtherThanApple, raisesAnException);
        // }

        [Property]
        private Property grand_total_for_apples_and_pears_is_positive()
        {
            var useCases = Arb.From(
                from nApples in Arb.Generate<PositiveInt>()
                from nPears in Arb.Generate<PositiveInt>()
                select new UseCase<int, int>(nApples.Item, nPears.Item));

            bool myProperty(UseCase<int, int> useCase)
            {
                var cashRegister = new CashRegister();

                for (var i = 0; i <= useCase.t1; i++)
                {
                    cashRegister.Scan("apple");
                }

                for (var i = 0; i <= useCase.t2; i++)
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
            var cashRegister = new CashRegisterPbt();
            var quantity = positiveInt.Item;

            for (var i = 1; i <= quantity; i++)
            {
                cashRegister.Scan(new Product("pear", 30));
            }

            var grandTotal = cashRegister.Checkout();

            return grandTotal == 30 * quantity;
        }

        [Property]
        private Property offer_does_not_trigger()
        {
            var useCases = Arb.From(
                from quantity in Generators.PositiveIntBiggerThan(1000)
                from product in Generators.Product()
                from offer in Generators.OfferForProduct(quantity, product)
                from numberOfItems in Generators.PositiveIntSmallerThan(quantity)
                select new UseCase<Product, Offer, int>(product, offer, numberOfItems));

            bool CheckGrandTotal(UseCase<Product, Offer, int> useCase)
            {
                var cashRegister = new CashRegisterPbt();

                for (var i = 0; i < useCase.t3; i++)
                {
                    cashRegister.Scan(useCase.t1);
                }

                var grandTotal = cashRegister.Checkout();

                return grandTotal == useCase.t1.Price * useCase.t3;
            }

            return ForAll(useCases, CheckGrandTotal);
        }

        [Property]
        private Property offer_does_triggers()
        {
            var useCases = Arb.From(
                from quantity in Generators.PositiveIntBiggerThan(1000)
                from product in Generators.Product()
                from offer in Generators.OfferForProduct(quantity, product)
                from numberOfItems in Generators.PositiveIntSmallerThan(offer.MinimumQuantity)
                select new UseCase<Product, Offer, int>(product, offer, numberOfItems));

            bool CheckGrandTotal(UseCase<Product, Offer, int> useCase)
            {
                var cashRegister = new CashRegisterPbt();

                for (var i = 0; i < useCase.t3; i++)
                {
                    cashRegister.Scan(useCase.t1);
                }

                var grandTotal = cashRegister.Checkout();

                return grandTotal == useCase.t1.Price * useCase.t3;
            }

            return ForAll(useCases, CheckGrandTotal);
        }

        [Property]
        private Property sum_product_prices()
        {
            var useCases = Arb.From(
                from numberOfItems in Arb.Generate<PositiveInt>()
                from product in Arb.Generate<Product>()
                select new UseCase<int, Product>(t1: numberOfItems.Item, t2: product));

            bool CheckGrandTotal(UseCase<int, Product> useCase)
            {
                var cashRegister = new CashRegisterPbt();

                for (int i = 0; i < useCase.t1; i++)
                {
                    cashRegister.Scan(useCase.t2);
                }


                var grandTotal = cashRegister.Checkout();

                return grandTotal == useCase.t2.Price * useCase.t1;
            }

            return Prop.ForAll(useCases, CheckGrandTotal);
        }
    }
}