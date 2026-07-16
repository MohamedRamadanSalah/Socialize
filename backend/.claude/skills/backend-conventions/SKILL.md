---
name: backend-conventions
description: The rulebook for the Socialize .NET backend — Clean Architecture layers, dependency direction, CQRS/MediatR structure, naming, and error handling. Read this BEFORE adding or modifying any code in the backend so changes stay consistent with the established architecture.
---

# Socialize Backend Conventions

The authoritative architecture rules for this .NET 8 solution. Follow them for every
change. When something here conflicts with a quick shortcut, the convention wins.

## Solution layout

```
src/
├── Socialize.Domain/          # Entities, enums, domain events. NO dependencies.
├── Socialize.Application/     # CQRS commands/queries, DTOs, validators, interfaces.
├── Socialize.Infrastructure/  # EF Core, repositories, JWT, file storage, SignalR impl.
└── Socialize.Api/             # Controllers, middleware, DI, SignalR hubs, Program.cs.
tests/
└── Socialize.Application.Tests/
```

## Dependency rule (never violate)

`Api → Infrastructure → Application → Domain`

- **Domain** references nothing (no EF Core, no ASP.NET).
- **Application** references Domain only. It declares interfaces
  (e.g. `IAppDbContext`, `IJwtService`, `INotificationPublisher`); it never references
  EF Core or Npgsql types directly.
- **Infrastructure** implements those interfaces and owns EF Core / external concerns.
- **Api** wires everything up via DI and exposes HTTP + SignalR. No business logic here.

If a change would make an inner layer depend on an outer one, stop and rethink —
introduce an interface in Application instead.

## CQRS with MediatR

Every use case is a MediatR request + handler. Group by feature, then by
Commands/Queries:

```
Application/Posts/Commands/CreatePost/
    CreatePostCommand.cs        # : IRequest<PostDto>
    CreatePostCommandHandler.cs
    CreatePostCommandValidator.cs   # FluentValidation
Application/Posts/Queries/GetFeed/
    GetFeedQuery.cs             # : IRequest<PagedResult<FeedItemDto>>
    GetFeedQueryHandler.cs
    FeedItemDto.cs
```

- **Commands** change state and return a small DTO (or `Unit`).
- **Queries** are read-only and return DTOs — never return Domain entities to the API.
- Cross-cutting concerns run as MediatR **pipeline behaviors**: validation, logging,
  and exception mapping. Do not scatter try/catch in handlers for validation.

## Controllers

- Thin. A controller action builds the command/query and calls `_mediator.Send(...)`.
- No EF Core, no business rules in controllers.
- Return typed results (`Ok`, `CreatedAtAction`, `NoContent`) with proper status codes.
- Protected endpoints use `[Authorize]`; read the current user id from claims via a
  shared `ICurrentUser` abstraction, not by parsing the token manually.

## DTOs and mapping

- API never exposes Domain entities directly — always map to a DTO.
- Keep DTOs in the Application layer next to their query/command.

## Error handling

- Throw domain/application exceptions (`NotFoundException`, `ForbiddenException`,
  `ValidationException`) and let the global exception-mapping middleware translate them
  to `404 / 403 / 400` with a consistent problem-details body.
- Never return `500` for an expected condition (missing entity, unauthorized action).

## Naming & style

- One public type per file, file name = type name.
- Async everywhere for I/O; suffix async methods with `Async` and accept a
  `CancellationToken`.
- Records for DTOs and commands/queries where practical.

## Persistence

- All EF Core access goes through the `IAppDbContext` interface from Application.
- Entity configuration lives in `Infrastructure` via `IEntityTypeConfiguration<T>`.
- See the `ef-migrations` skill for the migration workflow.

## When adding a feature

Use the `add-feature` skill — it encodes the exact file set and order.
