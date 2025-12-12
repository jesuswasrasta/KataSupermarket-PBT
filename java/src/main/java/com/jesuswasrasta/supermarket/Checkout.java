package com.jesuswasrasta.supermarket;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Represents a supermarket checkout system.
 * Handles scanning products and calculating totals.
 */
public class Checkout {
    private final List<Product> scannedProducts;
    private final Map<String, Integer> productCounts;

    public Checkout() {
        this.scannedProducts = new ArrayList<>();
        this.productCounts = new HashMap<>();
    }

    /**
     * Scan a product at the checkout.
     * @param product the product to scan
     */
    public void scan(Product product) {
        if (product == null) {
            throw new IllegalArgumentException("Cannot scan null product");
        }
        scannedProducts.add(product);
        productCounts.put(product.getName(),
                         productCounts.getOrDefault(product.getName(), 0) + 1);
    }

    /**
     * Calculate the total price of all scanned products.
     * @return the total in cents
     */
    public int calculateTotal() {
        return scannedProducts.stream()
                .mapToInt(Product::getPriceInCents)
                .sum();
    }

    /**
     * Get the count of a specific product.
     * @param productName the name of the product
     * @return the count of that product
     */
    public int getProductCount(String productName) {
        return productCounts.getOrDefault(productName, 0);
    }

    /**
     * Get all scanned products.
     * @return a copy of the scanned products list
     */
    public List<Product> getScannedProducts() {
        return new ArrayList<>(scannedProducts);
    }

    /**
     * Get the number of items scanned.
     * @return the total number of scanned items
     */
    public int getItemCount() {
        return scannedProducts.size();
    }

    /**
     * Clear all scanned products.
     */
    public void clear() {
        scannedProducts.clear();
        productCounts.clear();
    }
}
