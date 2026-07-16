# Socialize Backend

A production-realistic .NET 8 social media backend, built as a strong integration-training
target for Flutter developers. See [`PLAN.md`](./PLAN.md) for the original design brief and
[`API.md`](./API.md) for the full client-facing integration guide.

## Prerequisites

- Docker + Docker Compose v2+ (this is all you need to **run** the backend)
- .NET 8 SDK (8.0.417) — only needed for local development / authoring migrations
- Flutter 3.41+ — only needed if you're building the mobile client separately

## Quickstart

```bash
docker compose up --build
```

This starts Postgres, pgAdmin, and the API. On first boot the API applies EF Core migrations and
seeds demo data automatically — no manual steps required.

| Service | URL |
|---|---|
| API | http://localhost:8080 |
| Swagger UI (interactive API explorer) | http://localhost:8080/swagger |
| Health check | http://localhost:8080/health |
| pgAdmin | http://localhost:5050 (admin@socialize.local / admin) |

Demo accounts (see `API.md` for the full list): username `alice`, password `P@ssw0rd123`.

## Solution structure

Clean Architecture with a strict dependency rule: `Api → Infrastructure → Application → Domain`
(Domain depends on nothing). CQRS via MediatR — each use case is a command/query + handler under
`Socialize.Application`, organized by feature area (Auth, Users, Posts, Engagement, Notifications,
Search).

```
backend/
├── Socialize.sln
├── docker-compose.yml
├── API.md
├── src/
│   ├── Socialize.Domain/          # Entities, enums — no dependencies
│   ├── Socialize.Application/     # CQRS commands/queries, DTOs, validators, abstractions
│   ├── Socialize.Infrastructure/  # EF Core, JWT/BCrypt, file storage, SignalR, full-text search
│   └── Socialize.Api/             # Controllers, middleware, SignalR hub, Program.cs
└── tests/
    └── Socialize.Application.Tests/  # xUnit handler/validator unit tests
```

## Local development (without Docker)

```bash
dotnet tool restore                 # restores the pinned dotnet-ef 8.x (see .config/dotnet-tools.json)
dotnet build
dotnet test tests/Socialize.Application.Tests
```

Running the API directly against a local Postgres requires `ConnectionStrings:Default` to point
at a reachable instance (see `src/Socialize.Api/appsettings.Development.json`).

### Working with migrations

```bash
dotnet ef migrations add <Name> -p src/Socialize.Infrastructure -s src/Socialize.Api -o Persistence/Migrations
dotnet ef database update -p src/Socialize.Infrastructure -s src/Socialize.Api
```

The API also auto-applies pending migrations on startup, so `database update` is only needed when
authoring a new migration locally.

## Key design decisions (see `specs/001-social-media-backend/research.md` for full rationale)

- **Auth**: JWT access token (15 min) + rotating opaque refresh token (7 days, hashed at rest).
- **Pagination**: keyset/cursor pagination everywhere — no OFFSET, no unbounded lists.
- **Feed**: posts from followed users only, newest first.
- **Search**: native PostgreSQL full-text search (`tsvector` + GIN index, `ts_rank`).
- **Images**: local volume storage, ≤4 per post, ≤5MB each, JPEG/PNG/WebP verified by magic number.
- **Real-time**: SignalR hub at `/hubs/notifications`, JWT passed via `?access_token=` query string.
- **Errors**: consistent RFC 7807 ProblemDetails for all failure responses.

## Feature documentation

The full spec, plan, research, data model, API contract, and task breakdown for this backend live
under [`specs/001-social-media-backend/`](../specs/001-social-media-backend/) at the repo root.
