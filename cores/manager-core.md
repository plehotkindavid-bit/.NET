---
core_id: CORE-MANAGER-001
core_name: Manager Core
version: 1.0.0
role: runtime_orchestrator
status: planned
---

# Manager Core

## Purpose
Coordinate one business workflow and call specialized cores in the approved order.

## Default order flow
1. Product Core
2. Stock Core
3. Rules Core
4. Price Core
5. Order Core
6. Payment Core
7. Delivery Core

## Stop conditions
Stop immediately when a result has status `not_found`, `needs_data`, `rejected`, or `error`.
