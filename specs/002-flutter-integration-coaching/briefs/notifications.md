## Slice 5: Notifications & Real-time

**Prerequisite**: Engagement (passed).
**Backend contract**: `backend/API.md` §"Notifications & real-time".

### Goal

Let a signed-in user see a persisted list of notifications (someone liked/commented/followed them), mark one or all as read, and receive new notifications live while the app is open — without polling.

### Backend Endpoints Involved

- `GET /api/notifications?cursor=&limit=20` — cursor-paginated `Notification` list (`id`, `type` — Like/Comment/Follow, `actor`, `entityId`, `isRead`, `createdAt`).
- `POST /api/notifications/{id}/read` — mark one as read; `204`.
- `POST /api/notifications/read-all` — mark all as read; `204`.
- Real-time hub: `ws(s)://<host>/hubs/notifications?access_token=<accessToken>` (SignalR). The access token goes in the query string, not an `Authorization` header, because the WebSocket handshake can't carry custom headers. Once connected, the client receives a `ReceiveNotification` push (same `Notification` shape) within ~2 seconds of the triggering action. A connection with a missing or expired access token is rejected.

### Required Architecture Layers

- **Domain**: a `Notification` entity (type, actor, related entity id, read state, timestamp); a repository interface for "get notifications page", "mark one read", "mark all read"; and a **separate** domain-level interface for the real-time source (e.g. `NotificationsRealtimeSource`) exposing "connect", "disconnect", and a stream of incoming notifications — with no SignalR-specific type appearing in this interface's signature.
- **Data**: the REST repository implementation for the 3 HTTP endpoints, and a `NotificationsRealtimeSource` implementation wrapping a SignalR client package (e.g. `signalr_netcore`) that reconnects the access token via the query string and maps each `ReceiveNotification` payload into the same domain `Notification` type the REST list uses.
- **Presentation**: a notifications list screen (cursor-paginated, unread state visually distinct), a mark-one-read and mark-all-read action, and a live-update mechanism that prepends newly-arrived notifications to the list (or updates an unread badge) while the app is open and connected.

### Required Riverpod Providers

- A notifications-list provider, cursor-paginated, that also merges in live pushes so a newly-arrived notification appears without the user manually refreshing.
- A connection-lifecycle provider (or equivalent) that opens the real-time connection once signed in and tears it down on sign-out — this must depend on the auth-session state from Slice 1 rather than duplicating "am I signed in" logic.
- Mark-one-read and mark-all-read actions that update local unread state immediately, consistent with the server call's result.

### UI Flows

- Notifications list, cursor-paginated, unread items visually distinct.
- Mark a single notification as read (e.g. by tapping it).
- Mark all as read.
- A live-arriving notification appears (or an unread badge updates) without the user pulling to refresh, while the app is open.

### Acceptance Criteria

1. Given user A has a post, when user B likes or comments on it, user A gets a notification, and — if connected — it arrives live, typically within about 2 seconds (mirrors `001-social-media-backend/spec.md` Story 6, Scenario 1).
2. Given user A, when user B follows them, user A receives a follow notification the same way (Scenario 2).
3. Given a valid access token, when the client connects to the real-time hub, it receives subsequent notifications live; an invalid/missing/expired token is rejected at connection time (Scenario 3).
4. Given unread notifications, when the user marks one (or all) as read, the read state updates and the visible unread indicator shrinks accordingly (Scenario 4).
5. Given a user, when they request their notifications, the list is returned with cursor pagination (Scenario 5).
6. A dropped real-time connection (e.g. brief network loss) does not permanently break future live updates — some form of recovery (reconnect, or at least a clean resync via the REST list) is in place, even if simple.

### Definition of Done

- [ ] Notification list (paginated), mark-one-read, mark-all-read, and live delivery all work against the real backend and real-time hub.
- [ ] Domain layer has zero imports of Flutter, Riverpod, HTTP, or SignalR-specific packages — the real-time source is abstracted behind a domain interface.
- [ ] The real-time connection is opened/closed in step with the auth session from Slice 1 (connected while signed in, torn down on sign-out) rather than managed as an unrelated global singleton.
- [ ] A live-pushed notification and the REST-fetched list converge on one consistent unread count — no double-counting or drift between the two sources.
- [ ] This slice's domain/data code for the real-time source is swappable (a different transport could replace `signalr_netcore` later without presentation-layer changes) because it's hidden behind the `NotificationsRealtimeSource` interface.

### Submitting This Slice

When you believe this slice is complete, tell the coach explicitly (e.g. "I'm done with Notifications, please evaluate" or "submit for evaluation"). Casual questions with code attached are not graded; only an explicit submission is.
