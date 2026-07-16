# Implementation Plan: Social Media Backend for Flutter Integration Training

**Branch**: `001-social-media-backend` | **Date**: 2026-07-16 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-social-media-backend/spec.md`

## Summary

Deliver a production-realistic social media REST + real-time backend that Flutter trainees integrate against. It provides authenticated sessions (JWT access + rotating refresh), a followed-users feed, posts with image upload, engagement (likes/comments), live notifications, and full-text search — all self-documented (Swagger + `API.md`), seeded with demo data, and startable with one command (`docker compose up`).

Technical approach (from `backend/PLAN.md`, confirmed by clarifications): ASP.NET Core 8 Web API structured as **Clean Architecture + CQRS** (MediatR), **EF Core 8** over **PostgreSQL**, **SignalR** for real-time notification push, **PostgreSQL full-text search** for the search feature, local file storage for images, **Serilog** logging, **FluentValidation** via a MediatR pipeline behavior, and **Swashbuckle** for the interactive API description.

## Technical Context

**Language/Version**: C# 12 on .NET 8 (SDK 8.0.417). EF Core pinned to 8.x via a local tool manifest (`dotnet-ef` 8.x) so migrations run cleanly against a net8 solution despite a global `dotnet ef` v10.

**Primary Dependencies**: ASP.NET Core 8 (Web API + SignalR), EF Core 8 + Npgsql, MediatR (CQRS + pipeline behaviors), FluentValidation, Serilog, Swashbuckle.AspNetCore (Swagger/OpenAPI), BCrypt.Net (password hashing), System.IdentityModel.Tokens.Jwt (JWT).

**Storage**: PostgreSQL 16 (Docker) via EF Core 8 (Npgsql). Uploaded images stored on a mounted local volume served as static files; image URLs persisted in the database. PostgreSQL `tsvector`/`tsquery` (GIN-indexed) backs full-text search.

**Testing**: xUnit unit tests for Application handlers/validators (`tests/Socialize.Application.Tests`), with FluentAssertions and an in-memory or SQLite provider for handler tests; Swagger/curl for phase-level end-to-end verification per PLAN.md.

**Target Platform**: Cross-platform .NET 8 running in a Linux container; orchestrated locally with Docker Compose (API + Postgres + pgAdmin).

**Project Type**: Web service (REST/JSON API + WebSocket real-time hub). The Flutter mobile client is out of scope.

**Performance Goals**: Live notification delivered to a connected recipient within 2 s of the triggering action (SC-004). Feed/list endpoints return bounded cursor pages regardless of data volume (SC-005). Training-scale interactivity (sub-second typical responses); not tuned for high concurrency.

**Constraints**: JWT access lifetime 15 min, refresh lifetime 7 days with rotation-on-use and revocation-on-logout. Image uploads: ≤4 per post, ~5 MB each, JPEG/PNG/WebP. All list endpoints use cursor pagination. Clean Architecture dependency rule enforced: Api → Infrastructure → Application → Domain (Domain depends on nothing).

**Scale/Scope**: Training target — moderate data volumes (hundreds of demo users, thousands of posts). ~30 REST endpoints across 6 areas + 1 SignalR hub. Single-region, single-instance operation assumed.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

The project constitution (`.specify/memory/constitution.md`) is an unratified template with placeholder principles, so there are **no binding constitutional gates**. In their absence, the plan adopts the guardrails implied by `backend/PLAN.md` and treats them as the review criteria:

| Guardrail | Status | Notes |
|-----------|--------|-------|
| Clean Architecture dependency rule (Api → Infrastructure → Application → Domain; Domain depends on nothing) | PASS | Enforced by project references; verified per phase |
| CQRS: each use case = a MediatR request + handler (+ validator) | PASS | Structure mirrors PLAN.md §5 |
| Cross-cutting via pipeline behaviors (validation, logging, exception→HTTP) | PASS | No logic duplication in controllers |
| Handler unit tests for non-trivial use cases | PASS | `Socialize.Application.Tests` |
| Phase-gated delivery, verified (Swagger/curl) before advancing | PASS | 8 phases per PLAN.md §9 |
| Self-documentation + one-command run + seed data | PASS | FR-025/026/027; SC-001/002 |

**Result**: PASS (no violations; Complexity Tracking not required).

**Recommendation**: Ratify a real constitution before further features so future gates are enforceable. Not blocking for this plan.

## Project Structure

### Documentation (this feature)

```text
specs/001-social-media-backend/
├── plan.md              # This file (/speckit-plan output)
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   └── openapi.yaml      # REST + hub contract
├── checklists/
│   └── requirements.md  # Spec quality checklist (from /speckit-specify)
└── tasks.md             # Phase 2 output (/speckit-tasks — NOT created here)
```

### Source Code (repository root)

The solution lives under `backend/` (this working directory). Layout follows PLAN.md §2 — Clean Architecture with four source projects and one test project:

```text
backend/
├── Socialize.sln
├── docker-compose.yml                  # API + Postgres + pgAdmin
├── .config/dotnet-tools.json           # pins dotnet-ef 8.x (local tool manifest)
├── API.md                              # Flutter-facing integration guide (FR-026)
├── src/
│   ├── Socialize.Domain/               # Entities, enums, domain events (no dependencies)
│   │   ├── Entities/                   # User, RefreshToken, Post, PostImage, Comment, Like, Follow, Notification
│   │   ├── Enums/                      # NotificationType
│   │   └── Common/                     # base types, domain events
│   ├── Socialize.Application/          # CQRS commands/queries, DTOs, validators, interfaces
│   │   ├── Common/                     # Behaviors (Validation, Logging), CursorPagination, Result/Error types
│   │   ├── Abstractions/               # IApplicationDbContext, IJwtService, ITokenService, IFileStorage, INotificationPublisher, ICurrentUser
│   │   ├── Auth/                       # Commands: Register, Login, Refresh, Logout; Query: GetMe
│   │   ├── Users/                      # profile get/update, avatar, follow/unfollow, followers/following
│   │   ├── Posts/                      # Create, Get, EditText, Delete, GetFeed, GetUserPosts
│   │   ├── Engagement/                 # Like/Unlike, AddComment, GetComments, DeleteComment
│   │   ├── Notifications/              # List, MarkRead, MarkAllRead
│   │   └── Search/                     # SearchUsers, SearchPosts (full-text)
│   ├── Socialize.Infrastructure/       # EF Core, repositories, JWT, file storage, SignalR publisher
│   │   ├── Persistence/                # AppDbContext, configurations, migrations, DataSeeder
│   │   ├── Auth/                       # JwtService, TokenService (BCrypt, refresh rotation)
│   │   ├── Storage/                    # LocalFileStorage
│   │   └── Realtime/                   # SignalR NotificationPublisher
│   └── Socialize.Api/                  # Controllers, middleware, DI, SignalR hub, Program.cs
│       ├── Controllers/                # Auth, Users, Posts, Engagement, Notifications, Search
│       ├── Hubs/                       # NotificationsHub (/hubs/notifications)
│       ├── Middleware/                 # ExceptionHandlingMiddleware (exception → HTTP)
│       └── Program.cs                  # DI, auth, Swagger, Serilog, SignalR, auto-migrate + seed on startup
└── tests/
    └── Socialize.Application.Tests/    # xUnit handler + validator unit tests
```

**Structure Decision**: Single .NET solution under `backend/` using the four-project Clean Architecture layout from PLAN.md. This is a standalone web service; there is no `frontend/` in scope (the Flutter app is a separate future effort). The dependency rule is enforced through project references, and CQRS use cases are organized by feature area under `Socialize.Application`.

## Implementation Phases (delivery order, verified between each)

Mirrors PLAN.md §9; each phase is verifiable via Swagger/curl before the next begins. `/speckit-tasks` will expand these into concrete tasks.

1. **Scaffold** — solution, 4 projects + test project, Docker Compose, Postgres connection, Serilog, Swagger, health check.
2. **Domain + EF Core** — entities, `AppDbContext` + configurations, initial migration, full-text `tsvector` columns/indexes.
3. **Auth** — register/login/refresh/logout/me, JWT issuance, BCrypt hashing, refresh rotation + revocation.
4. **Users & follow** — profile get/update, avatar upload, follow/unfollow, followers/following lists.
5. **Posts, feed, image upload** — create/get/edit-text/delete post, multipart image upload + validation, followed-users feed, user posts.
6. **Likes & comments** — like/unlike, add/list/delete comments.
7. **Notifications + SignalR** — notification creation on like/comment/follow, persistence, live push, JWT-authenticated hub, list/mark-read.
8. **Search + seed data + API.md + polish** — full-text user/post search, demo seed data, `API.md` guide, final Swagger polish.

## Complexity Tracking

No constitutional violations to justify. Section intentionally left empty.
