---
name: ef-migrations
description: How to create and apply EF Core migrations for the Socialize backend using the pinned local dotnet-ef 8.x tool against PostgreSQL. Use whenever a Domain entity or its EF configuration changes and the database schema must be updated. Avoids the global dotnet-ef v10 vs .NET 8 SDK mismatch.
---

# EF Core Migrations

The solution targets **.NET 8 / EF Core 8**, but the machine's global `dotnet-ef` tool
is **v10**. To avoid a tool/runtime mismatch, always use the **local tool manifest**
pinned to `dotnet-ef` 8.x.

## One-time setup (per clone)

From the `backend/` folder:

```bash
dotnet new tool-manifest        # creates .config/dotnet-tools.json (if missing)
dotnet tool install dotnet-ef --version 8.*
dotnet tool restore             # restores the pinned tool
```

After this, always invoke migrations with `dotnet ef` — it resolves to the pinned local
8.x tool, not the global v10.

## Project layout for EF commands

- Migrations live in **Socialize.Infrastructure** (the DbContext project).
- The startup project is **Socialize.Api** (holds the connection string / DI).

So every command uses both flags:

```
--project src/Socialize.Infrastructure
--startup-project src/Socialize.Api
```

## Add a migration

After changing a Domain entity and/or its `IEntityTypeConfiguration<T>`:

```bash
dotnet ef migrations add <DescriptiveName> \
  --project src/Socialize.Infrastructure \
  --startup-project src/Socialize.Api \
  --output-dir Persistence/Migrations
```

Name migrations descriptively: `AddPosts`, `AddFollowRelationship`, `AddNotificationRead`.

## Apply migrations

Local dev (direct):

```bash
dotnet ef database update \
  --project src/Socialize.Infrastructure \
  --startup-project src/Socialize.Api
```

Docker: the API is configured to **auto-apply migrations on startup**, so
`docker compose up` brings the schema current automatically. Prefer that for the full
stack; use `database update` only when running the API outside Docker.

## Review before committing

- Open the generated migration and confirm it does what you intended — no accidental
  table drops or column renames that lose data.
- A rename may appear as drop+add. If so, hand-edit to use `RenameColumn`/`RenameTable`
  to preserve data.

## Undo the last (unapplied) migration

```bash
dotnet ef migrations remove \
  --project src/Socialize.Infrastructure \
  --startup-project src/Socialize.Api
```

Only remove a migration that has NOT been applied to a shared database.

## Connection string

Read from `Socialize.Api` configuration (`appsettings.json` /
`appsettings.Development.json` / env vars). In Docker it points at the `postgres`
service host; locally it points at `localhost`. Never hardcode credentials in code.
