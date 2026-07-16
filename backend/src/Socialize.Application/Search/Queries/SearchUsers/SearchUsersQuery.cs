using MediatR;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;

namespace Socialize.Application.Search.Queries.SearchUsers;

public record SearchUsersQuery(Guid CallerId, string Q, string? Cursor, int? Limit) : IRequest<CursorPage<UserSummaryDto>>;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, CursorPage<UserSummaryDto>>
{
    private readonly ISearchService _searchService;

    public SearchUsersQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public Task<CursorPage<UserSummaryDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Q))
        {
            return Task.FromResult(new CursorPage<UserSummaryDto>(Array.Empty<UserSummaryDto>(), null));
        }

        return _searchService.SearchUsersAsync(request.CallerId, request.Q, request.Cursor, PageSize.Clamp(request.Limit), cancellationToken);
    }
}
