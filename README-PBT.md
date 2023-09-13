# Kata Supermarket

Inspired from [Supermarket Kata](http://codekata.com/kata/kata01-supermarket-pricing/).

## Table of Contents
- [Kata Supermarket](#kata-supermarket)
  - [Table of Contents](#table-of-contents)
  - [The kata](#the-kata)
  - [Start small](#start-small)
    - [The 1st User Story](#the-1st-user-story)
      - [Acceptance Criteria](#acceptance-criteria)
    - [The 2nd User Story](#the-2nd-user-story)
      - [Acceptance Criteria](#acceptance-criteria-1)
    - [The 3rd User Story](#the-3rd-user-story)
      - [Acceptance Criteria](#acceptance-criteria-2)
    - [The 4th User Story](#the-4th-user-story)
      - [Acceptance Criteria](#acceptance-criteria-3)
    - [The 5th User Story: Fruit Salad offer](#the-5th-user-story-fruit-salad-offer)
      - [Acceptance Criteria](#acceptance-criteria-4)
  - [Go big](#go-big)
    - [User Story: Payments, show total amount on display](#user-story-payments-show-total-amount-on-display)
      - [Acceptance Criteria](#acceptance-criteria-5)
      - [Implementation notes](#implementation-notes)
    - [User Story: Payments, show change on display](#user-story-payments-show-change-on-display)
      - [Acceptance Criteria](#acceptance-criteria-6)
    - [User Story: Fiscal Receipt](#user-story-fiscal-receipt)
      - [Acceptance Criteria](#acceptance-criteria-7)
      - [Implementation notes](#implementation-notes-1)
    - [User Story: Fidelity Cards](#user-story-fidelity-cards)
      - [Acceptance Criteria](#acceptance-criteria-8)
      - [Implementation notes](#implementation-notes-2)
    - [User Story: Payment by Credit Card](#user-story-payment-by-credit-card)
      - [Acceptance Criteria](#acceptance-criteria-9)
      - [Implementation notes](#implementation-notes-3)
    - [Add some more extra features](#add-some-more-extra-features)


## The kata

In Jet Supermarket we have a checkout system that can do only one kind of offer,
based on quantity of the same item: when products are purchased in larger quantities, they are available at a more favorable discounted price.
At the moment items are priced individually in cents
while some items are multi-priced: Purchase a specific quantity, and the cost per item decreases.

The current prices can be defined at configuration time.

Our checkout system accepts items in any order, so shuffling the item in a purchase 
will not change the grand total.
Because the pricing changes frequently, we want to provide a set of pricing rules 
each time we start handling a checkout transaction.

## Start small

Start making the system able to check an individual price: for now, the cashier will calculate discounts by hand.  
Then start implementing offers.  

### The 1<sup>st</sup> User Story

Let's start with one single kind of product.

```markdown
As a cashier, 
I wish my customers can pay for one kind of product
so that the grand total, based on the product price, is displayed
```

#### Acceptance Criteria

```markdown
* When I checkout a product, the system charges its price
* When I checkout `n` items of a product, the system charges `n` times its price
* When I checkout more than 1 kind of product, the system raises an error
```

Note: ideally, we could reduce this 1<sup>st</sup> User Story in 2 smaller ones:

- A checkout system only able to display the product price, and still not capable of calculating the grand total (e.g., scanning twice the same product would not change the displayed value; the cashier would use the bare basic checkout system as a price reference, not yet as a calculator)
- A checkout system capable of calculating the grand total
---

### The 2<sup>nd</sup> User Story

Implement individual price for multiple kinds of product.
No offers implemented, yet.  

```markdown
As a cashier, 
I want to scan products of different kinds and get the grand total
so I can speed up the payment process
```

#### Acceptance Criteria

```markdown
* When I checkout `1` product, the system charges its price
* When I checkout `N` items of the same product, the system charges the grand total calculated as `N` times the product price

* When I checkout more than 1 kind of product, the system calculates the grand total as the sum of the totals of each product type
```

---

### The 3<sup>rd</sup> User Story

Time to implement offers!

First `N` are discounted:

```markdown
As a cashier, 
I want to specify offers for items
so my customers enjoy savings when they surpass a certain quantity threshold in their purchases.
```

#### Acceptance Criteria

```markdown
* When I checkout products for which no offer is defined, the standard grand total is calculated based on the price P
* When an offer defined as `N products of type T is sold to the discounted cost D instead of the standard price P`
  * purchasing less than `N` products of type `T` the standard grand total is calculated based on `P`
  * purchasing exactly `N` products of type `T`, the total for that product is `N` times the discounted price `D`
  * purchasing more than `N` products of type `T`, the discount is applied only to the first `N`; the rest are priced `D` as usual
```

### The 4<sup>th</sup> User Story
Take `N` pay `D` offer.

```markdown
As a cashier,
I want to add a "take `N` pay `D` offer" kind of offer for items
so my customers will pay less for multiple items purchase
```


#### Acceptance Criteria

```markdown
As before, plus:

* each group of `N` Products qualifies for a discounted price `D`. Any Products beyond the multiples of `N` are priced at the ordinary rate `P`.
```

---
### The 5<sup>th</sup> User Story: Fruit Salad offer
Combo offers.  

```markdown
As a cashier, 
I want to create a customized offer where customers must purchase a specific quantity of each product type to qualify for a discounted price
so that my customers are encouraged to purchase more products.
```


#### Acceptance Criteria

* If the offer is defined as:

```markdown
N1 of Product1, N2 of Product2, N3 of Product3, ..., cost D
```

then each group of `N1 of Product1, N2 of Product2, N3 of Product3, ...` results in the system charging the specified cost `D`.

## Go big

Feel free to go on adding more complex features.  
Here some inspiration :)  

---

### User Story: Payments, show total amount on display
Let's add some payments features.  

```markdown
As a cashier, 
I want the cash register to show the total amount on the display while checking-out,  
so I can tell my customers how much cash they have to pay 
```

#### Acceptance Criteria

```markdown
* E.g: If customer buys 1 apple (50 cents), on checkout the cash register will tell on the display 
    |   Total: 50  |
```

#### Implementation notes
In this scenario, we have just discovered there is a display attached to the cash register.  
The display is a pluggable external device.  
For the sake of simplicity, in this exercise the display prints its messages to the console.  

---

### User Story: Payments, show change on display

```markdown
As a cashier, 
I want the cash register to calculate the change  
so I can avoid silly mistakes with my customers during the payments 
```

#### Acceptance Criteria

```markdown
* I want to tell the cash register the amount of cash my customers gives to me, so it can calculate the change.
  * E.g: If customer buys 1 apple (50 cents) and gives me a 1$ dollar note, the cash register will tell **on the display** 
    |   Total: 50  |
    |    Cash: 100 |
    |  Change: 50  |
```

---

### User Story: Fiscal Receipt

Time to print some fiscal receipt.  

```markdown
As a cashier, 
I want to print the fiscal receipt for sold items
so I can collect money from my customers 
```

#### Acceptance Criteria

```markdown
Scenario 1: customer buys items with and without applied offers

Background: 
* I checked out 3 apples for 130 cents instead of 150
* I checked out 2 pears for 45 cents instead of 60
* I checked out 2 pineapples for 440 cents (no offers for pineapples)

* When I close the checkout, a fiscal receipt like this is printed to the console:

|Item       | Price            |
|-----------|------------------|
| apple     | 50x3=~~150~~ 130 |
| pear      | 30x2=~~60~~ 45   |
| pineapple | 220x2=440        |
|           |                  |
| TOTAL     | **615**          |
|           |                  |
| Goodbye!  |                  |
```

```markdown
Scenario 2: an item is damaged then subtracted form the list

Background: 
* I checked out 3 apples for 130 cents instead of 150
* I checked out 2 pears for 45 cents instead of 60
* I checked out 2 pineapples for 440 cents (no offers for pineapples)
* I refund 1 pear

* When I close the checkout, a fiscal receipt like this is printed to the console:

| Item      | Price            |
|-----------|------------------|
| apple     | 50x3=~~150~~ 130 |
| pear      | 30x2=~~60~~ 45   |
| pineapple | 220x2=440        |
| ~~pear~~  | ~~30x2=60 45~~   |
| pear      | 30x1=30          |
|           |                  |
| TOTAL     | **600**          |
|           |                  |
| Goodbye!  |                  |
```

```markdown
Scenario 3: automatic calculation of change

Background: 
* I checked out 1 pear for 30 cents

* I close the checkout
* I tell to the customer the total amount
* She gives me a 1 dollar bill
* I enter the received amount in the cash register

* I close the transaction, then a fiscal receipt like this is printed to the console:

| Item      | Price           |
|-----------|-----------------|
| pear      | 30              |
|           |                 |
| TOTAL     | 30              |
| --------- | --------------- |
| Received  | 100             |
| Change    | 70              |
|           |                 |
| Goodbye!  |                 |
```


#### Implementation notes

In this scenario, we have just discovered there is a printer attached to the cash register.  
The printer is a pluggable external device.  
For the sake of simplicity, in this exercise the printer prints its messages to the console.  

---
### User Story: Fidelity Cards

What if the customers has a fidelity card?
Feel free to add implementation details at runtime (e.g., having a 3rd party serve to check cards validity, 
check the accumulated balance on the card, etc.)

```markdown
As a cashier, 
I want to give a loyalty card that applies an additional 10% discount to some "fidelity products"
so I can build up the loyalty of my customers
```

#### Acceptance Criteria

```markdown
Scenario 1: Customer with fidelity card

Background: 
* Apple is a "fidelity product"
* Customer has fidelity card number 123456 (additional 10% discount)
* There's an offer for apples: 3 apples for 130 cents instead of 150
* There's an offer for pears: 2 pears for 45 cents instead of 60

* I checked out 3 apples
* I checked out 2 pears
* I checked out 2 pineapple

* When I close the checkout, a fiscal receipt like this is printed to the console:

|Item       | Price                                                           |
|-----------|-----------------------------------------------------------------|
| apple     | Base price: 50x3=~~150~~; Offer: ~~130~~; Fidelity: 130-10%=117 |
| pear      | Base price: 30x2=~~60~~; Offer: 45                              |
| pineapple | Base price: 220x2=440                                           |
|           |                                                                 |
| TOTAL     | **602**                                                         |
|           |                                                                 |
|           | Fidelity card: 123456                                           |
|           |                                                                 |
| Goodbye!  |                                                                 |
```

#### Implementation notes

Treat the Fidelity Card service as an external service.  
For simplicity, create a mock object implementing the behaviors you decide.   

---
### User Story: Payment by Credit Card

```markdown
As a cashier, 
I want to let my customer pay by credit card 
```

#### Acceptance Criteria

```markdown
* E.g: If customer buys 1 apple (50 cents), on checkout with a valid credit card, the cash register will show on the display: 
    |   0 - Transaction completed successfully |
```

#### Implementation notes
In this scenario, we have an external service that allows the supermarket to pay by credit card.  
This is an external service, we don't own it.  
Fortunately, there is an interface that allows us to know how to call it:

```csharp
namespace KataSupermarket;
/// <summary 
/// Credit Card Payment Service
/// </summary>
public interface IPaymentService
{
   /// <summary>   
   /// A cash register must connect to the service before accepting the payment.
   /// </summary>   
   /// <param name="username">The username assigned to the supermarket</param>    
   /// <param name="password">The password assigned to the supermarket</param>    
   /// <returns>Result</returns> 
   /// <example>0 - Connection completed successfully.</example>    
   /// <example>10 - Invalid username or password.</example>    
   /// <example>90 - Connection failed. Please try again later.</example>    
   Result Connect(string username, string password);
   /// <summary>     
   /// This method allows the supermarket to charge the <param name="creditCardNumber"></param> with the specified <param name="totalToPay"></param>.
   /// </summary>    
   /// <param name="creditCardNumber"></param>    
   /// <param name="totalToPay"></param>    
   /// <returns>Result</returns>    
   /// <example>0 - Transaction completed successfully.</example>    
   /// <example>1 - User not connected.</example>    
   /// <example>10 - Insufficient funds.</example>    
   /// <example>20 - Expired credit card.</example>    
   /// <example>30 - Lost or stolen credit card.</example>    
   /// <example>40 - Unusual activity.</example>    
   /// <example>90 - Connection failed. Please try again later.
   ///</example>    
   Result AcceptPayment(string creditCardNumber, int totalToPay);
}

/// <summary>
/// The result of an interaction with the Payment Service 
/// </summary>
public class Result
{
   /// <summary>    
   /// A code that identifies the interaction result.
   /// </summary>    
   public int StatusCode { get; }
   /// <summary>      A human readable message.
   /// </summary>    
   public string Message { get; }
    
   public Result(int statusCode, string message)
   {
        StatusCode = statusCode;
        Message = message;
   }
}
```

---

### Add some more extra features

Here some other hints for small variations to the exercise 😃  

Now, the supermarket manager thinks that having more items and offers will boost his earnings; so he wants to:

* add these items:
  * mango -> 155 cents
  * coconut -> 233 cents
  * strawberry -> 120 cents
  * orange -> 45 cents
  * peach -> 40 cents

* implement new offers that lets us apply new pricing rules:
  * combo offers (e.g., buy 3 apple, and you will obtain an additional pear)
  * fidelity card offers
  * limited quantity offers (e.g., "you can buy this item at discount price, but max 3 pieces")

* we have to manage the cancellation of a product, because it is damaged or the customer decides to return it (e.g., it costs too much and he didn't realize it)

* the government stopped producing 1 and 2 cents coins:
  * every bill/invoice has to be rounded to the nearest lower multiple of 5, even if paid with debit/credit card

...[to be continued]...

PS: feel free to propose next change request to implement! 😀
