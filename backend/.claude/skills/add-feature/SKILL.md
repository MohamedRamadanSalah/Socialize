---
name: add-feature
description: Step-by-step recipe to add a new use case to the Socialize backend — a MediatR command or query plus handler, validator, DTO, and a thin controller endpoint with Swagger. Use whenever implementing a new backend capability (a new endpoint or business action).
---

# Add a Feature (CQRS use case)

Follow this order every time so the new code matches `backend-conventions`.

## 0. Decide: Command or Query?

- Changes state (create/update/delete, follow, like) → **Command**.
- Reads data (feed, profile, comments list) → **Query** (read-only, returns DTOs).

## 1. Domain (only if needed)

If the feature needs a new entity or field, add it to `Socialize.Domain` first, then
create/adjust the EF configuration in Infrastructure and add a migration
(see the `ef-migrations` skill). Skip this step if the entities already exist.

## 2. Application — the use case

Create a feature folder: `Application/<Feature>/Commands/<Name>/` or
`.../Queries/<Name>/`.

For a **command** `CreatePost`:

```csharp
// CreatePostCommand.cs
public record CreatePostCommand(string Content, IReadOnlyList<string> ImageUrls)
    : IRequest<PostDto>;

// CreatePostCommandValidator.cs  (FluentValidation)
public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}

// CreatePostCommandHandler.cs
public class CreatePostCommandHandler(IAppDbContext db, ICurrentUser user)
    : IRequestHandler<CreatePostCommand, PostDto>
{
    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken ct)
    {
        var post = new Post { AuthorId = user.Id, Content = request.Content /* ... */ };
        db.Posts.Add(post);
        await db.SaveChangesAsync(ct);
        return PostDto.From(post);
    }
}
```

Rules:
- Inject only Application interfaces (`IAppDbContext`, `ICurrentUser`, `IJwtService`,
  `INotificationPublisher`, `IFileStorage`) — never EF Core/Npgsql types.
- Queries must be read-only and return DTOs, never Domain entities.
- Do validation in the validator, not the handler. The validation pipeline behavior
  runs it automatically.
- For not-found / forbidden, throw `NotFoundException` / `ForbiddenException`; the
  exception middleware maps them to HTTP codes.

## 3. DTO

Add the response DTO in the same folder (or a shared `Application/<Feature>/Dtos`).
Provide a `From(entity)` mapping. Never expose the entity.

## 4. Api — thin controller endpoint

```csharp
[HttpPost]
[Authorize]
public async Task<ActionResult<PostDto>> Create(CreatePostRequest body, CancellationToken ct)
{
    var result = await _mediator.Send(new CreatePostCommand(body.Content, body.ImageUrls), ct);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}
```

- Keep the action thin: build the command/query, `Send`, return a typed result with the
  correct status code (`Created`, `Ok`, `NoContent`).
- Add `[Authorize]` for protected routes. Read the user id from `ICurrentUser`.
- XML doc comments / `[ProducesResponseType]` so Swagger documents it well.

## 5. Real-time (if the action should notify someone)

If the feature triggers a notification (like/comment/follow), publish via
`INotificationPublisher` after the state change — it persists the notification and
pushes it over SignalR. Do not call the hub directly from a handler.

## 6. Verify

Run the stack and exercise the endpoint via Swagger or curl — see the
`run-and-verify` skill. Add a handler unit test in `Socialize.Application.Tests` for
non-trivial logic.

## Checklist

- [ ] Command/Query + Handler + (Validator) + DTO in Application
- [ ] Only Application interfaces injected, no EF types leaked
- [ ] Thin controller action with correct status code and `[Authorize]`
- [ ] Notification published if applicable
- [ ] Verified via Swagger/curl; test added for real logic
