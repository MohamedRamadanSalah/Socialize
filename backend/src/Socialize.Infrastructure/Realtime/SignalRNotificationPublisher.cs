using Microsoft.AspNetCore.SignalR;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;

namespace Socialize.Infrastructure.Realtime;

public class SignalRNotificationPublisher : INotificationPublisher
{
    private readonly IHubContext<NotificationsHub> _hubContext;

    public SignalRNotificationPublisher(IHubContext<NotificationsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task PublishAsync(Guid recipientId, NotificationDto notification, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.User(recipientId.ToString())
            .SendAsync("ReceiveNotification", notification, cancellationToken);
    }
}
