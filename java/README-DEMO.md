# Property-Based Testing Demo with jqwik

This demo showcases Property-Based Testing (PBT) concepts using the **jqwik** library for Java, applied to the KataSupermarket domain.

## What is Property-Based Testing?

Property-Based Testing is a testing approach where you:
1. Define **properties** (invariants) that should always hold true
2. Generate **hundreds of random test cases** automatically
3. Let the framework find edge cases you didn't think of

Unlike Example-Based Testing (EBT) where you write specific test cases, PBT tests general rules.

## Demo Structure

This demo is organized in **three progressive levels**:

### 1. Basic Integer Properties (`BasicPropertyTests.java`)

**Purpose**: Introduce PBT concepts with simple numeric properties.

**Properties demonstrated**:
- ✅ Prices are always non-negative
- ✅ Total is independent of scanning order (commutativity)
- ✅ Total equals sum of individual prices
- ✅ Scanning N items costs N × unit price
- ✅ Adding zero-price items doesn't change total
- ✅ Item count equals number of scans

**Key concepts**:
- `@Property` annotation instead of `@Test`
- `@ForAll` for generating test data
- `@IntRange` for constraining integer values
- jqwik runs each property 1000 times by default with different values

**Example**:
```java
@Property
void totalEqualsSumOfPrices(
        @ForAll @IntRange(min = 1, max = 200) int applePrice,
        @ForAll @IntRange(min = 1, max = 100) int pearPrice) {

    Checkout checkout = new Checkout();
    checkout.scan(new Product("Apple", applePrice));
    checkout.scan(new Product("Pear", pearPrice));

    int expectedTotal = applePrice + pearPrice;
    assertEquals(expectedTotal, checkout.calculateTotal());
}
```

### 2. Custom Arbitraries (`ProductArbitraryTests.java`)

**Purpose**: Show how to generate domain-specific test data.

**Arbitraries created**:
1. **`anyProduct()`**: Random strings + random prices
2. **`supermarketProduct()`**: Realistic fruit names + realistic price ranges
3. **`catalogProduct()`**: Specific products from the kata (Apple, Pear, etc.)
4. **`expensiveProduct()`**: Filtered products (price ≥ 200)
5. **`shoppingBasket()`**: Lists of products (1-10 items)

**Key concepts**:
- `@Provide` methods return `Arbitrary<T>`
- `Combinators.combine()` for creating objects from multiple arbitraries
- `Arbitraries.of()` for choosing from a fixed set
- `.filter()` for constraining values
- `.list()` for generating collections

**Example**:
```java
@Provide
Arbitrary<Product> supermarketProduct() {
    List<String> fruitNames = Arrays.asList(
        "Apple", "Pear", "Pineapple", "Banana"
    );

    Arbitrary<String> names = Arbitraries.of(fruitNames);
    Arbitrary<Integer> prices = Arbitraries.integers().between(20, 500);

    return Combinators.combine(names, prices).as(Product::new);
}

@Property
void checkoutHandlesAnyBasket(
        @ForAll("shoppingBasket") List<Product> basket) {
    Checkout checkout = new Checkout();
    basket.forEach(checkout::scan);

    assertEquals(basket.size(), checkout.getItemCount());
    assertTrue(checkout.calculateTotal() >= 0);
}
```

### 3. Advanced Checkout Properties (`CheckoutPropertyTests.java`)

**Purpose**: Demonstrate sophisticated property testing patterns.

**Categories of properties**:

#### Algebraic Properties
- Associativity: `(a + b) + c = a + (b + c)`
- Identity: empty checkout has zero total
- Commutativity: **order independence** (KEY kata requirement!)

#### Invariant Properties
- Total is never negative
- Item count matches scans
- Product counts are consistent

#### Metamorphic Properties
- Clearing resets to empty state
- Scanning twice equals double quantity
- Adding product increases total by its price

#### Boundary Conditions
- Single item checkout
- Large basket calculation (10-100 items)
- All catalog products

**Example**:
```java
@Property
void orderOfScanningDoesNotMatterForTotal(
        @ForAll("nonEmptyBasket") List<Product> basket) {

    // Original order
    Checkout checkout1 = new Checkout();
    basket.forEach(checkout1::scan);
    int total1 = checkout1.calculateTotal();

    // Shuffled order
    List<Product> shuffled = new ArrayList<>(basket);
    Collections.shuffle(shuffled);

    Checkout checkout2 = new Checkout();
    shuffled.forEach(checkout2::scan);
    int total2 = checkout2.calculateTotal();

    assertEquals(total1, total2,
        "Total should be independent of scanning order");
}
```

## Running the Tests

### Using Maven

```bash
# Run all tests
mvn test

# Run a specific test class
mvn test -Dtest=BasicPropertyTests

# Run a specific property
mvn test -Dtest=BasicPropertyTests#totalEqualsSumOfPrices
```

### Using IDE

1. Open the project in IntelliJ IDEA or Eclipse
2. Navigate to any test class
3. Right-click and select "Run Tests"
4. Watch jqwik generate hundreds of test cases!

## Understanding jqwik Output

When you run a property test, jqwik will:
1. Generate random inputs (default: 1000 tries)
2. Run your property with each input
3. Report statistics:
   ```
   BasicPropertyTests:totalEqualsSumOfPrices =
     checks = 1000
     generation-mode = RANDOMIZED
     seed = 1234567890
   ```

If a property fails, jqwik will:
1. Show the failing example
2. **Shrink** the input to find the minimal failing case
3. Report the seed for reproducibility

## Key PBT Patterns Demonstrated

### 1. Use Built-in Arbitraries First
```java
@ForAll @IntRange(min = 1, max = 500) int price
```

### 2. Build Custom Arbitraries for Domain Objects
```java
@Provide
Arbitrary<Product> catalogProduct() { ... }
```

### 3. Combine Arbitraries
```java
Combinators.combine(names, prices).as(Product::new)
```

### 4. Generate Collections
```java
catalogProduct().list().ofMinSize(1).ofMaxSize(10)
```

### 5. Filter/Constrain Values
```java
supermarketProduct().filter(p -> p.getPriceInCents() >= 200)
```

## Benefits of PBT

1. **More Coverage**: 1000 test cases vs. 5-10 example tests
2. **Find Edge Cases**: Discovers scenarios you didn't think of
3. **Specification as Code**: Properties document system behavior
4. **Refactoring Confidence**: Properties ensure behavior is preserved
5. **Less Test Maintenance**: Change data, properties stay the same

## Comparison: EBT vs PBT

**Example-Based Test (EBT)**:
```java
@Test
void scanningThreeApplesCharges130() {
    Checkout checkout = new Checkout();
    checkout.scan(new Product("Apple", 50));
    checkout.scan(new Product("Apple", 50));
    checkout.scan(new Product("Apple", 50));
    assertEquals(150, checkout.calculateTotal());
}
```

**Property-Based Test (PBT)**:
```java
@Property
void scanningNItemsCostsNTimesUnitPrice(
        @ForAll @IntRange(min = 1, max = 100) int unitPrice,
        @ForAll @IntRange(min = 1, max = 10) int quantity) {

    Checkout checkout = new Checkout();
    Product product = new Product("Apple", unitPrice);

    for (int i = 0; i < quantity; i++) {
        checkout.scan(product);
    }

    assertEquals(unitPrice * quantity, checkout.calculateTotal());
}
```

The PBT version tests the **general rule** with thousands of different prices and quantities!

## Next Steps

To extend this demo:

1. **Add Offers**: Implement discount rules (3 for 130, take 3 pay 2)
2. **Test Offer Properties**:
   - Offers reduce or maintain price (never increase)
   - Multiple offers can be applied
   - Order independence still holds
3. **Add Combo Offers**: Fruit salad bundles
4. **Test with jqwik Statistics**: Use `Statistics.collect()` to see data distribution
5. **Try Stateful Testing**: Use `@Property` with `ActionSequence<T>`

## Resources

- [jqwik Documentation](https://jqwik.net/docs/current/user-guide.html)
- [Property-Based Testing Patterns](https://fsharpforfunandprofit.com/posts/property-based-testing-2/)
- [KataSupermarket README-PBT](./README-PBT.md) - Full kata requirements

## Project Structure

```
java/
├── pom.xml                                    # Maven config with jqwik
├── CLAUDE.md                                  # Project guidance
├── README-DEMO.md                             # This file
└── src/
    ├── main/
    │   └── java/com/jesuswasrasta/supermarket/
    │       ├── Product.java                   # Domain model
    │       └── Checkout.java                  # Checkout system
    └── test/
        └── java/com/jesuswasrasta/supermarket/
            ├── BasicPropertyTests.java        # Level 1: Integer properties
            ├── ProductArbitraryTests.java     # Level 2: Custom Arbitraries
            └── CheckoutPropertyTests.java     # Level 3: Advanced properties
```

## Summary

This demo progressively introduces Property-Based Testing:

1. **Start Simple**: Integer-based properties with built-in generators
2. **Go Domain-Specific**: Custom Arbitraries for Product and baskets
3. **Test Complex Behavior**: Algebraic, invariant, and metamorphic properties

Each test file is self-contained and demonstrates specific PBT concepts. Start with `BasicPropertyTests.java` and work your way up!

Happy Property Testing! 🎯
