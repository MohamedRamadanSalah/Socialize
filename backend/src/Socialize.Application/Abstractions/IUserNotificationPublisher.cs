using Socialize.Application.Common.Dtos;

namespace Socialize.Application.Abstractions;

public interface IUserNotificationPublisher
{
    Task PublishAsync(Guid recipientId, NotificationDto notification, CancellationToken cancellationToken = default);
}
