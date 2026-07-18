## Slice 4: Engagement (Likes & Comments)

**Prerequisite**: Posts & Feed (passed).
**Backend contract**: `backend/API.md` §"Engagement".

### Goal

Let a signed-in user like/unlike a post, add a comment, read a post's comments, and delete their own comment — comments are immutable once posted (there is no edit endpoint for them).

### Backend Endpoints Involved

- `POST /api/posts/{id}/like` / `DELETE /api/posts/{id}/like` — idempotent; `204`.
- `POST /api/posts/{id}/comments` — body `{ "content": "..." }`; `201` with the created `Comment` (`id`, `postId`, `author`, `content`, `createdAt`).
- `GET /api/posts/{id}/comments?cursor=&limit=20` — cursor-paginated comments.
- `DELETE /api/comments/{id}` — `204`; `403` if not the comment's author.

### Required Architecture Layers

- **Domain**: a `Comment` entity; the `Post` entity from Slice 3 gains no new fields here (its `likeCount`/`likedByMe`/`commentCount` already exist) but this slice's repository interface adds "like/unlike", "add comment", "get comments page", "delete comment" as operations, independent of HTTP.
- **Data**: the implementation for the 5 endpoints above, mapping JSON to `Comment`, and reconciling the like/comment counts on the in-memory `Post` after a successful action so the UI doesn't need a full post refetch just to reflect one new like.
- **Presentation**: a like/unlike control on post cards and the post-detail screen, a comment composer and comment list on the post-detail screen (cursor-paginated), and a delete-comment action gated to the comment's author.

### Required Riverpod Providers

- A like/unlike action that updates the relevant post's `likeCount`/`likedByMe` wherever that post is currently displayed (feed, user-posts, detail) without requiring a manual page refresh — this is the main state-consistency challenge of this slice.
- A comments-for-post provider, cursor-paginated, scoped per post id (not a single global comment list).
- An add-comment action that appends the new comment to the current list optimistically or via refetch, consistently either way.
- A delete-comment action restricted to the comment's own author in the UI (in addition to the backend's own `403`).

### UI Flows

- Like / unlike a post from anywhere it's shown (feed, user-posts list, detail screen).
- View a post's comments, paginated.
- Add a comment to a post.
- Delete your own comment.

### Acceptance Criteria

1. Given a post, when a user likes it, the like is recorded and the like count reflects it; liking again does not double-count (mirrors `001-social-media-backend/spec.md` Story 5, Scenario 1).
2. Given a post the user has liked, when they unlike it, the like is removed and the count decreases (Scenario 2).
3. Given a post, when a user adds a comment, it's stored and appears in that post's comment list, cursor-paginated (Scenario 3).
4. Given a user's own comment, when they delete it, it's removed; a different user's delete attempt on someone else's comment is refused (Scenario 4).
5. Liking or commenting on a deleted post is handled gracefully (a clear response, not a crash or silent no-op that looks like success).

### Definition of Done

- [ ] Like/unlike, add comment, list comments, and delete comment all work against the real backend.
- [ ] Domain layer has zero imports of Flutter, Riverpod, or HTTP packages.
- [ ] A like/unlike action reflects consistently everywhere the affected post is currently rendered — no stale count in one screen while another shows the update.
- [ ] Comment pagination is cursor-driven, scoped to the specific post, not a shared/global list.
- [ ] There is no "edit comment" UI or capability anywhere — comments are immutable by design, matching the backend.
- [ ] This slice reuses the `Post` entity from Slice 3 rather than redefining a parallel post model, and does not reach into Slice 3's data-layer implementation directly (only through Slice 3's own repository interface/domain types).

### Submitting This Slice

When you believe this slice is complete, tell the coach explicitly (e.g. "I'm done with Engagement, please evaluate" or "submit for evaluation"). Casual questions with code attached are not graded; only an explicit submission is.
