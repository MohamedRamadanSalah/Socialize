namespace Socialize.Application.Common.Pagination;

public record CursorPage<T>(IReadOnlyList<T> Items, string? NextCursor);
