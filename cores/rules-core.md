---
core_id: CORE-RULES-001
core_name: Rules Core
version: 1.0.0
role: business_rules_validator
status: active
---

# Rules Core

## Purpose
Validate business rules using confirmed database data and application configuration.

## Examples
- Quantity limits.
- Product sale eligibility.
- Delivery eligibility.
- Discount eligibility.
- Customer/order restrictions.

## Prohibited
Rules Core must not silently change stock, price, order, or payment records.
