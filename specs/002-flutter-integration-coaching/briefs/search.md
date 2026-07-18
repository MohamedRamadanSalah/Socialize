## Slice 6: Search

**Prerequisite**: Notifications & Real-time (passed).
**Backend contract**: `backend/API.md` §"Search".

### Goal

Let a signed-in user search for users by name/username and for posts by text content, seeing relevance-ranked, paginated results — with a no-match query returning an empty result, not an error.

### Backend Endpoints Involved

- `GET /api/search/users?q=...&cursor=&limit=20` — cursor-paginated, relevance-ranked `UserSummary` results.
- `GET /api/search/posts?q=...&cursor=&limit=20` — cursor-paginated, relevance-ranked `Post` results.
- A query matching nothing returns `{ "items": [], "nextCursor": null }` — this is a normal, successful response, not an error to special-case.

### Required Architecture Layers

- **Domain**: this slice does not need new entities — it reuses `UserSummary` (from Slice 2) and `Post` (from Slice 3) — but adds a repository interface (or extends an existing one) for "search users by query" and "search posts by query", each cursor-paginated, independent of HTTP.
- **Data**: the implementation for the 2 endpoints above, mapping results into the existing `UserSummary`/`Post` domain types (no parallel "search result" types needed).
- **Presentation**: a search screen with a query input, tabs or a toggle between "users" and "posts" results, each cursor-paginated, and an explicit (not blank/broken-looking) empty state when a query matches nothing.

### Required Riverpod Providers

- A user-search provider parameterized by the current query string, debounced so it doesn't fire a request on every keystroke, cursor-paginated for its results.
- A post-search provider, same shape, independent state from the user-search provider (a query can be "in progress" for one and "done" for the other).
- Both providers should cleanly represent "no query entered yet," "searching," "results" (possibly empty), and "error" as distinct states — an empty-results state must not look identical to a loading or broken state to the user.

### UI Flows

- Enter a search query.
- View relevance-ranked, paginated user results.
- View relevance-ranked, paginated post results.
- See a clear, deliberate "no results" state when a query matches nothing.

### Acceptance Criteria

1. Given existing users, when a user searches with a query string, matching users are returned relevance-ranked with cursor pagination (mirrors `001-social-media-backend/spec.md` Story 7, Scenario 1).
2. Given existing posts, when a user searches post content, matching posts are returned relevance-ranked with cursor pagination (Scenario 2).
3. Given a query that matches nothing, the search runs and an empty result set is shown via a deliberate empty state — not an error message and not a loading spinner stuck forever (Scenario 3).

### Definition of Done

- [ ] User search and post search both work against the real backend, each independently paginated.
- [ ] Domain layer has zero imports of Flutter, Riverpod, or HTTP packages.
- [ ] Query input is debounced — the app does not fire a network request on every single keystroke.
- [ ] This slice reuses `UserSummary` and `Post` from earlier slices rather than introducing duplicate "search result" domain types.
- [ ] The three distinct states — no query yet, loading, empty results, and results present — are all visually distinguishable to the user.

### Submitting This Slice

When you believe this slice is complete, tell the coach explicitly (e.g. "I'm done with Search, please evaluate" or "submit for evaluation"). Casual questions with code attached are not graded; only an explicit submission is. Completing this slice completes the full 6-slice program.
