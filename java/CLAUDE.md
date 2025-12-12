# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is the Java implementation of the **Kata Supermarket** coding exercise, inspired by the [Supermarket Kata](http://codekata.com/kata/kata01-supermarket-pricing/). The kata focuses on building a supermarket checkout system with pricing rules, offers, and discounts.

The parent repository contains implementations in multiple languages (Java, C#, TypeScript) and includes two approaches to the same problem:
- **EBT (Example-Based Testing)**: Requirements defined through specific examples (see `../README-EBT.md`)
- **PBT (Property-Based Testing)**: Requirements defined as general rules and properties (see `../README-PBT.md`)

## Build Commands

This project uses Maven with Java 8.

```bash
# Compile the project
mvn compile

# Run all tests
mvn test

# Run a specific test class
mvn test -Dtest=Setup

# Run a specific test method
mvn test -Dtest=Setup#isGood

# Clean and rebuild
mvn clean install

# Package the project
mvn package
```

## Project Structure

```
java/
├── pom.xml                    # Maven configuration (Java 8, JUnit 5.5.0)
└── src/
    └── test/
        └── java/              # Test files
```

Note: The `src/main/java` directory does not exist yet. Create it when implementing production code.

## Kata Progression

The kata follows an incremental approach through user stories. Each story builds on the previous one:

### Phase 1: Start Small
1. **Single product checkout**: Handle one type of product with simple price calculation
2. **Multiple products**: Extend to handle different fruit types (apples, pears, pineapples, bananas)
3. **Quantity-based offers**: Implement "N for X" offers (e.g., 3 apples for 130 cents instead of 150)
4. **"Take N pay M" offers**: Implement "buy 3 pay for 2" style offers (e.g., for oranges)
5. **Combo offers**: Implement bundle deals (e.g., 4 apples + 2 pears + 2 bananas + 1 pineapple for 500 cents)

### Phase 2: Go Big (Extended Features)
- Display integration (printing to console)
- Payment handling (cash with change calculation)
- Fiscal receipt printing
- Fidelity card system (with additional discounts)
- Credit card payment integration (external service)
- Product refunds/cancellations
- Rounding rules (multiples of 5 for eliminated coin denominations)

## Domain Concepts

### Pricing Model
- All prices are in **cents** (integers)
- Items have both **unit prices** and optional **special prices**
- Current standard prices:
  - Apple: 50 cents (special: 3 for 130)
  - Pear: 30 cents (special: 2 for 45)
  - Pineapple: 220 cents (no special)
  - Banana: 60 cents (no special)
  - Orange: 45 cents (special: take 3 pay 2)

### Key Requirements
- **Order independence**: Items can be scanned in any order, but offers still apply correctly
- **Configurable pricing**: Pricing rules should be provided at checkout initialization, not hardcoded
- **Pluggable devices**: Display and printer are external devices (simulate with console output)
- **External services**: Payment and fidelity card services are external (use interfaces/mocks)

## Testing Approach

This Java implementation can follow either:
- **EBT approach**: Write specific example-based tests following acceptance criteria in README-EBT.md
- **PBT approach**: Write property-based tests defining general rules from README-PBT.md

Both README files contain the same functionality requirements, just expressed differently.

## Architectural Considerations

Based on the kata requirements, consider these design patterns:

1. **Strategy Pattern**: For different offer types (quantity discount, take N pay M, combo)
2. **Pricing Rules Configuration**: Inject pricing rules at checkout creation time
3. **External Device Abstraction**: Use interfaces for Display, Printer, PaymentService, FidelityCardService
4. **Order of operations**: Offers → Fidelity discounts (stacking in that order)
5. **Immutability**: Consider immutable product/pricing objects for thread safety

When implementing credit card payments, use the IPaymentService interface pattern shown in the README (adapted to Java):
- `connect(username, password)` → returns status code
- `acceptPayment(creditCardNumber, totalToPay)` → returns status code
- Use mock implementations for testing
