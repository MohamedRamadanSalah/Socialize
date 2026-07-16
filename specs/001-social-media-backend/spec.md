# Feature Specification: Social Media Backend for Flutter Integration Training

**Feature Branch**: `001-social-media-backend`

**Created**: 2026-07-16

**Status**: Draft

**Input**: User description: "we need to create real backend to make the strong training flutter devs to integrate with it so use this file to understand the idea of the project and what we do PLAN.md"

## User Scenarios & Testing *(mandatory)*

This backend serves two audiences at once:

- **App end-users** — the people whose social behavior the API models (registering, posting, following, liking, commenting, receiving notifications, searching).
- **Flutter trainee developers** — the primary customers of this project, who build a mobile app against the backend and, in doing so, learn realistic client-integration skills (token refresh, real-time sockets, cursor pagination, file upload).

The user stories below are prioritized so that each one, delivered on its own, gives trainees a meaningful, independently testable slice to integrate against.

### User Story 1 - Account creation and secure session (Priority: P1)

A new person registers with a username, email, and password, then logs in and receives a short-lived access credential plus a longer-lived refresh credential. When the access credential expires, the client silently exchanges the refresh credential for a fresh pair without forcing the user to log in again; logging out invalidates the refresh credential.

**Why this priority**: Nothing else in the product is reachable without an authenticated session. It is also the single most valuable integration lesson for trainees — implementing a token-refresh interceptor is a core real-world client skill.

**Independent Test**: Register a user, log in, call a protected endpoint successfully, wait for (or simulate) access-credential expiry, confirm the client can refresh and continue, then log out and confirm the refresh credential no longer works.

**Acceptance Scenarios**:

1. **Given** no existing account, **When** a person registers with valid unique username/email and a password meeting the strength policy, **Then** the account is created and they can immediately log in.
2. **Given** a registered account, **When** they log in with correct credentials, **Then** they receive an access credential and a refresh credential.
3. **Given** an expired access credential and a valid refresh credential, **When** the client requests a refresh, **Then** a new access + refresh pair is issued and the previous refresh credential is invalidated (rotation).
4. **Given** a valid session, **When** the user logs out, **Then** the refresh credential can no longer be used to obtain new access.
5. **Given** a request to a protected endpoint without a valid access credential, **When** the request is made, **Then** it is rejected as unauthorized.

---

### User Story 2 - Self-documenting, ready-to-run integration target (Priority: P1)

A Flutter trainee clones the project and, with a single command, brings the entire backend online (API + database, schema applied, demo data loaded). They open an interactive API explorer in the browser, read a client-facing integration guide with request/response examples for every endpoint, and immediately start making calls against a system that already contains realistic users and posts.

**Why this priority**: The stated goal is a *strong training target*. A backend that is empty, undocumented, or hard to start fails that goal regardless of feature completeness. This story is what turns a correct API into an effective teaching tool.

**Independent Test**: On a clean machine, run the single start command; confirm the API becomes reachable, the interactive explorer lists all endpoints, the integration guide documents auth flow and every endpoint, and demo users/posts are queryable without any manual data entry.

**Acceptance Scenarios**:

1. **Given** a clean checkout, **When** the trainee runs the single start command, **Then** the API, database, schema, and demo data are all provisioned and the API responds to a health check.
2. **Given** the running backend, **When** the trainee opens the interactive API explorer, **Then** every endpoint is listed with its parameters, request bodies, and response shapes, and can be exercised from the browser.
3. **Given** the running backend, **When** the trainee reads the integration guide, **Then** it states the base address, describes the full authentication/refresh flow, and gives request/response examples for every endpoint.
4. **Given** a freshly started backend, **When** the trainee queries the feed or searches, **Then** demo users and posts are already present.

---

### User Story 3 - Profiles and social graph (Priority: P2)

An authenticated user views another user's public profile, updates their own profile (display name, bio, avatar image), and follows or unfollows other users. They can see who follows a user and who a user follows.

**Why this priority**: The social graph is the foundation for a personalized feed and for notifications. It is independently valuable — trainees can build profile screens and follow buttons against it — but it depends on authentication (P1).

**Independent Test**: As an authenticated user, fetch another profile by username, update your own profile and avatar, follow a user, confirm the relationship appears in both the follower and following lists, then unfollow and confirm it is removed.

**Acceptance Scenarios**:

1. **Given** an existing username, **When** any authenticated user requests that profile, **Then** the public profile details are returned.
2. **Given** an authenticated user, **When** they update their display name, bio, or avatar, **Then** the changes are persisted and reflected on their profile.
3. **Given** two distinct users, **When** one follows the other, **Then** the relationship is recorded and appears in the follower/following lists; following the same user twice does not create a duplicate.
4. **Given** an existing follow relationship, **When** the follower unfollows, **Then** the relationship is removed.
5. **Given** a user, **When** their followers or following list is requested, **Then** the list is returned using cursor-based pagination.

---

### User Story 4 - Posting and the feed (Priority: P2)

An authenticated user creates a post containing text and optionally one or more images, views a single post, deletes their own post, browses a paginated home feed, and views all posts authored by a specific user.

**Why this priority**: Content creation and consumption is the core of a social product and the richest integration surface for trainees (multipart image upload, cursor pagination, list/detail navigation). It depends on accounts (P1) and is enriched by the social graph (P3).

**Independent Test**: Create a text-plus-image post, retrieve it by id, page through the feed and confirm the new post appears, list the author's posts, then delete the post as its author and confirm it is gone; confirm a non-author cannot delete it.

**Acceptance Scenarios**:

1. **Given** an authenticated user, **When** they submit a post with text and image(s), **Then** the post is created with the images attached and is retrievable by id.
2. **Given** an existing post, **When** any authenticated user requests it by id, **Then** the post with its author and images is returned.
3. **Given** a populated feed, **When** a user requests it with a pagination cursor, **Then** a bounded page of posts is returned along with a cursor for the next page.
4. **Given** a user's own post, **When** they delete it, **Then** it is removed; **When** a different user attempts to delete it, **Then** the request is refused.
5. **Given** a user id, **When** that user's posts are requested, **Then** their posts are returned with cursor pagination.

---

### User Story 5 - Engagement: likes and comments (Priority: P3)

An authenticated user likes and unlikes a post, adds comments to a post, reads a post's comments, and deletes their own comment.

**Why this priority**: Engagement deepens the product and adds notification triggers, but the app is already demonstrable without it. It depends on posts (P4).

**Acceptance Scenarios**:

1. **Given** a post, **When** a user likes it, **Then** the like is recorded and the post's like count reflects it; liking again does not double-count.
2. **Given** a post the user has liked, **When** they unlike it, **Then** the like is removed and the count decreases.
3. **Given** a post, **When** a user adds a comment, **Then** the comment is stored and appears in that post's comment list (cursor-paginated).
4. **Given** a user's own comment, **When** they delete it, **Then** it is removed; a user cannot delete another user's comment.

---

### User Story 6 - Real-time notifications (Priority: P3)

When someone likes a user's post, comments on it, or follows the user, the recipient receives a notification. Notifications are persisted so they can be listed later, and are also delivered live to any currently connected client. The recipient can mark a single notification, or all notifications, as read.

**Why this priority**: Real-time delivery is a high-value, distinctive integration lesson (authenticated socket connection, live UI updates), but the product functions without it via the persisted list. It depends on engagement and the social graph.

**Acceptance Scenarios**:

1. **Given** user A has a post, **When** user B likes or comments on it, **Then** a notification for user A is created and pushed live to A's connected client if any.
2. **Given** user A, **When** user B follows A, **Then** A receives a follow notification.
3. **Given** an authenticated client, **When** it establishes a real-time connection with a valid access credential, **Then** it receives subsequent notifications live; an invalid or missing credential is rejected.
4. **Given** unread notifications, **When** the user marks one (or all) as read, **Then** their read state updates and the unread set shrinks accordingly.
5. **Given** a user, **When** they request their notifications, **Then** the list is returned with cursor pagination.

---

### User Story 7 - Search (Priority: P4)

An authenticated user searches for users by name/username and for posts by text content.

**Why this priority**: Search improves discoverability and rounds out the demo, but is the least critical to a first integration milestone.

**Acceptance Scenarios**:

1. **Given** existing users, **When** a user searches with a query string, **Then** matching users are returned with cursor pagination.
2. **Given** existing posts, **When** a user searches post content, **Then** matching posts are returned with cursor pagination.
3. **Given** a query that matches nothing, **When** the search runs, **Then** an empty result set is returned (not an error).

---

### Edge Cases

- Registration with an already-taken username or email is rejected with a clear, distinguishable error.
- A refresh credential that has been rotated, revoked (via logout), or expired cannot be used to obtain new access.
- A malformed or oversized image upload, or an unsupported file type, is rejected with a clear error rather than corrupting the post.
- Requesting a profile, post, comment, or notification that does not exist returns a clear not-found result.
- Acting on a resource you do not own (deleting another user's post/comment) is refused as forbidden.
- Following yourself, or liking/commenting on a deleted post, is handled gracefully.
- A pagination cursor that is invalid or points past the end returns an empty page rather than an error.
- A real-time client connecting without or with an expired credential is rejected and does not receive other users' notifications.
- Deleting a post removes its associated images, comments, and likes without leaving orphaned data.

## Requirements *(mandatory)*

### Functional Requirements

**Authentication & Sessions**
- **FR-001**: System MUST allow a person to register with a unique username, unique email, and a password, storing the password only in a non-reversible (hashed) form.
- **FR-002**: System MUST authenticate a registered user by credentials and issue a short-lived access credential and a longer-lived refresh credential on success.
- **FR-003**: System MUST allow a client to exchange a valid refresh credential for a new access + refresh pair, invalidating (rotating) the previously issued refresh credential.
- **FR-004**: System MUST allow a user to log out, after which their refresh credential can no longer be used.
- **FR-005**: System MUST reject any request to a protected resource that lacks a valid access credential.
- **FR-006**: System MUST expose the currently authenticated user's own account details.

**Profiles & Social Graph**
- **FR-007**: Users MUST be able to retrieve another user's public profile by username.
- **FR-008**: Users MUST be able to update their own display name, bio, and avatar image.
- **FR-009**: Users MUST be able to follow and unfollow other users, with duplicate follows prevented and self-follows disallowed.
- **FR-010**: System MUST return a user's followers and following lists with cursor-based pagination.

**Posts & Feed**
- **FR-011**: Users MUST be able to create a post with text and optionally one or more attached images.
- **FR-012**: Users MUST be able to retrieve a single post (with author and images) by its identifier.
- **FR-013**: Users MUST be able to delete their own posts, and MUST NOT be able to delete posts they do not own.
- **FR-014**: System MUST provide a paginated home feed and a paginated list of a given user's posts, both using cursor-based pagination.
- **FR-015**: System MUST accept image uploads, validate file type and size, and reject invalid uploads with a clear error.

**Engagement**
- **FR-016**: Users MUST be able to like and unlike a post, with likes de-duplicated per user/post.
- **FR-017**: Users MUST be able to add comments to a post and retrieve a post's comments with cursor-based pagination.
- **FR-018**: Users MUST be able to delete their own comments, and MUST NOT be able to delete others' comments.

**Notifications & Real-time**
- **FR-019**: System MUST create a persisted notification for the affected user when their post is liked or commented on, or when they are followed.
- **FR-020**: System MUST deliver notifications live to a recipient's currently connected client, in addition to persisting them.
- **FR-021**: System MUST authenticate real-time connections and reject connections without a valid access credential.
- **FR-022**: Users MUST be able to list their notifications (cursor-paginated) and mark a single notification or all notifications as read.

**Search**
- **FR-023**: Users MUST be able to search for users and for posts by text query, with cursor-paginated results and graceful empty results.

**Cross-cutting**
- **FR-024**: System MUST validate all incoming request data and return clear, structured, consistent error responses distinguishing validation, unauthorized, forbidden, and not-found conditions.
- **FR-025**: System MUST publish an interactive, browsable API description covering every endpoint's parameters, request bodies, and response shapes.
- **FR-026**: System MUST provide a client-facing integration guide documenting the base address, the full authentication/refresh flow, and request/response examples for every endpoint.
- **FR-027**: System MUST be startable with a single command that provisions the API and its data store, applies the schema, and loads demo seed data (demo users and posts).
- **FR-028**: System MUST expose a health check indicating the API and its data store are reachable.
- **FR-029**: System MUST record structured operational logs for requests and significant events to support debugging during training.

### Key Entities *(include if feature involves data)*

- **User**: A registered person. Key attributes: username, email, hashed password, display name, bio, avatar reference, creation time. Central to every other entity.
- **Refresh Credential**: A longer-lived token bound to a user, with an expiry and a revoked state; supports rotation and logout.
- **Post**: Text content authored by a user at a point in time, optionally with attached images.
- **Post Image**: An image attached to a post (reference/location + owning post).
- **Comment**: Text authored by a user on a specific post at a point in time.
- **Like**: A user's like of a specific post; unique per user/post pair.
- **Follow**: A directed relationship from a follower to a followee; unique per pair.
- **Notification**: A record that something concerning a recipient occurred, with an actor, a type (like/comment/follow), a reference to the related entity, a read/unread state, and a creation time.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A trainee can go from a clean checkout to a fully running backend with demo data using a single command in under 5 minutes, with no manual configuration.
- **SC-002**: 100% of endpoints are discoverable and executable from the interactive API explorer and documented with request/response examples in the integration guide.
- **SC-003**: A trainee can complete the full authenticated session lifecycle (register → log in → call a protected endpoint → refresh an expired credential → log out) end-to-end without manual token handling, following only the integration guide.
- **SC-004**: A liked/commented/followed action results in the recipient seeing a live notification within 2 seconds of the action, without refreshing.
- **SC-005**: Every list endpoint returns bounded pages and a working next-page cursor, with no unbounded responses regardless of data volume.
- **SC-006**: All ownership rules hold: a user can never delete or modify another user's post or comment, verified for 100% of ownership-guarded actions.
- **SC-007**: On first run, the feed and search return non-empty, realistic demo content without any manual data entry.
- **SC-008**: Every error condition (validation, unauthorized, forbidden, not-found) returns a distinct, consistent, machine-readable response that a client can branch on.

## Assumptions

- The **Flutter mobile app is out of scope**; this effort delivers only the backend that Flutter trainees integrate against. Wiring the Flutter side is a separate later effort.
- The training value comes from realistic, production-shaped patterns (secure token rotation, real-time delivery, cursor pagination, file upload, self-documentation) rather than from social scale — moderate data volumes and single-region operation are assumed.
- Password strength, credential lifetimes (short access, longer refresh), and rate/size limits follow common industry defaults suitable for a training environment rather than a hardened public deployment.
- Content moderation, private/blocked accounts, direct messaging, media transcoding, and email verification are out of scope for this version.
- Uploaded images are stored via a straightforward file-storage mechanism appropriate for local/training use; a managed cloud object store is not required.
- A single relational data store backs the system, provisioned alongside the API by the one-command start.
- Demo seed data contains enough users, posts, follows, and engagement to make the feed, search, and notifications meaningfully demonstrable on first run.
