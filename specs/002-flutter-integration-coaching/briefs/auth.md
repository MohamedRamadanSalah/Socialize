## Slice 1: Authentication & Sessions

**Prerequisite**: None — start here.
**Backend contract**: `backend/API.md` §"Authentication & refresh flow" (base URL, register/login/refresh/logout/me, error format).

### Goal

Let a person create an account, sign in, stay signed in silently across access-token expiry, see their own account details, and sign out — with the app never asking them to re-enter credentials just because 15 minutes passed.

### Backend Endpoints Involved

- `POST /api/auth/register` — create an account (`userName`, `email`, `password`); returns an `AuthResponse` (`accessToken`, 15-min JWT; `refreshToken`, 7-day opaque token) on success; a taken username/email returns a distinguishable conflict error.
- `POST /api/auth/login` — same `AuthResponse` shape as register, for an existing account.
- `POST /api/auth/refresh` — body `{ "refreshToken": "..." }`; returns a **new** `AuthResponse` with both tokens rotated; the old refresh token is revoked immediately — never reuse it.
- `POST /api/auth/logout` — requires `Authorization: Bearer <accessToken>`; body `{ "refreshToken": "..." }`; revokes that refresh token server-side.
- `GET /api/auth/me` — requires `Authorization: Bearer <accessToken>`; returns the caller's own account details.
- Every protected endpoint (i.e., everything except register/login) expects `Authorization: Bearer <accessToken>` and returns `401` if it's missing, malformed, or expired.

### Required Architecture Layers

- **Domain**: an `AuthSession` (or equivalent) entity/value type representing "am I signed in, as whom" — access token, refresh token, and the signed-in user's identity, independent of how it's stored or transported. A repository *interface* describing "authenticate", "refresh", "logout", "get current session" as operations, with no HTTP or storage detail in its signature.
- **Data**: an implementation of that repository interface that talks to the 5 endpoints above over HTTP, maps their JSON responses into the domain session type, and is responsible for *where* tokens are stored between app launches (e.g., secure/encrypted local storage) — this detail belongs entirely in this layer, never leaking into domain or presentation.
- **Presentation**: Riverpod provider(s) exposing the current auth state to the rest of the app (signed out / signed in / session expired), plus the register/login/logout screens and the logic that intercepts a `401` on any request and triggers a silent refresh-then-retry before giving up and forcing a real sign-out.

### Required Riverpod Providers

- Current auth/session state — a provider whose value can represent "signed out," "signed in as user X," and "in progress" (loading), not a plain boolean.
- Register action — something invokable from the register screen that reports success/failure without leaking HTTP details to the UI.
- Login action — same shape as register, for existing accounts.
- Logout action — invokable from anywhere the signed-in UI lives.
- The silent-refresh behavior does not need its own UI-facing provider, but the mechanism that intercepts a `401`, calls refresh, and retries the original request must live in the data layer (or a thin cross-cutting piece it owns) — not duplicated ad hoc at each call site.

### UI Flows

- Register screen (username, email, password → account created → signed in).
- Login screen (username/email + password → signed in).
- An authenticated "home"/placeholder screen reachable only when signed in (later slices fill this in — for this slice, it just needs to prove the session works, e.g. by showing the result of `GET /api/auth/me`).
- Logout action from wherever the signed-in UI lives.
- Automatic, invisible handling of an expired access token: the user should never see a random 401 or be dropped back to the login screen just because 15 minutes passed and a valid refresh token exists.

### Acceptance Criteria

1. Given no existing account, when a person registers with a valid unique username/email and a password meeting the strength policy, then the account is created and they are signed in immediately (mirrors `001-social-media-backend/spec.md` Story 1, Scenario 1).
2. Given a registered account, when they log in with correct credentials, then they receive an access token (15-minute lifetime) and a refresh token (7-day lifetime) and reach the signed-in state (Scenario 2).
3. Given an expired access token and a valid refresh token, when any protected request is attempted, then the client silently refreshes (new pair issued, old refresh token rotated/invalidated) and the original request succeeds without the user noticing (Scenario 3).
4. Given a signed-in session, when the user logs out, then the refresh token can no longer be used to obtain new access, and the app returns to a signed-out state (Scenario 4).
5. Given no valid access token, when a protected endpoint is called, then the request is rejected as unauthorized and the app reflects a signed-out state rather than crashing or hanging (Scenario 5).
6. Registration with an already-taken username or email surfaces a clear, distinguishable error to the user (not a generic failure).

### Definition of Done

- [ ] Register, login, silent refresh, logout, and "get my account" all work against the real running backend (not a mock).
- [ ] Domain layer has zero imports of Flutter, Riverpod, or HTTP packages.
- [ ] Exactly one place in the app performs the HTTP calls for these 5 endpoints (the data-layer repository implementation) — no controller/widget calls `http`/`dio` directly.
- [ ] A 401 on any protected call triggers silent refresh-and-retry at most once before falling back to a real sign-out.
- [ ] The refresh token is never held only in memory in a way that loses the session on a simple app restart, nor stored insecurely in a way that's trivially readable.
- [ ] Auth state is modeled as more than a boolean (loading / signed-out / signed-in-as-X is distinguishable).
- [ ] A duplicate-username/email registration attempt shows a specific, user-readable error distinct from a generic failure message.

### Submitting This Slice

When you believe this slice is complete, tell the coach explicitly (e.g. "I'm done with Authentication, please evaluate" or "submit for evaluation"). Casual questions with code attached are not graded; only an explicit submission is.
