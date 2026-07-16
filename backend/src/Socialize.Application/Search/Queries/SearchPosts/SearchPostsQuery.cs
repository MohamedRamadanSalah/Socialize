using MediatR;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;

namespace Socialize.Application.Search.Queries.SearchPosts;

public record SearchPostsQuery(Guid CallerId, string Q, string? Cursor, int? Limit) : IRequest<CursorPage<PostDto>>;

public class SearchPostsQueryHandler : IRequestHandler<SearchPostsQuery, CursorPage<PostDto>>
{
    private readonly ISearchService _searchService;

    public SearchPostsQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public Task<CursorPage<PostDto>> Handle(SearchPostsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Q))
        {
            return Task.FromResult(new CursorPage<PostDto>(Array.Empty<PostDto>(), null));
        }

        return _searchService.SearchPostsAsync(request.CallerId, request.Q, request.Cursor, PageSize.Clamp(request.Limit), cancellationToken);
    }
}
