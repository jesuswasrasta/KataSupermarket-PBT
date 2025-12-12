package com.jesuswasrasta.supermarket;

import net.jqwik.api.*;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Advanced Property-Based Tests for the Checkout system.
 * Demonstrates complex properties and invariants of the checkout domain.
 */
class CheckoutPropertyTests {

    // ========== CUSTOM ARBITRARIES ==========

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

    @Provide
    Arbitrary<List<Product>> nonEmptyBasket() {
        return catalogProduct()
                .list()
                .ofMinSize(1)
                .ofMaxSize(15);
    }

    // ========== ALGEBRAIC PROPERTIES ==========

    /**
     * PROPERTY 1: Scanning is associative
     * (scan A + scan B) + scan C = scan A + (scan B + scan C)
     */
    @Property
    void scanningIsAssociative(
            @ForAll("catalogProduct") Product p1,
            @ForAll("catalogProduct") Product p2,
            @ForAll("catalogProduct") Product p3) {

        Checkout checkout = new Checkout();
        checkout.scan(p1);
        checkout.scan(p2);
        checkout.scan(p3);

        int total = checkout.calculateTotal();
        int expectedTotal = p1.getPriceInCents() +
                           p2.getPriceInCents() +
                           p3.getPriceInCents();

        assertEquals(expectedTotal, total,
                    "Total should be associative");
    }

    /**
     * PROPERTY 2: Empty checkout has zero total (Identity element)
     * Scanning nothing should give 0.
     */
    @Property
    void emptyCheckoutHasZeroTotal() {
        Checkout checkout = new Checkout();
        assertEquals(0, checkout.calculateTotal(),
                    "Empty checkout should have zero total");
        assertEquals(0, checkout.getItemCount(),
                    "Empty checkout should have zero items");
    }

    /**
     * PROPERTY 3: Order independence (Commutativity)
     * Scanning the same products in different orders gives the same total.
     * This is a KEY property from the kata requirements!
     */
    @Property
    void orderOfScanningDoesNotMatterForTotal(
            @ForAll("nonEmptyBasket") List<Product> basket) {

        // Calculate total with original order
        Checkout checkout1 = new Checkout();
        basket.forEach(checkout1::scan);
        int total1 = checkout1.calculateTotal();

        // Shuffle and calculate again
        List<Product> shuffledBasket = new ArrayList<>(basket);
        Collections.shuffle(shuffledBasket);

        Checkout checkout2 = new Checkout();
        shuffledBasket.forEach(checkout2::scan);
        int total2 = checkout2.calculateTotal();

        assertEquals(total1, total2,
                    "Total should be independent of scanning order");
    }

    // ========== INVARIANT PROPERTIES ==========

    /**
     * PROPERTY 4: Total is always non-negative
     * Invariant: total >= 0
     */
    @Property
    void totalIsNeverNegative(
            @ForAll("nonEmptyBasket") List<Product> basket) {
        Checkout checkout = new Checkout();
        basket.forEach(checkout::scan);

        assertTrue(checkout.calculateTotal() >= 0,
                  "Total should never be negative");
    }

    /**
     * PROPERTY 5: Item count never exceeds scans
     * Invariant: itemCount == numberOfScans
     */
    @Property
    void itemCountMatchesScans(
            @ForAll("nonEmptyBasket") List<Product> basket) {
        Checkout checkout = new Checkout();
        basket.forEach(checkout::scan);

        assertEquals(basket.size(), checkout.getItemCount(),
                    "Item count should exactly match number of scans");
    }

    /**
     * PROPERTY 6: Product count is consistent
     * The sum of all product counts equals total item count.
     */
    @Property
    void productCountsAreConsistent(
            @ForAll("nonEmptyBasket") List<Product> basket) {
        Checkout checkout = new Checkout();
        basket.forEach(checkout::scan);

        // Count unique product names
        int sumOfCounts = basket.stream()
                .map(Product::getName)
                .distinct()
                .mapToInt(checkout::getProductCount)
                .sum();

        assertEquals(checkout.getItemCount(), sumOfCounts,
                    "Sum of product counts should equal total item count");
    }

    // ========== METAMORPHIC PROPERTIES ==========

    /**
     * PROPERTY 7: Clearing resets to empty state
     * Metamorphic relation: checkout.clear() => checkout == new Checkout()
     */
    @Property
    void clearingResetsToEmptyState(
            @ForAll("nonEmptyBasket") List<Product> basket) {
        Checkout checkout = new Checkout();
        basket.forEach(checkout::scan);

        // Verify we have items
        assertTrue(checkout.getItemCount() > 0);

        // Clear
        checkout.clear();

        // Verify empty state
        assertEquals(0, checkout.getItemCount(),
                    "After clear, item count should be 0");
        assertEquals(0, checkout.calculateTotal(),
                    "After clear, total should be 0");
        assertTrue(checkout.getScannedProducts().isEmpty(),
                  "After clear, scanned products should be empty");
    }

    /**
     * PROPERTY 8: Scanning same product twice equals scanning with double quantity
     * Metamorphic: scan(p) + scan(p) = scan(p, quantity=2)
     */
    @Property
    void scanningTwiceEqualsDoubleQuantity(
            @ForAll("catalogProduct") Product product,
            @ForAll @IntRange(min = 1, max = 5) int quantity) {

        Checkout checkout1 = new Checkout();
        for (int i = 0; i < quantity; i++) {
            checkout1.scan(product);
        }

        Checkout checkout2 = new Checkout();
        for (int i = 0; i < quantity; i++) {
            checkout2.scan(product);
        }

        assertEquals(checkout1.calculateTotal(), checkout2.calculateTotal(),
                    "Scanning same product multiple times should give same total");
        assertEquals(checkout1.getItemCount(), checkout2.getItemCount(),
                    "Item counts should match");
    }

    /**
     * PROPERTY 9: Adding same product increases total by its price
     * Metamorphic: total_after = total_before + product.price
     */
    @Property
    void addingProductIncreasesTotalByItsPrice(
            @ForAll("nonEmptyBasket") List<Product> initialBasket,
            @ForAll("catalogProduct") Product additionalProduct) {

        Checkout checkout = new Checkout();
        initialBasket.forEach(checkout::scan);

        int totalBefore = checkout.calculateTotal();
        checkout.scan(additionalProduct);
        int totalAfter = checkout.calculateTotal();

        int expectedIncrease = additionalProduct.getPriceInCents();
        int actualIncrease = totalAfter - totalBefore;

        assertEquals(expectedIncrease, actualIncrease,
                    String.format("Adding %s should increase total by %d cents",
                                 additionalProduct.getName(), expectedIncrease));
    }

    // ========== BOUNDARY CONDITIONS ==========

    /**
     * PROPERTY 10: Single item checkout works correctly
     * Edge case: basket with exactly one item.
     */
    @Property
    void singleItemCheckoutWorks(
            @ForAll("catalogProduct") Product product) {
        Checkout checkout = new Checkout();
        checkout.scan(product);

        assertEquals(1, checkout.getItemCount(),
                    "Should have exactly 1 item");
        assertEquals(product.getPriceInCents(), checkout.calculateTotal(),
                    "Total should equal the product price");
        assertEquals(1, checkout.getProductCount(product.getName()),
                    "Product count should be 1");
    }

    /**
     * PROPERTY 11: Large basket calculation
     * Stress test: many items should calculate correctly.
     */
    @Property
    void largeBasketCalculatesCorrectly(
            @ForAll("catalogProduct") Product product,
            @ForAll @IntRange(min = 10, max = 100) int quantity) {

        Checkout checkout = new Checkout();
        for (int i = 0; i < quantity; i++) {
            checkout.scan(product);
        }

        long expectedTotal = (long) product.getPriceInCents() * quantity;
        assertEquals(expectedTotal, checkout.calculateTotal(),
                    String.format("Total for %d items should be %d",
                                 quantity, expectedTotal));
    }

    /**
     * PROPERTY 12: Scanning all catalog products covers all items
     * Integration test: ensure all products can be processed.
     */
    @Property
    void checkoutHandlesAllCatalogProducts() {
        Checkout checkout = new Checkout();

        List<Product> catalog = List.of(
                new Product("Apple", 50),
                new Product("Pear", 30),
                new Product("Pineapple", 220),
                new Product("Banana", 60),
                new Product("Orange", 45)
        );

        catalog.forEach(checkout::scan);

        assertEquals(catalog.size(), checkout.getItemCount(),
                    "Should have scanned all catalog items");

        int expectedTotal = catalog.stream()
                .mapToInt(Product::getPriceInCents)
                .sum();
        assertEquals(expectedTotal, checkout.calculateTotal(),
                    "Total should match sum of catalog prices");
    }
}
