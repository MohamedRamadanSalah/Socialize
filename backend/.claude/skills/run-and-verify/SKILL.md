---
name: run-and-verify
description: How to run the Socialize backend stack and verify a change works end-to-end before moving on — docker compose up, Swagger, curl, and the auth flow. Use after implementing or modifying any endpoint to confirm it actually behaves correctly, not just that it compiles.
---

# Run & Verify the Backend

A change is not "done" until it has been exercised against a running server. Follow this
after any endpoint change.

## Run the full stack (preferred)

From `backend/`:

```bash
docker compose up --build
```

This starts:
- **api** — ASP.NET Core, auto-applies EF migrations on startup
- **postgres** — the database
- **pgadmin** (optional) — DB browsing UI

Health check: `GET http://localhost:8080/health` should return `200`.
Swagger UI: `http://localhost:8080/swagger`.

Stop with `Ctrl+C`; `docker compose down -v` to also wipe the DB volume for a clean slate.

## Run the API alone (fast inner loop)

Start only Postgres in Docker, then run the API on the host:

```bash
docker compose up -d postgres
dotnet run --project src/Socialize.Api
```

Use this when iterating on C# so you get fast rebuilds without rebuilding the image.

## Verify via Swagger

1. Open `/swagger`.
2. `POST /register` then `POST /login` — copy the `accessToken`.
3. Click **Authorize**, paste `Bearer <accessToken>`.
4. Exercise the endpoint you changed; confirm status code and response body.

## Verify via curl (auth flow example)

```bash
# register
curl -s -X POST http://localhost:8080/register \
  -H 'Content-Type: application/json' \
  -d '{"userName":"demo","email":"demo@x.io","password":"Passw0rd!"}'

# login -> capture accessToken
TOKEN=$(curl -s -X POST http://localhost:8080/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"demo@x.io","password":"Passw0rd!"}' | jq -r .accessToken)

# call a protected endpoint
curl -s http://localhost:8080/me -H "Authorization: Bearer $TOKEN"
```

## Verify SignalR notifications

- Connect a client to `/hubs/notifications?access_token=<jwt>` (token in query string —
  browsers can't set headers on the WS handshake).
- Trigger a like/comment/follow from a second user and confirm the recipient receives a
  live message, and that `GET /notifications` also lists it.

## What "verified" means

- [ ] Server builds and starts (no startup exceptions in logs).
- [ ] Happy path returns the right status code and body.
- [ ] Auth: protected route returns `401` without a token, works with one.
- [ ] Error path returns the correct code (`400` invalid input, `404` missing, `403`
      forbidden) — not `500`.
- [ ] If real-time is involved, the SignalR message actually arrives.

Report what you actually observed (status codes, sample response). If something failed,
say so with the log/output — do not claim success on a compile alone.
