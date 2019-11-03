# Coding assignment. Week 9 (2019).

[![Join the chat at https://gitter.im/kmaooad/coding-19W09](https://badges.gitter.im/kmaooad/coding-19W09.svg)](https://gitter.im/kmaooad/coding-19W09?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
<NEVER BUILT>

### Task

**Implement orders processing**

### API
`Client.fs` contains API that you need to implement in this task:
- `addProduct name price taxable: int` Creates new product with given name, price, and tax flag (some products require adding tax %, some don't). Product storage can be implemented as you wish - file, in-memory etc. whichever is the simplest one for you.

- `updateProduct (p: Product) : unit` Updates given product

- `newOrder (c: Customer) (pp: OrderedProductDto list): int` Starts a new order in Draft status, provided customer and ordered products & quantities 

- `getTotal orderId: OrderTotalsDto` Returns order totals given order ID

- `updateOrder orderId (pp: OrderedProductDto list) : unit` Updates order with provided products & quantities (replaces previous order items). Only Draft order can updated.

- `getStatus orderId : OrderStatus` Returns order status

- `submitOrder orderId : unit` Moves Draft order to Submitted status

- `payOrder orderId : unit` Moves Submitted order to Paid status

### Order calculation

Main functional task in this assignment is calculation of order total amount (how much customer should pay). _Maintaining accurate order total is very important in every order processing application, both in actual ordering moment and later on for reporting and tracking._ Thus, pay special attention to designing calculation logic in the most secure way (in design sense).

Result of order calculation is represented with `OrderTotalsDto` structure with subtotal (actually how much ordered products cost), total tax for ordered products (can be 0), and overall total, which should be subtotal + tax (that's the `total` field in structure).

#### Tax calculation

Certain products are taxed (`taxable` field in `Product`) and require adding tax amount to the order.
Tax rate is not constant and depends on location where customer resides (defined by zip code). You can get tax rate value by zip code from [Fancy Tax API stub](https://my-json-server.typicode.com/kmaooad/fancy-taxing/rates) e.g. [https://my-json-server.typicode.com/kmaooad/fancy-taxing/rates?zip=10001](https://my-json-server.typicode.com/kmaooad/fancy-taxing/rates?zip=10001) _Note: This API has only several zip codes as sample; to keep it simple, if zip code is not present use default rate of 20% (0.2)._

#### Updating product price

Product price can change over time. Changed product price should be reflected in Draft orders, thus is product price changes, Draft order total should change too. Submitted and Paid orders don't change even if product price changes.

