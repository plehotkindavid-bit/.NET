# Git Workflow for ECommerce Cores

## What Git stores

- ASP.NET Core and React source code.
- Core instructions.
- Contracts and schemas.
- Architecture decisions.
- Tests and CI configuration.

## What Git must not store

- Real customers.
- Real orders.
- Payment data.
- SQLite production database files.
- Secrets and connection strings.
- Runtime core-result files containing personal data.

## Branches

- `main` — stable version.
- `develop` — integrated development version.
- `feature/<task-id>-<short-name>` — one change.

Example:

`feature/ECOM-001-core-foundation`

## First workflow

```bash
git checkout -b feature/ECOM-001-core-foundation
git add AGENTS.md cores contracts docs .cursor .github .gitignore
git commit -m "chore: add AI-first core foundation"
git push -u origin feature/ECOM-001-core-foundation
```

Then create a Pull Request from the feature branch into `develop` or `main`.

## Commit convention

- `chore:` project setup and configuration
- `docs:` documentation only
- `feat:` new behavior
- `fix:` bug fix
- `test:` tests
- `refactor:` internal restructuring without behavior change

## Task ID

Every planned change receives a human-readable task ID:

- `ECOM-001` — Core foundation
- `ECOM-002` — CoreResult C# model
- `ECOM-003` — ProductService
- `ECOM-004` — StockService
- `ECOM-005` — PriceService
- `ECOM-006` — OrderWorkflowService
