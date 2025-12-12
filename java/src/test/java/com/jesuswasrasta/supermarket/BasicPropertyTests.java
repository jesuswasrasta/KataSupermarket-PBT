package com.jesuswasrasta.supermarket;

import net.jqwik.api.*;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Basic Property-Based Testing examples using integers.
 * These tests demonstrate fundamental PBT concepts with simple numeric properties.
 */
class BasicPropertyTests {

    /**
     * PROPERTY 1: Prices are always non-negative
     * This is a basic invariant - prices in cents should never be negative.
     */
    @Property
    void pricesShouldAlwaysBeNonNegative(@ForAll @Positive int priceInCents) {
        Product product = new Product("TestProduct", priceInCents);
        assertTrue(product.getPriceInCents() >= 0,
                  "Product price should never be negative");
    }

    /**
     * PROPERTY 2: Total calculation is commutative (order doesn't matter)
     * Scanning products in different orders should give the same total.
     */
    @Property
    void totalIsIndependentOfScanningOrder(
            @ForAll @IntRange(min = 1, max = 500) int price1,
            @ForAll @IntRange(min = 1, max = 500) int price2) {

        // Scenario 1: Scan product1 then product2
        Checkout checkout1 = new Checkout();
        checkout1.scan(new Product("Apple", price1));
        checkout1.scan(new Product("Pear", price2));
        int total1 = checkout1.calculateTotal();

        // Scenario 2: Scan product2 then product1
        Checkout checkout2 = new Checkout();
        checkout2.scan(new Product("Pear", price2));
        checkout2.scan(new Product("Apple", price1));
        int total2 = checkout2.calculateTotal();

        assertEquals(total1, total2,
                    "Total should be the same regardless of scanning order");
    }

    /**
     * PROPERTY 3: Total equals sum of individual prices
     * The mathematical property: total = price1 + price2 + ... + priceN
     */
    @Property
    void totalEqualsSumOfPrices(
            @ForAll @IntRange(min = 1, max = 200) int applePrice,
            @ForAll @IntRange(min = 1, max = 100) int pearPrice,
            @ForAll @IntRange(min = 1, max = 300) int pineapplePrice) {

        Checkout checkout = new Checkout();
        checkout.scan(new Product("Apple", applePrice));
        checkout.scan(new Product("Pear", pearPrice));
        checkout.scan(new Product("Pineapple", pineapplePrice));

        int expectedTotal = applePrice + pearPrice + pineapplePrice;
        int actualTotal = checkout.calculateTotal();

        assertEquals(expectedTotal, actualTotal,
                    "Total should equal the sum of all prices");
    }

    /**
     * PROPERTY 4: Scanning N identical items costs N times the unit price
     * This tests the multiplicative property for identical items.
     */
    @Property
    void scanningNItemsCostsNTimesUnitPrice(
            @ForAll @IntRange(min = 1, max = 100) int unitPrice,
            @ForAll @IntRange(min = 1, max = 10) int quantity) {

        Checkout checkout = new Checkout();
        Product product = new Product("Apple", unitPrice);

        // Scan the product 'quantity' times
        for (int i = 0; i < quantity; i++) {
            checkout.scan(product);
        }

        int expectedTotal = unitPrice * quantity;
        int actualTotal = checkout.calculateTotal();

        assertEquals(expectedTotal, actualTotal,
                    String.format("Scanning %d items at %d cents should cost %d cents",
                                 quantity, unitPrice, expectedTotal));
    }

    /**
     * PROPERTY 5: Adding zero-price items doesn't change the total
     * Edge case: products with zero price (free items).
     */
    @Property
    void addingZeroPriceItemsDoesNotChangeTotal(
            @ForAll @IntRange(min = 1, max = 500) int initialTotal,
            @ForAll @IntRange(min = 0, max = 5) int freeItemsCount) {

        Checkout checkout = new Checkout();
        checkout.scan(new Product("Apple", initialTotal));

        int totalBeforeFreeItems = checkout.calculateTotal();

        // Add free items
        for (int i = 0; i < freeItemsCount; i++) {
            checkout.scan(new Product("FreeItem", 0));
        }

        int totalAfterFreeItems = checkout.calculateTotal();

        assertEquals(totalBeforeFreeItems, totalAfterFreeItems,
                    "Adding zero-price items should not change the total");
    }

    /**
     * PROPERTY 6: Item count equals number of scans
     * A simple counting property - we should count what we scan.
     */
    @Property
    void itemCountEqualsNumberOfScans(
            @ForAll @IntRange(min = 0, max = 20) int numberOfScans) {

        Checkout checkout = new Checkout();
        Product product = new Product("TestProduct", 50);

        for (int i = 0; i < numberOfScans; i++) {
            checkout.scan(product);
        }

        assertEquals(numberOfScans, checkout.getItemCount(),
                    "Item count should equal the number of scans");
    }
}
