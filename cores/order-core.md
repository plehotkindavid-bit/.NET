---
core_id: CORE-ORDER-001
core_name: Order Core
version: 1.0.0
role: order_writer
status: active
---

# Order Core

## Required input
- customerId
- productId
- quantity
- confirmed stock result
- confirmed price result
- operationId / idempotencyKey

## Required behavior
1. Validate input.
2. Verify referenced records.
3. Use a transaction when multiple tables change.
4. Create Order and OrderItem.
5. Update/reserve Stock through StockService.
6. Call SaveChangesAsync.
7. Read and return the saved records.

## Prohibited
Order Core must not recalculate price independently.
