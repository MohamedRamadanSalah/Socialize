---
description: "Task list for Social Media Backend for Flutter Integration Training"
---

# Tasks: Social Media Backend for Flutter Integration Training

**Input**: Design documents from `/specs/001-social-media-backend/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/openapi.yaml, quickstart.md

**Tests**: Handler/validator unit tests are included because `backend/PLAN.md` §2 defines a dedicated `Socialize.Application.Tests` project and the plan's guardrails require handler unit tests for non-trivial use cases. They are scoped to Application-layer logic; end-to-end verification is via Swagger/curl (quickstart.md).

**Organization**: Tasks are grouped by user story (from spec.md) to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1–US7)
- All paths are relative to repo root; the solution lives under `backend/`.

## Path Conventions (from plan.md)

- Domain: `backend/src/Socialize.Domain/`
- Application: `backend/src/Socialize.Application/`
- Infrastructure: `backend/src/Socialize.Infrastructure/`
- API: `backend/src/Socialize.Api/`
- Tests: `backend/tests/Socialize.Application.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Solution scaffold, dependencies, containerization, tooling.

- [ ] T001 Create solution `backend/Socialize.sln` with four projects (`Socialize.Domain`, `Socialize.Application`, `Socialize.Infrastructure`, `Socialize.Api`) and test project `tests/Socialize.Application.Tests`; wire project references so Api→Infrastructure→Application→Domain (Domain references nothing)
- [ ] T002 [P] Add NuGet packages per project: MediatR + FluentValidation (Application); EF Core 8 + Npgsql.EntityFrameworkCore.PostgreSQL + BCrypt.Net-Next + Microsoft.AspNetCore.SignalR (Infrastructure); Swashbuckle.AspNetCore + Serilog.AspNetCore + Microsoft.AspNetCore.Authentication.JwtBearer + AspNetCore.HealthChecks.NpgSql (Api); xUnit + FluentAssertions + Microsoft.EntityFrameworkCore.Sqlite (tests). Pin all EF Core packages to 8.x
- [ ] T003 [P] Create `backend/.config/dotnet-tools.json` pinning `dotnet-ef` 8.x (local tool manifest, per research R1)
- [ ] T004 [P] Create `backend/docker-compose.yml` with services `api`, `postgres:16`, `pgadmin`; named volumes for Postgres data and `/app/wwwroot/uploads`; Postgres healthcheck; `api` depends_on healthy Postgres
- [ ] T005 [P] Create multi-stage `backend/src/Socialize.Api/Dockerfile` (SDK build → runtime, .NET 8, exposes port 8080)
- [ ] T006 Create `backend/src/Socialize.Api/appsettings.json` + `appsettings.Development.json` with connection string, JWT settings (issuer/audience/signing key, AccessTokenMinutes=15, RefreshTokenDays=7), and upload limits (MaxImagesPerPost=4, MaxImageBytes=5242880, AllowedTypes=jpeg,png,webp)
- [ ] T007 [P] Configure Serilog structured JSON console logging + request logging in `backend/src/Socialize.Api/Program.cs` (research R15)
- [ ] T008 [P] Add `backend/.editorconfig`, `backend/.gitignore` (bin/obj, uploads), and `backend/Directory.Build.props` enabling nullable + implicit usings + warnings-as-errors

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain model, persistence, cross-cutting pipeline, and API host that ALL user stories depend on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [ ] T009 [P] Create `User` entity in `backend/src/Socialize.Domain/Entities/User.cs` (data-model.md)
- [ ] T010 [P] Create `RefreshToken` entity in `backend/src/Socialize.Domain/Entities/RefreshToken.cs`
- [ ] T011 [P] Create `Post` and `PostImage` entities in `backend/src/Socialize.Domain/Entities/Post.cs` and `PostImage.cs`
- [ ] T012 [P] Create `Comment` entity in `backend/src/Socialize.Domain/Entities/Comment.cs`
- [ ] T013 [P] Create `Like` entity (composite key) in `backend/src/Socialize.Domain/Entities/Like.cs`
- [ ] T014 [P] Create `Follow` entity (composite key) in `backend/src/Socialize.Domain/Entities/Follow.cs`
- [ ] T015 [P] Create `Notification` entity and `NotificationType` enum in `backend/src/Socialize.Domain/Entities/Notification.cs` and `Enums/NotificationType.cs`
- [ ] T016 [P] Define Application abstractions in `backend/src/Socialize.Application/Abstractions/`: `IApplicationDbContext`, `ICurrentUser`, `IFileStorage`, `IJwtService`, `ITokenService`, `INotificationPublisher`
- [ ] T017 [P] Create Result/Error types and domain exceptions (`NotFoundException`, `ForbiddenException`, `ConflictException`, `ValidationException`) in `backend/src/Socialize.Application/Common/`
- [ ] T018 [P] Create cursor pagination helpers (`CursorPage<T>`, opaque `(CreatedAt,Id)` encode/decode) in `backend/src/Socialize.Application/Common/Pagination/` (research R5)
- [ ] T019 Create MediatR `ValidationBehavior` and `LoggingBehavior` plus `DependencyInjection.AddApplication()` in `backend/src/Socialize.Application/DependencyInjection.cs` (depends on T016–T018)
- [ ] T020 Implement `AppDbContext : IApplicationDbContext` with all `DbSet`s and EF configurations (keys, indexes, cascade rules, `tsvector` + GIN for User/Post) in `backend/src/Socialize.Infrastructure/Persistence/AppDbContext.cs` + `Persistence/Configurations/*` (depends on T009–T015)
- [ ] T021 Implement `LocalFileStorage : IFileStorage` (save to `wwwroot/uploads/{posts|avatars}`, return relative URL, delete) in `backend/src/Socialize.Infrastructure/Storage/LocalFileStorage.cs` (research R7)
- [ ] T022 Create `DependencyInjection.AddInfrastructure()` registering `AppDbContext` (Npgsql), `IFileStorage`, and (placeholders for) auth/realtime services in `backend/src/Socialize.Infrastructure/DependencyInjection.cs` (depends on T020, T021)
- [ ] T023 Generate the initial EF Core migration (all 8 tables, composite keys, indexes, `tsvector` columns + GIN indexes) via pinned `dotnet-ef` into `backend/src/Socialize.Infrastructure/Persistence/Migrations/` (depends on T020)
- [ ] T024 Implement `ExceptionHandlingMiddleware` mapping exceptions to RFC 7807 ProblemDetails (400/401/403/404/409) in `backend/src/Socialize.Api/Middleware/ExceptionHandlingMiddleware.cs` (research R11; depends on T017)
- [ ] T025 Implement `CurrentUser : ICurrentUser` reading id/username from claims in `backend/src/Socialize.Api/Auth/CurrentUser.cs` and register with HttpContextAccessor
- [ ] T026 Wire `Program.cs`: AddApplication + AddInfrastructure + controllers + Swagger placeholder + ProblemDetails middleware + static files for `/uploads` + apply migrations on startup (depends on T019, T022, T024, T025)

**Checkpoint**: API boots against Postgres in Docker, migrations applied, health of the host verifiable. User stories can now begin.

---

## Phase 3: User Story 1 - Account creation and secure session (Priority: P1) 🎯 MVP

**Goal**: Register/login/refresh(rotation)/logout/me with JWT access (15 min) + rotating refresh (7 days).

**Independent Test**: Register → login → call `/api/auth/me` → refresh (old refresh now 401) → logout (refresh now 401). Per quickstart Scenario 1.

### Tests for User Story 1

- [ ] T027 [P] [US1] Unit tests for Register/Login/Refresh/Logout handlers (rotation revokes prior token; duplicate email → conflict; bad password → unauthorized) in `backend/tests/Socialize.Application.Tests/Auth/`

### Implementation for User Story 1

- [ ] T028 [P] [US1] Implement `JwtService : IJwtService` (HMAC-SHA256 access token with sub/username, 15-min expiry) in `backend/src/Socialize.Infrastructure/Auth/JwtService.cs` (research R3)
- [ ] T029 [P] [US1] Implement `TokenService : ITokenService` (BCrypt password hash/verify; random refresh token, SHA-256 hash at rest, rotation + revocation) in `backend/src/Socialize.Infrastructure/Auth/TokenService.cs` (research R2, R3, R4)
- [ ] T030 [US1] Configure JWT bearer authentication + authorization in `Program.cs` and register `JwtService`/`TokenService` in `AddInfrastructure` (depends on T028, T029)
- [ ] T031 [P] [US1] Register command + handler + validator in `backend/src/Socialize.Application/Auth/Commands/Register/` (unique username/email → 409; hashed password; returns AuthResponse)
- [ ] T032 [P] [US1] Login command + handler + validator in `backend/src/Socialize.Application/Auth/Commands/Login/`
- [ ] T033 [P] [US1] Refresh command + handler in `backend/src/Socialize.Application/Auth/Commands/Refresh/` (validate → revoke presented → issue new pair)
- [ ] T034 [P] [US1] Logout command + handler in `backend/src/Socialize.Application/Auth/Commands/Logout/` (revoke current refresh token)
- [ ] T035 [P] [US1] GetMe query + handler in `backend/src/Socialize.Application/Auth/Queries/GetMe/`
- [ ] T036 [US1] `AuthController` with register/login/refresh/logout/me in `backend/src/Socialize.Api/Controllers/AuthController.cs` (depends on T031–T035)

**Checkpoint**: US1 fully functional and independently testable (MVP).

---

## Phase 4: User Story 2 - Self-documenting, ready-to-run integration target (Priority: P1)

**Goal**: One-command run, interactive Swagger, health check, seeded demo data, and the `API.md` integration guide.

**Independent Test**: `docker compose up` on a clean checkout → `/health` 200, `/swagger` lists endpoints, demo users present, `API.md` documents auth flow. Per quickstart Scenario "Self-documentation" + SC-001/002/007.

### Implementation for User Story 2

- [ ] T037 [US2] Configure Swagger/OpenAPI with JWT bearer security scheme, grouped tags, and XML comments in `backend/src/Socialize.Api/Program.cs` (+ enable XML doc generation in Api csproj)
- [ ] T038 [US2] Add `/health` endpoint with Npgsql/DbContext readiness check in `Program.cs` and reference it from the Compose healthcheck (research R14)
- [ ] T039 [US2] Verify one-command run: `docker compose up` provisions API+DB, auto-applies migrations, and API answers `/health` within the SC-001 budget; fix wiring/env as needed
- [ ] T040 [P] [US2] Create idempotent `DataSeeder` seeding demo users (known passwords) + a follow graph in `backend/src/Socialize.Infrastructure/Persistence/DataSeeder.cs` (extended with content in Polish T080)
- [ ] T041 [US2] Invoke `DataSeeder` on startup after migration in `Program.cs` (seed only if DB empty)
- [ ] T042 [P] [US2] Author `backend/API.md`: base URL, auth/refresh flow, demo credentials, and an endpoint index (request/response examples completed in Polish T081)

**Checkpoint**: US1 + US2 work; a trainee can start the backend and explore it.

---

## Phase 5: User Story 3 - Profiles and social graph (Priority: P2)

**Goal**: View profiles, update own profile + avatar, follow/unfollow, followers/following lists.

**Independent Test**: Get profile by username → update profile + avatar → follow → appears in follower/following lists → unfollow. Per quickstart Scenario 2.

### Tests for User Story 3

- [ ] T043 [P] [US3] Unit tests for UpdateProfile, Follow (self-follow rejected, idempotent), Unfollow, and followers/following pagination in `backend/tests/Socialize.Application.Tests/Users/`

### Implementation for User Story 3

- [ ] T044 [P] [US3] GetProfileByUsername query + handler (returns UserProfile with follower/following counts + isFollowedByMe) in `backend/src/Socialize.Application/Users/Queries/GetProfile/`
- [ ] T045 [P] [US3] UpdateProfile command + handler + validator (display name, bio) in `backend/src/Socialize.Application/Users/Commands/UpdateProfile/`
- [ ] T046 [US3] UploadAvatar command + handler (validate + store via `IFileStorage`, set AvatarUrl) in `backend/src/Socialize.Application/Users/Commands/UploadAvatar/`
- [ ] T047 [P] [US3] Follow command + handler (self-follow guard, duplicate-safe) in `backend/src/Socialize.Application/Users/Commands/Follow/`
- [ ] T048 [P] [US3] Unfollow command + handler in `backend/src/Socialize.Application/Users/Commands/Unfollow/`
- [ ] T049 [P] [US3] GetFollowers query + handler (cursor) in `backend/src/Socialize.Application/Users/Queries/GetFollowers/`
- [ ] T050 [P] [US3] GetFollowing query + handler (cursor) in `backend/src/Socialize.Application/Users/Queries/GetFollowing/`
- [ ] T051 [US3] `UsersController` (`GET /users/{username}`, `PUT /users/me`, `POST /users/me/avatar`, follow/unfollow, followers/following) in `backend/src/Socialize.Api/Controllers/UsersController.cs` (depends on T044–T050)

**Checkpoint**: US1–US3 independently functional. (Follow notifications are added in US6.)

---

## Phase 6: User Story 4 - Posting and the feed (Priority: P2)

**Goal**: Create post (text + ≤4 images), get post, edit own post text, delete own post, followed-users feed, user posts.

**Independent Test**: Create post with images → get by id → edit text (editedAt set) → feed shows followed authors → non-author edit/delete → 403 → oversized/5th/bad-type image → 400. Per quickstart Scenario 3.

### Tests for User Story 4

- [ ] T052 [P] [US4] Unit tests for CreatePost (image count/size/type validation), EditPostText (ownership + EditedAt), DeletePost (ownership + cascade), GetFeed (followed-only, keyset) in `backend/tests/Socialize.Application.Tests/Posts/`

### Implementation for User Story 4

- [ ] T053 [P] [US4] Image validation helper (count ≤4, ≤5 MB, JPEG/PNG/WebP by content-type + magic-number sniff) in `backend/src/Socialize.Application/Common/Media/ImageValidator.cs` (research R7, SC-009)
- [ ] T054 [US4] CreatePost command + handler + validator (persist post, store images, create PostImage rows) in `backend/src/Socialize.Application/Posts/Commands/CreatePost/` (depends on T053)
- [ ] T055 [P] [US4] GetPost query + handler (author + images + likeCount/commentCount + likedByMe) in `backend/src/Socialize.Application/Posts/Queries/GetPost/`
- [ ] T056 [P] [US4] EditPostText command + handler + validator (ownership → 403; set EditedAt) in `backend/src/Socialize.Application/Posts/Commands/EditPostText/`
- [ ] T057 [P] [US4] DeletePost command + handler (ownership → 403; cascade rows + delete image files) in `backend/src/Socialize.Application/Posts/Commands/DeletePost/`
- [ ] T058 [P] [US4] GetFeed query + handler (posts by followed authors, keyset newest-first) in `backend/src/Socialize.Application/Posts/Queries/GetFeed/` (research R6)
- [ ] T059 [P] [US4] GetUserPosts query + handler (keyset) in `backend/src/Socialize.Application/Posts/Queries/GetUserPosts/`
- [ ] T060 [US4] `PostsController` (`POST /posts`, `GET/PATCH/DELETE /posts/{id}`, `GET /feed`, `GET /users/{id}/posts`) in `backend/src/Socialize.Api/Controllers/PostsController.cs` (depends on T054–T059)

**Checkpoint**: US1–US4 independently functional.

---

## Phase 7: User Story 5 - Engagement: likes and comments (Priority: P3)

**Goal**: Like/unlike (idempotent, de-duplicated), add/list comments (cursor), delete own comment.

**Independent Test**: Like twice → count 1 → unlike → count 0 → add comment → list → delete own comment (non-owner → 403). Per quickstart Scenario 4.

### Tests for User Story 5

- [ ] T061 [P] [US5] Unit tests for Like idempotency/de-dup, Unlike, AddComment, DeleteComment (ownership) in `backend/tests/Socialize.Application.Tests/Engagement/`

### Implementation for User Story 5

- [ ] T062 [P] [US5] LikePost command + handler (idempotent via composite key) in `backend/src/Socialize.Application/Engagement/Commands/LikePost/`
- [ ] T063 [P] [US5] UnlikePost command + handler in `backend/src/Socialize.Application/Engagement/Commands/UnlikePost/`
- [ ] T064 [P] [US5] AddComment command + handler + validator in `backend/src/Socialize.Application/Engagement/Commands/AddComment/`
- [ ] T065 [P] [US5] GetComments query + handler (cursor) in `backend/src/Socialize.Application/Engagement/Queries/GetComments/`
- [ ] T066 [P] [US5] DeleteComment command + handler (ownership → 403) in `backend/src/Socialize.Application/Engagement/Commands/DeleteComment/`
- [ ] T067 [US5] `EngagementController` (`POST/DELETE /posts/{id}/like`, `POST/GET /posts/{id}/comments`, `DELETE /comments/{id}`) in `backend/src/Socialize.Api/Controllers/EngagementController.cs` (depends on T062–T066)

**Checkpoint**: US1–US5 independently functional.

---

## Phase 8: User Story 6 - Real-time notifications (Priority: P3)

**Goal**: Persist + live-push notifications on like/comment/follow; JWT-authenticated hub; list + mark read/all.

**Independent Test**: Connect hub with `?access_token=` → another user likes/comments/follows → receive `ReceiveNotification` within 2 s AND it is listed by `GET /notifications`; mark read/all works; no/expired token rejected. Per quickstart Scenario 5.

### Tests for User Story 6

- [ ] T068 [P] [US6] Unit tests for notification creation on like/comment/follow (no self-notification), MarkRead, MarkAllRead in `backend/tests/Socialize.Application.Tests/Notifications/`

### Implementation for User Story 6

- [ ] T069 [P] [US6] `NotificationsHub` (`/hubs/notifications`, `[Authorize]`) in `backend/src/Socialize.Api/Hubs/NotificationsHub.cs`
- [ ] T070 [US6] Configure `JwtBearerEvents.OnMessageReceived` to read `?access_token=` for hub paths and map the hub in `Program.cs` (research R8; depends on T030, T069)
- [ ] T071 [P] [US6] `SignalRNotificationPublisher : INotificationPublisher` (push to `Clients.User(recipientId)`) in `backend/src/Socialize.Infrastructure/Realtime/SignalRNotificationPublisher.cs`, registered in `AddInfrastructure`
- [ ] T072 [US6] Extend Like (T062), AddComment (T064), and Follow (T047) handlers to persist a `Notification` and publish via `INotificationPublisher` (persist-then-push; skip self-actions) — cross-story integration (research R9; depends on T071)
- [ ] T073 [P] [US6] ListNotifications query + handler (cursor) in `backend/src/Socialize.Application/Notifications/Queries/ListNotifications/`
- [ ] T074 [P] [US6] MarkRead + MarkAllRead commands + handlers in `backend/src/Socialize.Application/Notifications/Commands/`
- [ ] T075 [US6] `NotificationsController` (`GET /notifications`, `POST /notifications/{id}/read`, `POST /notifications/read-all`) in `backend/src/Socialize.Api/Controllers/NotificationsController.cs` (depends on T073, T074)

**Checkpoint**: US1–US6 independently functional; live notifications delivered.

---

## Phase 9: User Story 7 - Search (Priority: P4)

**Goal**: Relevance-ranked full-text search of users and posts (cursor-paginated; empty query/no match → empty page).

**Independent Test**: Search users/posts return ranked results; no-match query returns empty items. Per quickstart Scenario 6.

### Tests for User Story 7

- [ ] T076 [P] [US7] Unit tests for SearchUsers/SearchPosts (ranking order, empty result, pagination) in `backend/tests/Socialize.Application.Tests/Search/`

### Implementation for User Story 7

- [ ] T077 [P] [US7] SearchUsers query + handler (`tsquery` over username/display name, `ts_rank`, keyset) in `backend/src/Socialize.Application/Search/Queries/SearchUsers/` (research R10)
- [ ] T078 [P] [US7] SearchPosts query + handler (`tsquery` over content, `ts_rank`, keyset) in `backend/src/Socialize.Application/Search/Queries/SearchPosts/`
- [ ] T079 [US7] `SearchController` (`GET /search/users`, `GET /search/posts`) in `backend/src/Socialize.Api/Controllers/SearchController.cs` (depends on T077, T078)

**Checkpoint**: All user stories independently functional.

---

## Phase 10: Polish & Cross-Cutting Concerns

**Purpose**: Complete the training deliverables and validate the whole system.

- [ ] T080 [P] Extend `DataSeeder` (T040) with demo posts + images, likes, comments, and notifications so feed/search are non-empty on first run (SC-007) in `backend/src/Socialize.Infrastructure/Persistence/DataSeeder.cs`
- [ ] T081 [P] Complete `backend/API.md` with request/response JSON examples for every endpoint (FR-026, SC-002)
- [ ] T082 [P] Verify Swagger exposes 100% of endpoints with accurate schemas; add missing XML doc comments across controllers
- [ ] T083 Run all quickstart.md scenarios end-to-end via `docker compose up` and record results (SC-001…SC-009)
- [ ] T084 [P] Add `backend/README.md` (prerequisites, one-command run, migration commands, demo credentials)
- [ ] T085 Security hardening pass: confirm magic-number image checks, no secrets/passwords in logs, JWT signing key from config/secret, and consistent ProblemDetails for all error paths
- [ ] T086 [P] Add edge-case unit tests (invalid cursor → empty page, ownership 403s, expired/revoked refresh) in `backend/tests/Socialize.Application.Tests/`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately.
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories.
- **User Stories (Phases 3–9)**: All depend on Foundational.
  - Priority order: US1 (P1) → US2 (P1) → US3 (P2) → US4 (P2) → US5 (P3) → US6 (P3) → US7 (P4).
  - With enough staff, US3–US7 can proceed in parallel after Foundational; note the cross-story hooks below.
- **Polish (Phase 10)**: Depends on the user stories it touches (T080 needs US3–US6 entities; T081/T082 need all endpoints).

### Cross-Story Dependencies (kept minimal)

- **US6 → US3/US5**: T072 extends the Follow (T047), Like (T062), and AddComment (T064) handlers to emit notifications. Those stories remain independently testable *without* US6; the notification side-effect is additive.
- **US7** relies on the `tsvector`/GIN indexes created in Foundational (T020, T023).
- **US2 seed (T040)** seeds users/follows early; content seed (T080) is deferred to Polish because it needs Post/Comment/Like entities from US4/US5.

### Within Each User Story

- Unit tests can be written alongside or before handlers.
- Models/abstractions → handlers/validators → controller → integration.
- Controller task depends on its story's handler tasks.

### Parallel Opportunities

- Setup: T002–T008 (after T001) run in parallel.
- Foundational: entities T009–T015 all parallel; abstractions/common T016–T018 parallel; then T019–T026 as dependencies allow.
- Within a story, all `[P]` handler/query tasks (different folders) run in parallel; the controller task is the join point.
- After Foundational, different developers can own US3, US4, US5, US7 concurrently (US6 integrates last).

---

## Parallel Example: User Story 1

```bash
# Infrastructure auth services (different files) in parallel:
Task: "T028 Implement JwtService in backend/src/Socialize.Infrastructure/Auth/JwtService.cs"
Task: "T029 Implement TokenService in backend/src/Socialize.Infrastructure/Auth/TokenService.cs"

# Then all auth use cases in parallel (different folders):
Task: "T031 Register command+handler+validator"
Task: "T032 Login command+handler+validator"
Task: "T033 Refresh command+handler"
Task: "T034 Logout command+handler"
Task: "T035 GetMe query+handler"
# Join: T036 AuthController
```

---

## Implementation Strategy

### MVP First

1. Phase 1 Setup → 2. Phase 2 Foundational → 3. Phase 3 US1 (Auth).
4. **STOP and VALIDATE**: run quickstart Scenario 1. This is the demonstrable MVP (secure sessions).
5. Add Phase 4 US2 so the backend is self-documenting and one-command runnable — the "strong training target" baseline.

### Incremental Delivery

Foundational → US1 (MVP) → US2 (runnable/documented) → US3 → US4 → US5 → US6 → US7 → Polish. Each story is a deployable increment validated via its quickstart scenario before moving on (mirrors PLAN.md §9's verify-between-phases rule).

---

## Notes

- `[P]` = different files, no dependency on an incomplete task.
- `[Story]` label maps each task to a spec.md user story for traceability.
- Commit after each task or logical group; verify each phase via Swagger/curl before advancing.
- **Task count**: 86 (Setup 8, Foundational 18, US1 10, US2 6, US3 9, US4 9, US5 7, US6 8, US7 4, Polish 7).
