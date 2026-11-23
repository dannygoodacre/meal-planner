# Meal Planner

An application for managing food and structured meal plans: an ASP.NET Core (.NET 10) API with PostgreSQL, and a React + Vite frontend, shipped as a single Docker image.

## Running it

Copy `.env.example` to `.env`, fill it in, then from the repo root:

```shell
docker compose up -d
```

The app listens on `127.0.0.1:${APP_PORT}`; the database is not exposed.

## Development

From the repo root, start the dev database, then run the backend and frontend in separate terminals:

```shell
docker compose -f docker-compose.dev.yml up -d
dotnet run --project backend/src/MealPlanner.Web      # API on http://localhost:5192
npm --prefix frontend run dev                          # app on http://localhost:5173, proxies /api to the backend
```

Run `npm --prefix frontend ci` once first. `python scripts/seed.py` wipes the dev database and fills it with sample data.

## Tests

From `backend/`:

```shell
dotnet test --solution MealPlanner.slnx
```

`backend/global.json` opts into the Microsoft.Testing.Platform runner, so the solution must be passed with `--solution`. Integration tests need Docker.
