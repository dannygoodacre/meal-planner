# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Meal Planner: an ASP.NET Core (.NET 10) API with PostgreSQL in `backend/`, and a React + Vite SPA in `frontend/`. In production a single image serves both, with the SPA in `wwwroot`.

## Commands

Backend, run from `backend/`. `global.json` opts into Microsoft.Testing.Platform, so `dotnet test` needs `--solution` or `--project`.

```shell
dotnet build MealPlanner.slnx
dotnet test --solution MealPlanner.slnx                                    # all tests; integration tests need Docker
dotnet test --project tests/MealPlanner.Application.Tests                  # unit tests only
dotnet test --project tests/MealPlanner.IntegrationTests --filter-method "*AddFoodPipelineTests.*"   # filter by method
dotnet run --project src/MealPlanner.Web                                   # http://localhost:5192, Scalar UI at /scalar in Development
dotnet ef migrations add <Name> -p src/MealPlanner.Data -s src/MealPlanner.Web
```

Frontend, run from `frontend/`: `npm run dev` (http://localhost:5173), `npm run build` (`tsc -b && vite build`), `npm run lint`, `npm run prettier:check`, `npm test` (Vitest, `vitest run`; tests sit beside the code as `*.test.ts`, covering pure logic such as `schema.ts`, `macros.ts` and request mapping, not components).

Local development needs the dev database: `docker compose -f docker-compose.dev.yml up -d` (from the repo root) starts Postgres as `meal-planner-dev-db` on `127.0.0.1:5432`, matching `appsettings.Development.json`; `down` stops it and keeps the data volume. The Vite dev server proxies `/api` to the backend on `http://localhost:5192`, so run both and use the app on port 5173. `scripts/seed.py` wipes that database and reseeds it through the running API. It is destructive.

Full stack in Docker, from the repo root: copy `.env.example` to `.env`, fill it in, then run `docker compose up -d`. `MEAL_PLANNER_VERSION` picks the `ghcr.io` image tag and the build's app version; unset, it builds a local `dev` image versioned `0.0.0-dev`. The app is bound to `127.0.0.1` (reach it over Tailscale, e.g. with `tailscale serve`) and the database has no published port.

## Backend architecture

There are three projects, and dependencies point inward: `Web → Application ← Data`.

- **`MealPlanner.Application`** holds the domain entities, repository interfaces (`Abstractions/Repositories`) and CQRS handlers. It is built on the author's own packages, `DannyGoodacre.Cqrs` and `DannyGoodacre.Results`. These are pre-release 1.0.0-alpha versions, and their APIs change between alphas.
- **`MealPlanner.Data`** holds the EF Core `ApplicationContext`, the repository implementations and the migrations.
- **`MealPlanner.Web`** holds thin controllers, the request DTOs (`Models/`, mapped to commands in `Extensions/RequestExtensions.cs`) and `Program.cs`. `Program.cs` runs `MigrateAsync()` at startup and serves the SPA from `wwwroot` with response compression: hashed `/assets/*` are cached as immutable, everything else (`index.html`, the SPA fallback) is `no-cache` so a deploy is picked up on reload.

**Vertical slices:** each use case is one file per entity folder (`Foods/`, `Meals/`, `MealPlans/`), containing a public command or query record and an `internal sealed` handler. `AddApplication()` registers every handler and validator by assembly scan, so there is no manual DI registration.

**Persistence is implicit.** A command implementing `IStateCommand` is wrapped in a unit of work that saves the change tracker when the handler returns success. That unit of work is registered by `AddEntityFrameworkUnits<ApplicationContext>()` in `Data/ServiceCollectionExtensions.cs`. Handlers therefore never call `SaveChanges`: an update loads with `GetWithTrackingAsync` and mutates the entity. The context defaults to `NoTracking`, so a plain `GetAsync` result won't persist changes.

**Validation is by marker interface.** Validators are open generics constrained on an interface, for example `FoodDetailsValidator<T> where T : IRequest, IFoodDetails`. Implementing `IFoodDetails`, `IMealDetails`, `IMealPlanDetails`, `IPublicIdRequest` or `ISearchTermRequest` on a command or query opts it into that validation automatically. The shared rules are the `ValidationState` extensions in `MealPlanner.Application/Extensions/ValidationStateExtensions.cs`, which add `ValidationError`s with a machine-readable code such as `"Required"` or `"NotEmpty"`. Tests assert on these codes.

**Results to HTTP:** handlers return `DannyGoodacre.Results` types. `Web/Extensions/ResultExtensions.cs` maps them: `Success` gives 200 or 204 (creates return 201 from the controller via `CreatedAtAction`), `Invalid` 400 with `ValidationProblemDetails`, `NotFound` 404, `Conflict` 409 and `DomainError` 422.

**Entities and identity:** entities derive from `PublicEntity`. They have an internal `int Id` primary key and a `Guid PublicId`, which is the only ID exposed through the API. Setting `Name` also sets `NormalizedName`. `ApplicationContext` configures every `PublicEntity` by reflection: a unique `PublicId`, a `pg_trgm` GIN index on `NormalizedName` for search, and, for types marked `IUniqueRegistryItem`, a unique index on `NormalizedName`, so names are unique case-insensitively.

**Meal plans copy meals.** `MealPlanMeal` and `MealPlanIngredient` are independent copies owned by the plan. They are not references to `Meal`, so editing a meal doesn't change existing plans.

## Tests

- **`MealPlanner.Application.Tests`** contains unit tests for validators and domain logic, using `ShouldHaveErrorCode` and similar from `DannyGoodacre.Testing`.
- **`MealPlanner.IntegrationTests`** starts one Testcontainers Postgres per run as an xUnit v3 assembly fixture, migrates it, and resets data with Respawn before each test.
  - `TestHost` builds the real `AddData()` and `AddApplication()` service graph without the web layer.
  - Pipeline tests resolve handlers from DI, so validation and the unit of work run exactly as in production.
  - Read assertions through `ReadAsync`, which uses a fresh scope, to see committed data.

## Frontend architecture

- **API calls:** `src/api/client.ts` wraps `fetch` and throws `ApiError`, which carries the server's `ValidationProblemDetails`. The per-entity modules (`food.ts`, `meal.ts`, `mealPlan.ts`) call the `/api/foods`, `/api/meals` and `/api/meal-plans` routes.
- **Data fetching:** TanStack Query. Every key in `src/api/queryKeys.ts` starts with its entity's `all` key, so invalidating `all` refreshes both details and searches. Search popovers share the query factories in that file so they hit the same cache entries. Mutations use `src/hooks/useInvalidatingMutation.ts`, passing the `all` keys whose cache they make stale.
- **Forms:** react-hook-form with the zod schemas in `src/schema.ts`. These mirror the backend's validation rules, so a rule change usually belongs in both places.
- **UI:** `components/ui/` is shadcn-generated, the rest of `components/` holds components that are shared across features or not tied to any one feature, and `src/features/<entity>/` holds each entity's components and hooks (only generic hooks go in `src/hooks`). Pages are in `src/pages`, with routes in `App.tsx`.
- **API base URL:** requests are always same-origin `/api/...` paths. In production the .NET app serves the SPA; in dev the Vite proxy (`vite.config.ts`) forwards `/api` to the backend, so there is no CORS setup.

## CI and releases

`.github/workflows/ci.yml` runs on pushes to `main` and on PRs: backend tests, then frontend lint, prettier and build. `release.yml` is run manually from `main` with a version. It reruns CI on the released commit, pushes `ghcr.io/dannygoodacre/meal-planner` tagged with the exact version and `latest` (pre-releases like `0.1.0-alpha.1` get only their exact tag), passing the version through `--build-arg VERSION`, and finally creates the `v<version>` tag and GitHub Release. Versions come only from git tags, so there is no version to bump in the code.

PR titles must be conventional commits (`pr-title.yml`); they become the squash commits that `CHANGELOG.md` is built from. To release: run `release.yml` with the version. It calls `changelog.yml`, which opens a changelog PR (or skips it if `CHANGELOG.md` already has a hand-written section for the version), then waits for approval on the `release` environment. Edit the PR down to user-facing changes, keeping the `## [<version>]` heading, merge it, and then approve the run, which releases the merge commit. `cliff.toml` maps `!` commits to "Upgrade notes" (use it for migrations or `.env` changes that need care), `feat` to Added, `perf` to Changed and `fix` to Fixed, and skips the other types. `release.yml` fails before publishing anything if `CHANGELOG.md` has no section for the version, and uses that section as the GitHub Release body.
