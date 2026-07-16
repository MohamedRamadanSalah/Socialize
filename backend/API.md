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

## Users

### Get a profile

```
GET /api/users/{username}
```

Response `200 OK`:

```json
{
  "id": "b3f1c2a4-...",
  "userName": "alice",
  "displayName": "Alice Anderson",
  "bio": "Flutter trainee exploring the Socialize API.",
  "avatarUrl": null,
  "followerCount": 3,
  "followingCount": 2,
  "isFollowedByMe": false,
  "createdAt": "2026-06-16T14:07:00Z"
}
```

### Update my profile

```
PUT /api/users/me
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "displayName": "Alice A.", "bio": "Updated bio" }
```

Response `200 OK`: a `UserProfile` (fields omitted from the body are left unchanged).

### Upload my avatar

```
POST /api/users/me/avatar
Authorization: Bearer <accessToken>
Content-Type: multipart/form-data; boundary=...

--...
Content-Disposition: form-data; name="file"; filename="avatar.png"
Content-Type: image/png

<binary>
--...--
```

Response `200 OK`: a `UserProfile` with `avatarUrl` set. `400` if the file exceeds 5MB or is not
JPEG/PNG/WebP.

### Follow / unfollow

```
POST   /api/users/{id}/follow      -> 204 (idempotent; self-follow -> 400)
DELETE /api/users/{id}/follow      -> 204 (idempotent)
```

### Followers / following (cursor-paginated)

```
GET /api/users/{id}/followers?cursor=<opaque>&limit=20
GET /api/users/{id}/following?cursor=<opaque>&limit=20
```

Response `200 OK`:

```json
{
  "items": [
    { "id": "...", "userName": "bob", "displayName": "Bob Baker", "avatarUrl": null }
  ],
  "nextCursor": null
}
```

## Posts & feed

### Create a post

```
POST /api/posts
Authorization: Bearer <accessToken>
Content-Type: multipart/form-data; boundary=...

--...
Content-Disposition: form-data; name="content"

Hello from the training backend!
--...
Content-Disposition: form-data; name="images"; filename="a.jpg"
Content-Type: image/jpeg

<binary>
--...--
```

Up to 4 `images` parts accepted, ~5MB each, JPEG/PNG/WebP only. Response `201 Created`:

```json
{
  "id": "6f2e...",
  "author": { "id": "...", "userName": "alice", "displayName": "Alice Anderson", "avatarUrl": null },
  "content": "Hello from the training backend!",
  "imageUrls": ["/uploads/posts/9c1a....jpg"],
  "likeCount": 0,
  "commentCount": 0,
  "likedByMe": false,
  "createdAt": "2026-07-16T14:10:00Z",
  "editedAt": null
}
```

### Get / edit / delete a post

```
GET    /api/posts/{id}                       -> 200 Post, 404 if missing
PATCH  /api/posts/{id}   { "content": "..." } -> 200 Post (editedAt set); 403 if not the author
DELETE /api/posts/{id}                        -> 204; 403 if not the author
```

### Feed (posts from users you follow, newest first)

```
GET /api/feed?cursor=<opaque>&limit=20
```

Response `200 OK`: `{ "items": [Post, ...], "nextCursor": "..." | null }`. A user who follows no
one gets `{ "items": [], "nextCursor": null }`.

### A user's posts

```
GET /api/users/{id}/posts?cursor=<opaque>&limit=20
```

## Engagement

```
POST   /api/posts/{id}/like              -> 204 (idempotent)
DELETE /api/posts/{id}/like              -> 204 (idempotent)
POST   /api/posts/{id}/comments  { "content": "..." } -> 201 Comment
GET    /api/posts/{id}/comments?cursor=&limit=20      -> 200 CursorPage<Comment>
DELETE /api/comments/{id}                             -> 204; 403 if not the comment's author
```

Comments cannot be edited once posted — there is no edit endpoint for comments.

```json
// Comment shape
{
  "id": "...",
  "postId": "...",
  "author": { "id": "...", "userName": "bob", "displayName": "Bob Baker", "avatarUrl": null },
  "content": "Nice post!",
  "createdAt": "2026-07-16T14:12:00Z"
}
```

## Notifications & real-time

```
GET  /api/notifications?cursor=&limit=20   -> 200 CursorPage<Notification>
POST /api/notifications/{id}/read          -> 204
POST /api/notifications/read-all           -> 204
```

```json
// Notification shape
{
  "id": "...",
  "type": "Like",
  "actor": { "id": "...", "userName": "bob", "displayName": "Bob Baker", "avatarUrl": null },
  "entityId": "6f2e...",
  "isRead": false,
  "createdAt": "2026-07-16T14:11:00Z"
}
```

### Real-time notifications hub (SignalR)

- URL: `ws(s)://<host>/hubs/notifications?access_token=<accessToken>`
- SignalR/WebSockets can't set an `Authorization` header on the handshake, so the JWT access
  token is passed via the `access_token` query parameter instead.
- Once connected, the client receives a `ReceiveNotification` invocation (payload = the
  `Notification` shape above) whenever someone likes/comments on your post or follows you —
  typically within 2 seconds of the triggering action.
- A connection without a valid, unexpired access token is rejected.

Flutter client sketch (using `signalr_netcore` or similar):

```dart
final connection = HubConnectionBuilder()
    .withUrl('$baseUrl/hubs/notifications?access_token=$accessToken')
    .build();
connection.on('ReceiveNotification', (args) { /* update UI */ });
await connection.start();
```

## Search

```
GET /api/search/users?q=alice&cursor=&limit=20   -> 200 CursorPage<UserSummary>, relevance-ranked
GET /api/search/posts?q=training&cursor=&limit=20 -> 200 CursorPage<Post>, relevance-ranked
```

A query that matches nothing returns `{ "items": [], "nextCursor": null }`, not an error.

## Full endpoint index

| Area | Endpoints |
|---|---|
| System | `GET /health` |
| Auth | `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/refresh`, `POST /api/auth/logout`, `GET /api/auth/me` |
| Users | `GET /api/users/{username}`, `PUT /api/users/me`, `POST /api/users/me/avatar`, `POST\|DELETE /api/users/{id}/follow`, `GET /api/users/{id}/followers`, `GET /api/users/{id}/following` |
| Posts | `POST /api/posts`, `GET\|PATCH\|DELETE /api/posts/{id}`, `GET /api/feed`, `GET /api/users/{id}/posts` |
| Engagement | `POST\|DELETE /api/posts/{id}/like`, `POST\|GET /api/posts/{id}/comments`, `DELETE /api/comments/{id}` |
| Notifications | `GET /api/notifications`, `POST /api/notifications/{id}/read`, `POST /api/notifications/read-all`, SignalR `/hubs/notifications` |
| Search | `GET /api/search/users`, `GET /api/search/posts` |

For the fully typed request/response schemas, open `/swagger` on the running instance.
