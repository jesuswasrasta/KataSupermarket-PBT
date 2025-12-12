package com.jesuswasrasta.supermarket;

import net.jqwik.api.*;

import java.util.Arrays;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Demonstrates Custom Arbitraries for Product domain.
 * Shows how to generate domain-specific test data that represents realistic products.
 */
class ProductArbitraryTests {

    /**
     * CUSTOM ARBITRARY 1: Generate random products with any name and price
     * This is the simplest form of custom Arbitrary using Combinators.
     */
    @Provide
    Arbitrary<Product> anyProduct() {
        Arbitrary<String> names = Arbitraries.strings()
                .alpha()
                .ofMinLength(3)
                .ofMaxLength(15);

        Arbitrary<Integer> prices = Arbitraries.integers()
                .between(1, 1000);

        return Combinators.combine(names, prices)
                .as(Product::new);
    }

    /**
     * CUSTOM ARBITRARY 2: Generate realistic supermarket products
     * Uses actual fruit names and realistic price ranges.
     */
    @Provide
    Arbitrary<Product> supermarketProduct() {
        List<String> fruitNames = Arrays.asList(
                "Apple", "Pear", "Pineapple", "Banana",
                "Orange", "Mango", "Strawberry", "Peach"
        );

        Arbitrary<String> names = Arbitraries.of(fruitNames);

        // Realistic price range: 20 cents to 500 cents
        Arbitrary<Integer> prices = Arbitraries.integers()
                .between(20, 500);

        return Combinators.combine(names, prices)
                .as(Product::new);
    }

    /**
     * CUSTOM ARBITRARY 3: Generate specific catalog products with fixed prices
     * Represents the actual product catalog from the kata.
     */
    @Provide
    Arbitrary<Product> catalogProduct() {
        return Arbitraries.of(
                new Product("Apple", 50),
                new Product("Pear", 30),
                new Product("Pineapple", 220),
                new Product("Banana", 60),
                new Product("Orange", 45)
        );
    }

    /**
     * CUSTOM ARBITRARY 4: Generate expensive products
     * Demonstrates filtered/constrained Arbitraries.
     */
    @Provide
    Arbitrary<Product> expensiveProduct() {
        return supermarketProduct()
                .filter(p -> p.getPriceInCents() >= 200);
    }

    /**
     * CUSTOM ARBITRARY 5: Generate a shopping basket (list of products)
     * Shows how to build collections from custom Arbitraries.
     */
    @Provide
    Arbitrary<List<Product>> shoppingBasket() {
        return catalogProduct()
                .list()
                .ofMinSize(1)
                .ofMaxSize(10);
    }

    // ========== PROPERTY TESTS USING CUSTOM ARBITRARIES ==========

    /**
     * PROPERTY 1: Product names are never empty
     * Tests using the anyProduct arbitrary.
     */
    @Property
    void productNameIsNeverEmpty(@ForAll("anyProduct") Product product) {
        assertNotNull(product.getName());
        assertFalse(product.getName().trim().isEmpty(),
                   "Product name should not be empty");
    }

    /**
     * PROPERTY 2: All supermarket products have realistic prices
     * Tests the constraints of our supermarketProduct arbitrary.
     */
    @Property
    void supermarketProductsHaveRealisticPrices(
            @ForAll("supermarketProduct") Product product) {
        int price = product.getPriceInCents();
        assertTrue(price >= 20 && price <= 500,
                  String.format("Price %d should be between 20 and 500 cents", price));
    }

    /**
     * PROPERTY 3: Catalog products match expected prices
     * Validates that our catalog contains known products.
     */
    @Property
    void catalogProductsMatchExpectedPrices(
            @ForAll("catalogProduct") Product product) {
        // Verify the product is from our catalog
        assertTrue(
                Arrays.asList("Apple", "Pear", "Pineapple", "Banana", "Orange")
                        .contains(product.getName()),
                "Product should be from the catalog"
        );

        // Verify price is positive
        assertTrue(product.getPriceInCents() > 0,
                  "Catalog products should have positive prices");
    }

    /**
     * PROPERTY 4: Expensive products are indeed expensive
     * Tests the filtered arbitrary.
     */
    @Property
    void expensiveProductsAreExpensive(
            @ForAll("expensiveProduct") Product product) {
        assertTrue(product.getPriceInCents() >= 200,
                  String.format("Expensive product %s should cost at least 200 cents, but costs %d",
                               product.getName(), product.getPriceInCents()));
    }

    /**
     * PROPERTY 5: Shopping baskets are never empty
     * Tests the list arbitrary.
     */
    @Property
    void shoppingBasketsAreNeverEmpty(
            @ForAll("shoppingBasket") List<Product> basket) {
        assertFalse(basket.isEmpty(),
                   "Shopping basket should contain at least one product");
        assertTrue(basket.size() <= 10,
                  "Shopping basket should not exceed 10 items");
    }

    /**
     * PROPERTY 6: Checkout handles any valid basket
     * Integration test using custom basket arbitrary.
     */
    @Property
    void checkoutHandlesAnyBasket(
            @ForAll("shoppingBasket") List<Product> basket) {
        Checkout checkout = new Checkout();

        // Scan all products
        basket.forEach(checkout::scan);

        // Properties to verify
        assertEquals(basket.size(), checkout.getItemCount(),
                    "Item count should match basket size");

        int expectedTotal = basket.stream()
                .mapToInt(Product::getPriceInCents)
                .sum();
        assertEquals(expectedTotal, checkout.calculateTotal(),
                    "Total should match sum of all product prices");

        assertTrue(checkout.calculateTotal() >= 0,
                  "Total should never be negative");
    }

    /**
     * PROPERTY 7: Two products with same name and price are equal
     * Tests the Product equality contract using generated data.
     */
    @Property
    void productsWithSameNameAndPriceAreEqual(
            @ForAll("supermarketProduct") Product original) {
        Product duplicate = new Product(
                original.getName(),
                original.getPriceInCents()
        );

        assertEquals(original, duplicate,
                    "Products with same name and price should be equal");
        assertEquals(original.hashCode(), duplicate.hashCode(),
                    "Equal products should have same hash code");
    }

    /**
     * PROPERTY 8: Product toString contains name and price
     * Tests the string representation.
     */
    @Property
    void productToStringContainsNameAndPrice(
            @ForAll("anyProduct") Product product) {
        String str = product.toString();
        assertTrue(str.contains(product.getName()),
                  "toString should contain product name");
        assertTrue(str.contains(String.valueOf(product.getPriceInCents())),
                  "toString should contain price");
    }
}
