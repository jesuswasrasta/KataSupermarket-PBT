using System;
using FsCheck;
using FsCheck.Xunit;
using KataSupermarket.Tests.PBT.Generators;
using KataSupermarket.Tests.PBT.UseCases;
using KataSupermarket.Tests.TDD;
using static FsCheck.Prop;

namespace KataSupermarket.Tests.PBT
{
    public class Tests
    {
        /*
         Story 1 PBT:
        
         As a cashier,
         I wish my customers can pay for one kind of product
         so that the grand total, based on the product price, is displayed
        
         scanning a product other than an apple => raise an exception
        */
                
        [Property]
        private bool product_price_is_positive()
        {
            /* Ha senso questa proprietà? Sì, perché il prezzo di un prodotto non può essere negativo.
             * Ma qui gli passo io un prezzo positivo...
             */
            string product = StringGenerator.NonBlankAlphaNumericString().ToString();
            int price = IntGenerator.PositiveInt().ToInt();
            
            var cashRegister = new CashRegisterPbt();
            cashRegister.Scan(new Product(product, price));
            var grandTotal = cashRegister.Checkout();

            return grandTotal > 0;
        }

        [Property]
        private bool product_price_is_positive_v2()
        {
            /* Come prima, visto che mi faccio generare un prodotto valido 
             */
            var product = ProductGenerator.Product().ToProduct();
            
            var cashRegister = new CashRegisterPbt();
            cashRegister.Scan(product);
            var grandTotal = cashRegister.Checkout();

            return grandTotal > 0;
        }
        
        [Property]
        private bool scan_one_product_then_grand_total_equals_product_price()
        {
            /* Proprietà un pelo più precisa, ma di nuovo testo i generators...
             */
            var product = ProductGenerator.Product().ToProduct();
            
            var cashRegister = new CashRegisterPbt();
            cashRegister.Scan(product);
            var grandTotal = cashRegister.Checkout();

            return grandTotal > 0 && grandTotal == product.Price;
        }
        
        [Property(DisplayName = "Scan a product with negative price throws exception")]
        private Property scan_a_product_with_negative_price_throws_exception()
        {
            /* C'è un modo migliore per testare questa proprietà?
             * E le eccezioni in generale?
             */
            
            var productArbitrary = ProductGenerator.InvalidPriceProduct().ToArbitrary();

            return Prop.ForAll(productArbitrary, product =>
            {
                var cashRegister = new CashRegisterPbt();
                try
                {
                    cashRegister.Scan(product);
                    return false; // If no exception is thrown, the test fails
                }
                catch (ArgumentException)
                {
                    return true; // If ArgumentException is thrown, the test passes
                }
            });
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
                from quantity in IntGenerator.PositiveIntBiggerThan(1000)
                from product in ProductGenerator.Product()
                from offer in OfferGenerator.OfferForProduct(quantity, product)
                from numberOfItems in IntGenerator.PositiveIntSmallerThan(quantity)
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
                from quantity in IntGenerator.PositiveIntBiggerThan(1000)
                from product in ProductGenerator.Product()
                from offer in OfferGenerator.OfferForProduct(quantity, product)
                from numberOfItems in IntGenerator.PositiveIntSmallerThan(offer.MinimumQuantity)
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