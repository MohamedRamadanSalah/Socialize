# Phase 0 Research: Social Media Backend

**Feature**: 001-social-media-backend | **Date**: 2026-07-16

The tech stack is fixed by `backend/PLAN.md` and confirmed by clarifications, so most "unknowns" are *how to apply* each choice well rather than *what to choose*. Each item below records the decision, rationale, and alternatives considered.

## R1. EF Core version vs. global tooling

- **Decision**: Target EF Core **8.x**; pin the `dotnet-ef` tool to 8.x via a local tool manifest (`.config/dotnet-tools.json`). Run migrations with `dotnet tool restore` then `dotnet ef …`.
- **Rationale**: The machine's global `dotnet ef` is v10, which emits/expects net10-shaped migration bundles and can break against a net8 solution. A local manifest guarantees every developer and the container use EF 8.x.
- **Alternatives**: Upgrade the whole solution to .NET 10 (rejected — PLAN pins net8 and the training toolchain assumes net8); use `dotnet ef` v10 against net8 (rejected — version-skew errors).

## R2. Password hashing

- **Decision**: **BCrypt** (BCrypt.Net-Next) with a work factor of 11.
- **Rationale**: Battle-tested, salt built-in, simple API, satisfies FR-001 (non-reversible storage). Good teaching example of proper credential storage.
- **Alternatives**: ASP.NET Core Identity's `PasswordHasher` (rejected — pulls in more of Identity than needed; we implement a lean custom auth for training clarity); Argon2 (fine but less ubiquitous in the .NET training ecosystem).

## R3. JWT access + refresh with rotation

- **Decision**: Symmetric-key (HMAC-SHA256) signed JWT access token, 15-min lifetime, carrying `sub` (user id) and `username`. Opaque refresh token = cryptographically random 256-bit value, stored **hashed** in the `RefreshTokens` table with `ExpiresAt` (7 days) and `RevokedAt`. `/refresh` validates → revokes the presented token → issues a new access+refresh pair (rotation). `/logout` revokes the current refresh token.
- **Rationale**: Meets FR-002/003/004 and the clarified 15-min/7-day lifetimes. Rotation + hashed-at-rest storage is the realistic, secure pattern that teaches trainees correct refresh-interceptor behavior.
- **Alternatives**: JWT-as-refresh-token (rejected — cannot be revoked server-side); asymmetric RS256 (unnecessary for a single-issuer training service); sliding sessions/cookies (rejected — mobile client wants bearer tokens).

## R4. Refresh-token reuse detection

- **Decision**: On presentation of an already-revoked refresh token, reject with 401 and log the event. Optionally revoke the user's whole active token set.
- **Rationale**: Demonstrates reuse-detection concept without over-engineering the training scope.
- **Alternatives**: Full token-family tracking with cascade revocation (deferred — nice-to-have, not required by spec).

## R5. Cursor pagination strategy

- **Decision**: **Keyset (seek) pagination**. Cursor = opaque base64 of the last item's ordering keys (`CreatedAt` DESC, then `Id` DESC as tiebreaker). Requests take `?cursor=<opaque>&limit=<n>` (default 20, max 50); responses return `{ items: [...], nextCursor: string|null }`.
- **Rationale**: Stable under inserts, O(1) index seek, no deep-offset cost — the correct realistic pattern (FR-010/014/017/022/023, SC-005). Composite `(CreatedAt, Id)` avoids skipped/duplicated rows when timestamps collide.
- **Alternatives**: OFFSET/LIMIT (rejected — drifts and degrades on large offsets); page-number pagination (same drawbacks).

## R6. Home feed composition (clarified: followed users only)

- **Decision**: Feed = posts whose `AuthorId` is in the set of users the viewer follows, ordered `CreatedAt` DESC, `Id` DESC, keyset-paginated. Implemented as a join between `Follows` (FollowerId = current user) and `Posts`. A user who follows no one gets an empty page.
- **Rationale**: Matches the clarification and makes the follow graph meaningful. Fan-out-on-read is appropriate at training scale (no need for a materialized timeline).
- **Alternatives**: Fan-out-on-write / precomputed timeline table (rejected — premature optimization for training volumes); include own posts (rejected — clarification chose followed-only).

## R7. Image upload & storage

- **Decision**: Multipart form upload. Validate **count ≤ 4/post**, **size ≤ 5 MB each**, **content type ∈ {image/jpeg, image/png, image/webp}** — verified by both declared content-type and magic-number sniffing. Persist files to a mounted volume `wwwroot/uploads/{posts|avatars}/{guid.ext}`, serve via static files, store the relative URL in `PostImages.Url` / `Users.AvatarUrl`.
- **Rationale**: Meets FR-015/SC-009 with clear, testable rejection paths; local volume is right for training (no cloud creds). Magic-number check prevents content-type spoofing — a good security lesson.
- **Alternatives**: Store bytes in DB (rejected — bloats DB, poor practice); cloud object store / S3 (out of scope per assumptions); base64 in JSON body (rejected — inefficient, not idiomatic for files).

## R8. Real-time notifications via SignalR

- **Decision**: A `NotificationsHub` at `/hubs/notifications`, JWT-authenticated. Because WebSocket handshakes can't set Authorization headers, accept the access token via **query string** (`?access_token=`) and wire `JwtBearerEvents.OnMessageReceived` to read it for hub paths. Map each connection to its user via `Context.UserIdentifier` (from the `sub`/NameIdentifier claim) and push with `Clients.User(recipientId)`.
- **Rationale**: Meets FR-020/021 and SC-004 (<2 s). `Clients.User` handles multiple connections per user automatically. Query-string token is the documented SignalR pattern and a real integration lesson for Flutter.
- **Alternatives**: Raw WebSockets (rejected — reinvents SignalR's grouping/reconnection); server-sent events (rejected — one-directional, weaker mobile support); polling only (rejected — fails the live-delivery requirement).

## R9. Notification creation flow (decoupled from request path)

- **Decision**: Command handlers for like/comment/follow persist the domain change, then create and persist a `Notification`, then publish via `INotificationPublisher` (implemented in Infrastructure by the SignalR publisher). Persist-then-push so the notification survives even if no client is connected (FR-019 + FR-022).
- **Rationale**: Keeps the Application layer free of SignalR (dependency rule); persistence is the source of truth, live push is best-effort.
- **Alternatives**: MediatR domain-event/notification handlers for full decoupling (nice, optional — can layer in without changing the contract); push-only without persistence (rejected — violates FR-019).

## R10. Full-text search (clarified: full-text, relevance-ranked)

- **Decision**: PostgreSQL full-text search. Add generated `tsvector` columns — `Users.SearchVector` over (username, display name) and `Posts.SearchVector` over content — with **GIN** indexes. Query with `EF.Functions.ToTsVector`/`WebSearchToTsQuery` and rank via `ts_rank`, ordered by rank then recency, keyset-paginated. Empty query or no matches → empty page (FR-023).
- **Rationale**: Matches the clarification (relevance-ranked, tokenized), leverages native Postgres so no extra infrastructure, and teaches a production-grade search pattern. GIN index keeps it fast at training scale.
- **Alternatives**: `ILIKE '%q%'` substring (rejected — clarification chose full-text; also non-sargable); external engine (Elasticsearch/Meilisearch) (rejected — over-scoped for training, adds ops burden); `pg_trgm` fuzzy (deferred — could complement FTS later).

## R11. Validation & error mapping

- **Decision**: FluentValidation validators per command/query, invoked by a MediatR **ValidationBehavior**. A central **ExceptionHandlingMiddleware** maps exception/result types to HTTP + **RFC 7807 ProblemDetails**: validation → 400, unauthorized → 401, forbidden → 403, not-found → 404, conflict (dup username/email/follow) → 409. Consistent machine-readable shape (FR-024, SC-008).
- **Rationale**: One consistent, branchable error contract for the Flutter client; keeps controllers thin.
- **Alternatives**: Data annotations (rejected — less expressive); per-controller try/catch (rejected — duplication, inconsistency).

## R12. Ownership / authorization

- **Decision**: Resource-level ownership checks inside handlers (compare `ICurrentUser.Id` to the resource's author/owner id) returning a forbidden result when mismatched. Applies to post edit/delete and comment delete (FR-013/018/030, SC-006).
- **Rationale**: Simple, explicit, testable in unit tests; no need for a policy framework at this scope.
- **Alternatives**: ASP.NET resource-based authorization handlers (heavier; fine but more ceremony than needed).

## R13. One-command run, auto-migrate, and seeding

- **Decision**: `docker compose up` starts Postgres, pgAdmin, and the API. On startup the API applies pending migrations (`db.Database.Migrate()`) and runs an idempotent `DataSeeder` (seeds only if the DB is empty) creating demo users (known passwords documented in `API.md`), follows, posts with images, likes, comments, and notifications (FR-027, SC-001/007).
- **Rationale**: Zero-config first-run experience; feed and search are non-empty immediately.
- **Alternatives**: Manual `dotnet ef database update` + seed script (rejected — fails the single-command goal); seeding via migrations (rejected — mixes schema and data lifecycles).

## R14. Health check

- **Decision**: ASP.NET Core health checks at `/health`, including a Npgsql/DbContext check verifying database reachability (FR-028).
- **Rationale**: Standard, container-friendly readiness signal used by Compose healthchecks and quickstart validation.
- **Alternatives**: Custom ping endpoint (rejected — reinvents the built-in feature).

## R15. Logging & observability

- **Decision**: **Serilog** with structured JSON to console (container-friendly), request logging middleware, and correlation of significant domain events (registration, login, notification push) (FR-029).
- **Rationale**: Debuggability during training; readable in `docker compose logs`.
- **Alternatives**: Default `ILogger` only (works but less structured); full OpenTelemetry export (deferred — out of scope).

## Resolved unknowns

All Technical Context items are resolved; **no `NEEDS CLARIFICATION` markers remain**. The five spec-level clarifications (feed scope, upload limits, token lifetimes, full-text search, post-edit scope) are reflected in R6, R7, R3, R10, and the data model respectively.
