# Phase 1 Data Model: Social Media Backend

**Feature**: 001-social-media-backend | **Date**: 2026-07-16

Derived from the spec's Key Entities and Functional Requirements. Types are conceptual; concrete EF Core mappings live in `Socialize.Infrastructure/Persistence/Configurations`. All timestamps are UTC, stored as `timestamptz`, serialized as ISO-8601 (e.g. `2026-07-16T14:07:00Z`). All surrogate keys are `Guid` (v4) unless noted.

## Entity: User

| Field | Type | Constraints |
|-------|------|-------------|
| Id | Guid | PK |
| UserName | string(3–30) | Required, **unique** (case-insensitive), `[a-zA-Z0-9_]` |
| Email | string | Required, **unique** (case-insensitive), valid email |
| PasswordHash | string | Required (BCrypt hash); never serialized |
| DisplayName | string(1–50) | Required |
| Bio | string(0–300) | Optional |
| AvatarUrl | string | Optional (relative URL to stored image) |
| CreatedAt | timestamptz | Required, default now |
| SearchVector | tsvector | Generated from UserName + DisplayName; **GIN** index (R10) |

- **Relationships**: 1‑to‑many → Post (as author), Comment (as author), RefreshToken; many‑to‑many self via Follow; 1‑to‑many → Notification (as recipient and as actor).
- **Rules**: FR-001 (hashed password), FR-007 (lookup by username), FR-008 (update DisplayName/Bio/AvatarUrl).

## Entity: RefreshToken

| Field | Type | Constraints |
|-------|------|-------------|
| Id | Guid | PK |
| UserId | Guid | FK → User, required, indexed |
| TokenHash | string | Required (SHA-256 of the opaque token); unique |
| ExpiresAt | timestamptz | Required (issued + 7 days) |
| CreatedAt | timestamptz | Required |
| RevokedAt | timestamptz? | Null until rotated/logged-out |
| ReplacedByTokenHash | string? | Set on rotation (audit chain, R4) |

- **Rules**: FR-002/003/004. Valid = `RevokedAt is null` AND `ExpiresAt > now`. Rotation sets `RevokedAt` + `ReplacedByTokenHash` and inserts a new row. Logout sets `RevokedAt`.
- **State**: `Active → Revoked` (one-way). Expiry is time-based, not a stored transition.

## Entity: Post

| Field | Type | Constraints |
|-------|------|-------------|
| Id | Guid | PK |
| AuthorId | Guid | FK → User, required, indexed |
| Content | string(1–2000) | Required |
| CreatedAt | timestamptz | Required, default now; index `(CreatedAt DESC, Id DESC)` for keyset |
| EditedAt | timestamptz? | Null until edited; non-null flags "edited" (FR-030) |
| SearchVector | tsvector | Generated from Content; **GIN** index (R10) |

- **Relationships**: many‑to‑1 → User (author); 1‑to‑many → PostImage, Comment, Like (all cascade-delete with the post, per Edge Cases).
- **Rules**: FR-011 (create with text+images), FR-012 (get with author+images), FR-013 (owner delete), FR-030 (owner edit text; set `EditedAt`), FR-014 (feed = posts by followed authors).

## Entity: PostImage

| Field | Type | Constraints |
|-------|------|-------------|
| Id | Guid | PK |
| PostId | Guid | FK → Post, required, indexed; cascade delete |
| Url | string | Required (relative URL to stored file) |
| Order | int | 0-based position within the post |

- **Rules**: FR-015 / SC-009 — at most **4** per post; each source file ≤ **5 MB**; type ∈ {JPEG, PNG, WebP}. Validation happens at upload before rows are created.

## Entity: Comment

| Field | Type | Constraints |
|-------|------|-------------|
| Id | Guid | PK |
| PostId | Guid | FK → Post, required, indexed; cascade delete |
| AuthorId | Guid | FK → User, required, indexed |
| Content | string(1–1000) | Required |
| CreatedAt | timestamptz | Required, default now; index `(PostId, CreatedAt DESC, Id DESC)` |

- **Rules**: FR-017 (add + list paginated), FR-018 (owner delete only). **Immutable once posted** — no edit (clarification Q5).

## Entity: Like

| Field | Type | Constraints |
|-------|------|-------------|
| UserId | Guid | FK → User, part of composite PK |
| PostId | Guid | FK → Post, part of composite PK; cascade delete |
| CreatedAt | timestamptz | Required, default now |

- **Key**: composite PK `(UserId, PostId)` — enforces de-duplication (FR-016). Unlike = delete the row.

## Entity: Follow

| Field | Type | Constraints |
|-------|------|-------------|
| FollowerId | Guid | FK → User, part of composite PK |
| FolloweeId | Guid | FK → User, part of composite PK |
| CreatedAt | timestamptz | Required, default now |

- **Key**: composite PK `(FollowerId, FolloweeId)` — prevents duplicate follows (FR-009).
- **Rules**: `FollowerId != FolloweeId` (no self-follow). Drives feed (R6) and follower/following lists (FR-010). Both FKs use `Restrict`/no-cascade to avoid multiple-cascade-path issues; rows removed explicitly.

## Entity: Notification

| Field | Type | Constraints |
|-------|------|-------------|
| Id | Guid | PK |
| RecipientId | Guid | FK → User, required, indexed `(RecipientId, CreatedAt DESC, Id DESC)` |
| ActorId | Guid | FK → User, required (who triggered it) |
| Type | enum NotificationType | Required: `Like`, `Comment`, `Follow` |
| EntityId | Guid? | Related entity id (PostId for like/comment; ActorId context for follow) |
| IsRead | bool | Default false |
| CreatedAt | timestamptz | Required, default now |

- **Rules**: FR-019 (created on like/comment/follow), FR-020 (also pushed live), FR-022 (list paginated; mark one / mark all read). Self-actions (e.g., liking your own post) should not create a notification.
- **State**: `Unread → Read` (via mark-read / mark-all-read).

## Enum: NotificationType

`Like` | `Comment` | `Follow`

## Cross-cutting value objects (not persisted)

- **CursorPage\<T\>**: `{ items: T[], nextCursor: string | null }` — response envelope for all list endpoints (R5).
- **Cursor**: opaque base64 encoding of `(CreatedAt, Id)` of the last returned item.
- **ProblemDetails**: RFC 7807 error body `{ type, title, status, detail, errors? }` (R11).

## Referential integrity & cascade summary

- Delete User → out of scope (no delete-account endpoint in spec); FKs restrict.
- Delete Post → cascade delete its PostImages, Comments, Likes; associated stored image files removed by the delete handler (Edge Cases).
- Delete Comment → removes the single row.
- Unlike / Unfollow → row deletes; no cascade beyond the join row.

## Index summary (performance-relevant)

- `Users.UserName` unique (ci), `Users.Email` unique (ci), `Users.SearchVector` GIN.
- `Posts (CreatedAt DESC, Id DESC)`, `Posts.AuthorId`, `Posts.SearchVector` GIN.
- `Comments (PostId, CreatedAt DESC, Id DESC)`.
- `Notifications (RecipientId, CreatedAt DESC, Id DESC)`.
- `Follows (FolloweeId)` secondary index for follower-list queries (PK covers follower-side).
- `RefreshTokens.TokenHash` unique, `RefreshTokens.UserId`.
