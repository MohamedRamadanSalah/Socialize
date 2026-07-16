# Quickstart & Validation Guide: Socialize Backend

**Feature**: 001-social-media-backend | **Date**: 2026-07-16

This guide proves the feature works end-to-end. It is a run/validation guide — implementation code lives in `backend/src/**` and task breakdown in `tasks.md`. Contract details are in [`contracts/openapi.yaml`](./contracts/openapi.yaml); entities in [`data-model.md`](./data-model.md).

## Prerequisites

- Docker + Docker Compose (v2+). No local .NET or Postgres required to run.
- For development/migrations only: .NET 8 SDK (8.0.417) and `dotnet tool restore` (pins `dotnet-ef` 8.x).

## One-command start (SC-001, FR-027)

```bash
cd backend
docker compose up --build
```

Expected: Postgres, pgAdmin, and the API start; the API applies migrations and seeds demo data on first boot. When ready:

- API base: `http://localhost:8080`
- Interactive API explorer (Swagger UI): `http://localhost:8080/swagger`
- Health check: `GET http://localhost:8080/health` → `200 Healthy`

**Validation**: `curl -s http://localhost:8080/health` returns healthy within 5 minutes of a clean checkout, with no manual config.

## Demo credentials (seeded, documented in API.md)

Synthetic demo users are seeded so the feed/search are non-empty on first run (SC-007). Example: `alice` / `P@ssw0rd123` (full list in `backend/API.md`). These are training placeholders, not real secrets.

## Scenario 1 — Authenticated session lifecycle (US1 · SC-003)

```bash
BASE=http://localhost:8080

# 1. Register (or use a seeded user)
curl -s -X POST $BASE/api/auth/register -H 'Content-Type: application/json' \
  -d '{"userName":"newdev","email":"newdev@example.com","password":"P@ssw0rd123","displayName":"New Dev"}'

# 2. Login → capture tokens
curl -s -X POST $BASE/api/auth/login -H 'Content-Type: application/json' \
  -d '{"userNameOrEmail":"newdev","password":"P@ssw0rd123"}'
# → { accessToken, refreshToken, accessTokenExpiresAt, user }

# 3. Call a protected endpoint with the access token
curl -s $BASE/api/auth/me -H "Authorization: Bearer <accessToken>"          # → 200 profile

# 4. Refresh (rotation): old refresh token is now invalid
curl -s -X POST $BASE/api/auth/refresh -H 'Content-Type: application/json' \
  -d '{"refreshToken":"<refreshToken>"}'                                      # → new pair

# 5. Reusing the OLD refresh token now fails
curl -s -X POST $BASE/api/auth/refresh -H 'Content-Type: application/json' \
  -d '{"refreshToken":"<OLD refreshToken>"}'                                  # → 401

# 6. Logout revokes the current refresh token
curl -s -X POST $BASE/api/auth/logout -H "Authorization: Bearer <accessToken>" \
  -H 'Content-Type: application/json' -d '{"refreshToken":"<current refreshToken>"}'  # → 204
```

**Expected**: steps 1–4 succeed; step 5 and post-logout refresh return `401` (proves rotation + revocation).

## Scenario 2 — Profiles & follow (US3)

```bash
curl -s $BASE/api/users/alice                                                 # → 200 profile
curl -s -X POST $BASE/api/users/<aliceId>/follow -H "Authorization: Bearer <token>"   # → 204
curl -s $BASE/api/users/<aliceId>/followers                                   # → caller appears
curl -s -X DELETE $BASE/api/users/<aliceId>/follow -H "Authorization: Bearer <token>" # → 204
```

**Expected**: follow appears in follower/following lists; duplicate follow stays 204 (idempotent); self-follow → 400.

## Scenario 3 — Posting, feed & edit (US4 · FR-030)

```bash
# Create a post with images (multipart)
curl -s -X POST $BASE/api/posts -H "Authorization: Bearer <token>" \
  -F 'content=Hello from the training backend' \
  -F 'images=@./a.jpg' -F 'images=@./b.png'                                   # → 201 Post

# Feed = posts from followed users only, newest first (paginate with nextCursor)
curl -s "$BASE/api/feed?limit=20" -H "Authorization: Bearer <token>"          # → { items, nextCursor }
curl -s "$BASE/api/feed?limit=20&cursor=<nextCursor>" -H "Authorization: Bearer <token>"

# Edit own post text → editedAt becomes non-null
curl -s -X PATCH $BASE/api/posts/<postId> -H "Authorization: Bearer <token>" \
  -H 'Content-Type: application/json' -d '{"content":"Edited text"}'          # → 200, editedAt set

# A different user editing/deleting it → 403
```

**Expected**: feed only contains followed authors; a user following no one gets an empty page; non-author edit/delete → 403 (SC-006); upload of a 5th image / >5 MB / non-JPEG-PNG-WebP → 400 (SC-009).

## Scenario 4 — Engagement (US5)

```bash
curl -s -X POST $BASE/api/posts/<postId>/like -H "Authorization: Bearer <token>"      # → 204 (idempotent)
curl -s -X POST $BASE/api/posts/<postId>/comments -H "Authorization: Bearer <token>" \
  -H 'Content-Type: application/json' -d '{"content":"Nice post!"}'                    # → 201
curl -s "$BASE/api/posts/<postId>/comments?limit=20"                                  # → paged comments
```

**Expected**: like count reflects one like even if liked twice; only the comment's author can delete it (403 otherwise); comments cannot be edited (no edit endpoint).

## Scenario 5 — Real-time notifications (US6 · SC-004)

1. Connect a SignalR client to `ws(s)://localhost:8080/hubs/notifications?access_token=<accessToken>` (Alice's token).
2. As Bob, like or comment on one of Alice's posts (Scenario 4).
3. Alice's connected client receives a `ReceiveNotification` event **within 2 seconds**.
4. `GET /api/notifications` also lists it (persisted); `POST /api/notifications/{id}/read` and `/read-all` update read state.

**Expected**: live push within 2 s AND persisted; connecting without/with an expired `access_token` is rejected (FR-021).

## Scenario 6 — Full-text search (US7 · FR-023)

```bash
curl -s "$BASE/api/search/users?q=alice" -H "Authorization: Bearer <token>"   # → relevance-ranked
curl -s "$BASE/api/search/posts?q=training" -H "Authorization: Bearer <token>"# → relevance-ranked
curl -s "$BASE/api/search/posts?q=zzzznomatch" -H "Authorization: Bearer <token>"  # → empty items[]
```

**Expected**: relevance-ranked, cursor-paginated results; a no-match query returns an empty page (not an error).

## Self-documentation checks (US2 · SC-002)

- Every endpoint in [`contracts/openapi.yaml`](./contracts/openapi.yaml) appears and is executable in Swagger UI.
- `backend/API.md` documents base URL, the full auth/refresh flow, and request/response examples for every endpoint.

## Developer notes (migrations)

```bash
cd backend
dotnet tool restore                 # restores pinned dotnet-ef 8.x
dotnet ef migrations add <Name> -p src/Socialize.Infrastructure -s src/Socialize.Api
dotnet ef database update -p src/Socialize.Infrastructure -s src/Socialize.Api
```

Runtime auto-migration on startup means the manual `database update` is only needed when authoring new migrations.
