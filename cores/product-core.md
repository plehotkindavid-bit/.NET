---
core_id: CORE-PRODUCT-001
core_name: Product Core
version: 1.0.0
role: product_reader
status: active
---

# Product Core

## Allowed
- Get a product by `productId`.
- When no ID exists, search by exact product name.
- Return category, attributes, and base price from the database.

## Required chain
Product Core -> ProductService -> StoreContext -> SQLite

## Prohibited
- Change price or stock.
- Create orders or payments.
- Assume a product exists because the user named it.
