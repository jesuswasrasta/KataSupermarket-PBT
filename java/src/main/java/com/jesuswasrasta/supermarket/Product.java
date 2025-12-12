package com.jesuswasrasta.supermarket;

import java.util.Objects;

/**
 * Represents a product in the supermarket.
 * Prices are expressed in cents (integers).
 */
public class Product {
    private final String name;
    private final int priceInCents;

    public Product(String name, int priceInCents) {
        if (priceInCents < 0) {
            throw new IllegalArgumentException("Price cannot be negative");
        }
        if (name == null || name.trim().isEmpty()) {
            throw new IllegalArgumentException("Product name cannot be empty");
        }
        this.name = name;
        this.priceInCents = priceInCents;
    }

    public String getName() {
        return name;
    }

    public int getPriceInCents() {
        return priceInCents;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Product product = (Product) o;
        return priceInCents == product.priceInCents &&
               Objects.equals(name, product.name);
    }

    @Override
    public int hashCode() {
        return Objects.hash(name, priceInCents);
    }

    @Override
    public String toString() {
        return "Product{" +
                "name='" + name + '\'' +
                ", priceInCents=" + priceInCents +
                '}';
    }
}
