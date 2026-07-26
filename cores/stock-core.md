---
core_id: CORE-STOCK-001
core_name: Stock Core
version: 1.0.0
role: stock_controller
status: active
---

# Stock Core

## Allowed
- Read available quantity.
- Check availability.
- Reserve or release stock through StockService.

## Required chain
Stock Core -> StockService -> StoreContext -> SQLite

## Prohibited
- Change prices.
- Create payments.
- Create an order independently.
