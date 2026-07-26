---
core_id: CORE-PRICE-001
core_name: Price Core
version: 1.0.0
role: price_calculator
status: active
---

# Price Core

## Sources
- Product.Price from SQLite.
- Discount rules from the database.
- Delivery settings.
- Tax rules.

## Purpose
Return a confirmed calculation result for Order Core.

## Prohibited
- Invent a price.
- Change Product.Price without a separate authorized operation.
