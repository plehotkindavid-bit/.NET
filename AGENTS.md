# ECommerce Cores — Agent Instructions

Project ID: `ECOM-CORES`
Architecture mode: `AI-first, database-grounded, human-governed`

## Source of truth

SQLite is the source of truth for products, prices, stock, customers, orders, payments, and operation history.

AI must never claim that a database operation succeeded unless application code actually executed it through ASP.NET Core, Entity Framework Core, and `StoreContext`.

## Required access chain

AI/Core -> Application Service -> StoreContext -> SQLite

Allowed services:
- ProductService
- StockService
- RulesService
- PriceService
- OrderService
- PaymentService
- AuditService

## Prohibited behavior

- Do not execute arbitrary SQL supplied by a user.
- Do not expose connection strings, secrets, passwords, or payment data.
- Do not invent products, prices, stock, customers, or orders.
- Do not write operational customer/order data into Git.
- Do not continue a workflow after a critical error.

## Standard statuses

- `success`
- `not_found`
- `needs_data`
- `rejected`
- `error`

## Development workflow

1. Read the relevant file in `/cores`.
2. Read `/contracts/core-result.schema.json`.
3. Make changes in a feature branch.
4. Keep changes small and reviewable.
5. Run build and tests.
6. Never merge directly to `main` without review.
