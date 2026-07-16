# Socialize Backend — API Guide (for Flutter integration)

This guide is the client-facing reference for integrating a Flutter app against the Socialize
training backend. It covers the base address, the authentication/refresh flow, demo accounts,
and every endpoint with request/response examples.

## Base URL

```
http://localhost:8080
```

Interactive, browsable API explorer (Swagger UI): `http://localhost:8080/swagger`
Health check: `GET http://localhost:8080/health`

## Demo accounts

The database is seeded on first run with demo users so the feed/search are never empty.
Every seeded user shares the same password: **`P@ssw0rd123`**.

| Username | Display Name |
|----------|--------------|
| alice    | Alice Anderson |
| bob      | Bob Baker |
| carol    | Carol Chen |
| dave     | Dave Diaz |
| erin     | Erin Evans |

## Authentication & refresh flow

1. **Register** (`POST /api/auth/register`) or **Login** (`POST /api/auth/login`) returns an
   `accessToken` (JWT, **15-minute** lifetime) and a `refreshToken` (opaque, **7-day** lifetime).
2. Send the access token on every protected request: `Authorization: Bearer <accessToken>`.
3. When a request fails with `401 Unauthorized` because the access token expired, call
   `POST /api/auth/refresh` with the current `refreshToken` to get a **new** access+refresh pair.
   The old refresh token is revoked immediately (rotation) — do not reuse it.
4. `POST /api/auth/logout` revokes the current refresh token, ending the session server-side.

### Register

```
POST /api/auth/register
Content-Type: application/json

{
  "userName": "newdev",
  "email": "newdev@example.com",
  "password": "P@ssw0rd123",
  "displayName": "New Dev"
}
```

Response `201 Created`:

```json
{
  "accessToken": "eyJhbGciOi...",
  "refreshToken": "8f2b1c...opaque",
  "accessTokenExpiresAt": "2026-07-16T14:22:00Z",
  "user": {
    "id": "b3f1c2a4-1111-4a2b-9c3d-000000000001",
    "userName": "newdev",
    "displayName": "New Dev",
    "bio": null,
    "avatarUrl": null,
    "followerCount": 0,
    "followingCount": 0,
    "isFollowedByMe": false,
    "createdAt": "2026-07-16T14:07:00Z"
  }
}
```

### Login

```
POST /api/auth/login
Content-Type: application/json

{ "userNameOrEmail": "alice", "password": "P@ssw0rd123" }
```

Response: same shape as Register.

### Refresh

```
POST /api/auth/refresh
Content-Type: application/json

{ "refreshToken": "8f2b1c...opaque" }
```

Response: a **new** `AuthResponse` (both tokens rotated).

### Logout

```
POST /api/auth/logout
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "refreshToken": "8f2b1c...opaque" }
```

Response: `204 No Content`.

### Get my account

```
GET /api/auth/me
Authorization: Bearer <accessToken>
```

Response `200 OK`: a `UserProfile` object (same shape as `user` above).

## Error format

All errors are returned as `application/problem+json` (RFC 7807):

```json
{
  "type": "https://httpstatuses.io/400",
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more validation failures occurred.",
  "errors": { "email": ["Email is already taken."] }
}
```

| Status | Meaning |
|--------|---------|
| 400 | Validation failed (see `errors`) |
| 401 | Missing/invalid/expired credential |
| 403 | Authenticated but not allowed (ownership) |
| 404 | Resource not found |
| 409 | Conflict (e.g. username/email already taken) |

## Pagination

All list endpoints use cursor pagination:

```json
{ "items": [ ... ], "nextCursor": "opaque-string-or-null" }
```

Request the next page with `?cursor=<nextCursor>&limit=20` (default 20, max 50). `nextCursor: null`
means there are no more results. An invalid/garbled cursor returns an empty page, not an error.

---

The remaining endpoint reference (Users, Posts, Engagement, Notifications, Search, and the
SignalR notifications hub) is being completed alongside those features — see `contracts/openapi.yaml`
in the feature spec for the full, authoritative request/response shapes in the meantime, and
`/swagger` on the running instance for a live, executable reference.
