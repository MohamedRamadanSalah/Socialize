## Slice 2: Profiles & Social Graph

**Prerequisite**: Authentication & Sessions (passed).
**Backend contract**: `backend/API.md` §"Users".

### Goal

Let a signed-in user view any user's public profile, edit their own display name/bio/avatar, and follow/unfollow other users — with followers/following lists that page correctly no matter how large they get.

### Backend Endpoints Involved

- `GET /api/users/{username}` — public profile (`id`, `userName`, `displayName`, `bio`, `avatarUrl`, `followerCount`, `followingCount`, `isFollowedByMe`, `createdAt`).
- `PUT /api/users/me` — update own `displayName`/`bio` (requires auth); fields omitted are left unchanged.
- `POST /api/users/me/avatar` — multipart upload of a single avatar image (requires auth); `400` if over ~5MB or not JPEG/PNG/WebP.
- `POST /api/users/{id}/follow` / `DELETE /api/users/{id}/follow` — idempotent follow/unfollow; self-follow returns `400`.
- `GET /api/users/{id}/followers` / `GET /api/users/{id}/following` — cursor-paginated (`cursor`, `limit`), each item a `UserSummary`.

### Required Architecture Layers

- **Domain**: a `UserProfile` entity and a `UserSummary` (lighter shape for list items); a repository interface for "get profile by username", "update my profile", "upload avatar", "follow/unfollow", "get followers/following page" — expressed with no HTTP/multipart detail.
- **Data**: the implementation talking to the 6 endpoints above, mapping JSON to `UserProfile`/`UserSummary`, and handling multipart encoding for the avatar upload entirely inside this layer.
- **Presentation**: a profile screen (any user, by username), an edit-profile screen (self only), a follow/unfollow action available from a profile screen, and followers/following list screens using cursor pagination (load next page as the user scrolls, not all at once).

### Required Riverpod Providers

- A profile-by-username provider whose identity is parameterized by the username being viewed (not a single global "the profile").
- An edit-profile action provider for the signed-in user's own updates (name/bio/avatar), separate from the read-only profile provider above.
- A follow/unfollow action, with the calling screen's profile view reflecting the new `isFollowedByMe`/count without requiring a manual refresh.
- Paginated providers for followers/following that can fetch "next page" and append rather than reload the whole list from page 1.

### UI Flows

- View any user's profile (including your own) by username.
- Edit your own display name, bio, and avatar.
- Follow / unfollow a user from their profile.
- Browse a user's followers list and following list, each with working "load more" as you scroll.

### Acceptance Criteria

1. Given an existing username, when any authenticated user requests that profile, then the public profile details are returned and rendered (mirrors `001-social-media-backend/spec.md` Story 3, Scenario 1).
2. Given the signed-in user, when they update display name, bio, or avatar, then the change is persisted and immediately reflected on their own profile view (Scenario 2).
3. Given two distinct users, when one follows the other, then the relationship shows up in both the follower and following lists; following the same user twice does not create a duplicate or a visible error (Scenario 3).
4. Given an existing follow relationship, when the follower unfollows, then it disappears from both lists (Scenario 4).
5. Given a user with many followers/following, when the list is opened, then it loads as a bounded, cursor-paginated page with working "load more," not an unbounded single fetch (Scenario 5).
6. Attempting to follow yourself is handled gracefully (a clear response, not a crash).

### Definition of Done

- [ ] Viewing any profile, editing your own, following/unfollowing, and paging followers/following all work against the real backend.
- [ ] Domain layer has zero imports of Flutter, Riverpod, or HTTP/multipart packages.
- [ ] The avatar upload's multipart encoding lives only in the data layer.
- [ ] Follow/unfollow updates the UI optimistically or via refetch — but never leaves the UI showing a stale `isFollowedByMe`/count after the action completes.
- [ ] Followers/following screens never fetch "all pages at once" — pagination is driven by the cursor, one page at a time.
- [ ] This slice's code lives in its own feature folder, independent of the Authentication slice's domain/data/presentation code (only consuming the current-session state from Slice 1 where needed for "am I signed in").

### Submitting This Slice

When you believe this slice is complete, tell the coach explicitly (e.g. "I'm done with Profiles & Social Graph, please evaluate" or "submit for evaluation"). Casual questions with code attached are not graded; only an explicit submission is.
