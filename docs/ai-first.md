# AI-first for ECommerce Cores

## Working definition

AI-first means designing the workflow from the beginning around AI capabilities, while keeping deterministic code, permissions, verification, logs, and humans responsible for safety-critical actions.

It does not mean replacing all code with prompts.

## Our model

1. Human defines the goal and approves sensitive changes.
2. Headliner maintains architecture and priorities.
3. AI/Cursor reads repository instructions and proposes code changes.
4. Tests, schemas, services, and database constraints verify the work.
5. ASP.NET Core executes business operations.
6. EF Core accesses SQLite.
7. SQLite remains the operational source of truth.
8. Git records changes to the system, not live business data.

## AI-first development loop

Intent -> Specification -> AI implementation -> Build/tests -> Human review -> Pull Request -> Merge

## AI-first runtime loop

Customer language -> AI interpretation -> Structured draft -> Validation -> Human confirmation when needed -> Deterministic services -> Database -> Auditable result

## Non-negotiable principle

AI may decide what should happen within granted permissions. Application code proves what actually happened.
