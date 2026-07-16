# 📋 Socialize Backend — Implementation Plan

A full, production-realistic **.NET 8** social media backend, built to serve as a
strong integration-training target for **Flutter** developers.

---

## 1. Tech Stack (confirmed)

- **ASP.NET Core 8** Web API + **Clean Architecture + CQRS** (MediatR)
- **PostgreSQL** via **EF Core 8** (Npgsql)
- **JWT** access + **refresh tokens**
- **SignalR** for real-time notifications
- **Swagger/OpenAPI**, **Serilog** logging, **FluentValidation**
- **Docker Compose** (API + Postgres + pgAdmin)

### Toolchain notes
- Local SDK: **.NET 8.0.417**, Docker 29.4 + Compose v5.1.1, Flutter 3.41.6.
- The global `dotnet ef` tool is **v10**, but the project targets **.NET 8** — we pin
  EF Core to **8.x** and use a **local tool manifest** (`dotnet-ef` 8.x) so migrations
  run cleanly against a net8 solution.

---

## 2. Solution Structure

```
backend/
├── Socialize.sln
├── docker-compose.yml
├── .dotnet-tools.json                 # pins dotnet-ef 8.x
├── src/
│   ├── Socialize.Domain/              # Entities, enums, domain events (no dependencies)
│   ├── Socialize.Application/         # CQRS commands/queries, DTOs, validators, interfaces
│   ├── Socialize.Infrastructure/      # EF Core, repositories, JWT, file storage, SignalR
│   └── Socialize.Api/                 # Controllers, middleware, DI, SignalR hubs, Program.cs
└── tests/
    └── Socialize.Application.Tests/   # unit tests for handlers
```

**Dependency rule:** Api → Infrastructure → Application → Domain
(Domain depends on nothing).

---

## 3. Domain Entities

- **User** (Id, UserName, Email, PasswordHash, DisplayName, Bio, AvatarUrl, CreatedAt)
- **RefreshToken** (Token, UserId, ExpiresAt, RevokedAt)
- **Post** (Id, AuthorId, Content, CreatedAt) + **PostImage** (Url, PostId)
- **Comment** (Id, PostId, AuthorId, Content, CreatedAt)
- **Like** (UserId, PostId) — composite key
- **Follow** (FollowerId, FolloweeId) — composite key
- **Notification** (Id, RecipientId, ActorId, Type, EntityId, IsRead, CreatedAt)

---

## 4. API Endpoints

| Area | Endpoints |
|---|---|
| **Auth** | `POST /register`, `POST /login`, `POST /refresh`, `POST /logout`, `GET /me` |
| **Users** | `GET /users/{username}`, `PUT /users/me`, `POST /users/me/avatar`, `POST /users/{id}/follow`, `DELETE /users/{id}/follow`, `GET /users/{id}/followers`, `GET /users/{id}/following` |
| **Posts** | `POST /posts` (text+images), `GET /posts/{id}`, `DELETE /posts/{id}`, `GET /feed?cursor=` (paginated), `GET /users/{id}/posts` |
| **Engagement** | `POST /posts/{id}/like`, `DELETE /posts/{id}/like`, `POST /posts/{id}/comments`, `GET /posts/{id}/comments`, `DELETE /comments/{id}` |
| **Notifications** | `GET /notifications`, `POST /notifications/{id}/read`, `POST /notifications/read-all` + **SignalR `/hubs/notifications`** |
| **Search** | `GET /search/users?q=`, `GET /search/posts?q=` |

All list endpoints use **cursor pagination**; all protected routes require the JWT bearer.

---

## 5. CQRS Pattern (per feature)

Each use case = a MediatR request + handler, e.g.:

```
Application/Posts/Commands/CreatePost/{CreatePostCommand, Handler, Validator}
Application/Posts/Queries/GetFeed/{GetFeedQuery, Handler, FeedItemDto}
```

Cross-cutting via MediatR pipeline behaviors: **Validation**, **Logging**,
**exception → HTTP** mapping.

---

## 6. Auth Flow (training-realistic)

1. Login → returns short-lived **access token (15 min)** + **refresh token (7 days)**
2. `401` on expired access token → Flutter interceptor calls `/refresh`
3. Refresh rotates the token (old one revoked) — teaches secure token handling

---

## 7. Real-time (SignalR)

- On like/comment/follow, the handler raises a notification → persisted →
  **pushed live** to the recipient's SignalR connection.
- Flutter dev learns WebSocket auth (JWT in query string) + live UI updates.

---

## 8. Deliverables that make training strong

- **Swagger UI** at `/swagger` — self-documenting API to explore
- **Seed data** (demo users + posts) so the app isn't empty on first run
- **One-command run**: `docker compose up` starts API + DB + auto-migrates
- **`API.md`** — a Flutter-facing guide: base URL, auth flow, every endpoint with
  request/response JSON examples

---

## 9. Build Phases (in order, verified between each)

1. **Scaffold** solution, projects, Docker, Postgres connection, health check
2. **Domain + EF Core** entities, DbContext, first migration
3. **Auth** (register/login/refresh/JWT) — verifiable end-to-end
4. **Users & follow**
5. **Posts, feed, image upload**
6. **Likes & comments**
7. **Notifications + SignalR**
8. **Search + seed data + API.md + polish**

Each phase is verified (Swagger/curl) before moving on.

---

## 10. Scope note

The **Flutter mobile app is out of scope** for this plan — it delivers the backend that
Flutter trainees integrate against. Wiring up the Flutter side can follow later as a
separate effort.
