using MediatR;
using Microsoft.Extensions.Logging;

namespace Socialize.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling {RequestName}", requestName);
        try
        {
            var response = await next();
            _logger.LogInformation("Handled {RequestName}", requestName);
            return response;
        }
        catch (Exception ex) when (ex is not Common.Exceptions.NotFoundException
            and not Common.Exceptions.ForbiddenException
            and not Common.Exceptions.ConflictException
            and not Common.Exceptions.UnauthorizedAppException
            and not Common.Exceptions.ValidationAppException)
        {
            _logger.LogError(ex, "Unhandled exception while handling {RequestName}", requestName);
            throw;
        }
    }
}
