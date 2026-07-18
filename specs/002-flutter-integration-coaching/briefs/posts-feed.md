## Slice 3: Posts & Feed

**Prerequisite**: Profiles & Social Graph (passed).
**Backend contract**: `backend/API.md` §"Posts & feed".

### Goal

Let a signed-in user create a post (text + up to 4 images), view a single post, edit their own post's text, delete their own post, browse a paginated home feed of people they follow, and view any user's own posts list.

### Backend Endpoints Involved

- `POST /api/posts` — multipart: `content` field + up to 4 `images` parts (~5MB each, JPEG/PNG/WebP only); `201` with the created `Post` (`id`, `author`, `content`, `imageUrls`, `likeCount`, `commentCount`, `likedByMe`, `createdAt`, `editedAt`).
- `GET /api/posts/{id}` — `200` Post, `404` if missing.
- `PATCH /api/posts/{id}` — body `{ "content": "..." }`; `200` Post with `editedAt` set; `403` if not the author.
- `DELETE /api/posts/{id}` — `204`; `403` if not the author.
- `GET /api/feed?cursor=&limit=20` — cursor-paginated posts from followed users, newest first; empty page (not an error) if following no one.
- `GET /api/users/{id}/posts?cursor=&limit=20` — cursor-paginated posts by a specific user.

### Required Architecture Layers

- **Domain**: a `Post` entity (content, author summary, image URLs, counts, `likedByMe`, timestamps, edited flag); a repository interface for "create post" (with local image file references), "get post by id", "edit post text", "delete post", "get feed page", "get user's posts page" — no HTTP/multipart detail in the interface.
- **Data**: the implementation handling multipart image upload encoding, JSON mapping to `Post`, and cursor-token passing for both paginated list calls.
- **Presentation**: a compose screen (text + up to 4 image picks, with client-side validation before ever hitting the network — count/size/type), a post-detail screen, an edit-text flow gated to the author, a delete action gated to the author, a home feed screen with infinite/paginated scroll, and a user-posts screen (reusable for any profile, including your own).

### Required Riverpod Providers

- A feed provider that supports cursor-based "load next page" and prepends nothing retroactively (newest-first order is the backend's job, not the client's).
- A user-posts provider parameterized by user id, independent from the feed provider (different list, different cursor state).
- A single-post provider/state for the detail screen (supports the edit-text action updating that same in-memory entry so the detail screen doesn't need a full reload after an edit).
- A create-post action provider handling the multipart submission and surfacing granular failure reasons (too many images, image too large, bad type) rather than one generic error.
- A delete-post action, restricted in the UI to the author (though the backend also enforces `403` — this is defense in depth, not a replacement for the server check).

### UI Flows

- Compose a post: enter text, attach 0–4 images, submit.
- View a single post (with its author and images).
- Edit your own post's text; see the "edited" indicator once saved.
- Delete your own post.
- Home feed: paginated, newest-first, from users you follow.
- A given user's posts list (reachable from their profile).

### Acceptance Criteria

1. Given an authenticated user, when they submit a post with text and up to 4 images, then the post is created with the images attached and retrievable by id (mirrors `001-social-media-backend/spec.md` Story 4, Scenario 1).
2. Given an existing post, when any authenticated user requests it by id, then the post with its author and images is returned (Scenario 2).
3. Given a user following one or more authors, when they page through the feed, then a bounded page of posts from followed users (newest first) is shown, with working "load more" (Scenario 3).
4. Given a user's own post, when they edit its text, then the update persists and an "edited" indicator appears; when a different user attempts to edit it, the app surfaces the resulting forbidden error rather than pretending it worked (Scenario 4).
5. Given a user's own post, when they delete it, it disappears from all lists; a different user's delete attempt is refused (Scenario 5).
6. Given a user id, when that user's posts are requested, they page correctly with cursor pagination (Scenario 6).
7. An image upload that exceeds 4 images, exceeds ~5MB, or is a disallowed type is rejected client-side with a specific, actionable message before any network call, and server-side rejection (if it still occurs) is surfaced clearly, never silently dropped.

### Definition of Done

- [ ] Create (with images), get, edit-text, delete, feed, and user-posts all work against the real backend.
- [ ] Domain layer has zero imports of Flutter, Riverpod, or HTTP/multipart packages.
- [ ] Image count/size/type validation happens before the network call, using the same limits the backend enforces (≤4 images, ~5MB each, JPEG/PNG/WebP).
- [ ] Feed and user-posts pagination are both driven by the opaque cursor the backend returns, never by client-side offset math.
- [ ] A non-author's edit/delete attempt is never hidden from the user as if it silently succeeded.
- [ ] This slice's presentation code does not directly import or depend on the Profiles slice's internal data-layer types — it may reuse an `author`/`UserSummary` shape from the shared domain, but not reach into another slice's data layer.

### Submitting This Slice

When you believe this slice is complete, tell the coach explicitly (e.g. "I'm done with Posts & Feed, please evaluate" or "submit for evaluation"). Casual questions with code attached are not graded; only an explicit submission is.
